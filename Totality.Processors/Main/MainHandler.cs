﻿using System;
using System.Collections.Generic;
using System.Linq;
using Totality.CommonClasses;
using Totality.Handlers.Diplomatical;
using Totality.Handlers.News;
using Totality.Handlers.Nuke;
using Totality.Model;
using Totality.Model.Diplomatical;
using Totality.Model.Interfaces;
using Totality.OrderHandlers;

namespace Totality.Handlers.Main
{
    public class MainHandler : AbstractHandler
    {
        private Dictionary<string, Queue<Order>> _ordersBase = new Dictionary<string, Queue<Order>>();
        public ITransmitter Transmitter { get; set; }
        private List<IMinisteryHandler> _ministeryHandlers = new List<IMinisteryHandler>();
        private List<Order> _currentOrdersLine = new List<Order>();
        private NukeHandler _nukeHandler;
        public DiplomaticalHandler DipHandler { get; set; }
        private List<Battle> battles = new List<Battle>();
        private int _stepOfGame = 0;


        public MainHandler(NewsHandler newsHandler, IDataLayer dataLayer, ILogger logger, NukeHandler nukeHandler) : base(newsHandler, dataLayer, logger)
        {
            _nukeHandler = nukeHandler;

            _ministeryHandlers.Add(new MinIndustryHandler(newsHandler, dataLayer, logger));
            _ministeryHandlers.Add(new MinFinanceHandler(newsHandler, dataLayer, logger));
            _ministeryHandlers.Add(new MinMilitaryHandler(newsHandler, dataLayer, nukeHandler, logger));
            _ministeryHandlers.Add(new MinForeignHandler(newsHandler, dataLayer, logger));
            _ministeryHandlers.Add(new MinMediaHandler(newsHandler,dataLayer, logger));
            _ministeryHandlers.Add(new MinInnerHandler(newsHandler, dataLayer, logger));
            _ministeryHandlers.Add(new MinSecurityHandler(newsHandler, dataLayer, logger));
            _ministeryHandlers.Add(new MinScienceHandler(newsHandler, dataLayer, logger));
            _ministeryHandlers.Add(new MinPremierHandler(newsHandler, dataLayer, logger));

            (_ministeryHandlers[(short)Mins.Security] as MinSecurityHandler).SecretOrderProcessed += SecretOrderProcessed;

            _nukeHandler.AttackEnded += _nukeHandler_AttackEnded;
        }


        public Country GetCountry(string name)
        {
            return _dataLayer.GetCountry(name);
        }

        private void SecretOrderProcessed(Order order)
        {
            order.isSecret = true;
            _ordersBase[order.CountryName].Enqueue(order);
        }

        public void AddOrder(Order newOrder)
        {
            _ordersBase[newOrder.CountryName].Enqueue(newOrder);
        }

        public void AddOrders(List<Order> orders)
        {
            for (int i = 0; i < orders.Count; i++)
            _ordersBase[orders[i].CountryName].Enqueue(orders[i]);
        }

        public void FinishStep()
        {
            _stepOfGame++;
            try
            {
                ProcessOrders();
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
            }

            foreach (KeyValuePair<string, Queue<Order>> country in _ordersBase)
            {
                Country cur = (Country)_dataLayer.GetCountry(country.Key);
                cur.Step = _stepOfGame;
                cur.Mood = 100;
                if (cur.IsRepressed) cur.Mood += 10;
                _dataLayer.UpdateCountry(cur);
            }

            _nukeHandler.StartAttack();

        }

        private void _nukeHandler_AttackEnded()
        {
            updateMoney();

            updateMilitaryPower();

            updateResources();

            try
            {
                followDipContracts();
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
            }

            updateUranus();

            updateIndustries();

            fightBattles();

            foreach (KeyValuePair<string, Queue<Order>> cou in _ordersBase)
            {
                Country cur = _dataLayer.GetCountry(cou.Key);
                for (int i = 0; i < cur.MinsBlocks.Count(); i++)
                {
                    if (cur.MinsBlocks[i] > 0)
                        cur.MinsBlocks[i]--;
                }
                _dataLayer.UpdateCountry(cur);
            }

            updateMood();

            try
            {
                updateCurrencyRatios();
            }
            catch(Exception e)
            {
                _log.Error(e.Message);
            }

            DipHandler.SendContractsToAll();

            _newsHandler.Countries = _ordersBase.Keys.ToList();
            _newsHandler.SendNews();

            try
            {
                updateClients();
            }
            catch(Exception e)
            {
                _log.Error(e.Message);
            }

            _dataLayer.Save("lastSave.json");
        }

        private void followDipContracts()
        {
            var contracts = _dataLayer.GetContractList();
            for (int i = 0; i < contracts.Count; i++)
            {
                Country country1, country2;
                var contract = contracts[i];

                if (!contract.IsFinished)
                    switch (contract.Type)
                    {
                        case Model.Diplomatical.DipMsg.Types.Alliance:
                            country1 = _dataLayer.GetCountry(contract.From);
                            country2 = _dataLayer.GetCountry(contract.To);
                            if (country1.Alliance != country1.Name || country2.Alliance != country2.Name)
                            {
                                _newsHandler.AddNews(contract.From, new Model.News(true) { text = "Не удалось заключить союз со страной" + contract.To });
                                _newsHandler.AddNews(contract.To, new Model.News(true) { text = "Не удалось заключить союз со страной" + contract.From });
                                contract.IsFinished = true;
                                break;
                            }
                            country1.Alliance = contract.Text;
                            country1.IsBoss = true;
                            country2.Alliance = contract.Text;
                            _dataLayer.UpdateCountry(country1);
                            _dataLayer.UpdateCountry(country2);
                            contract.IsFinished = true;
                            _newsHandler.AddBroadNews(new Model.News(false) { text = "Страна " + country1.Name + " добавила страну " + country2.Name + " в альянс " + contract.Text + "."});
                            break;
                        case Model.Diplomatical.DipMsg.Types.CurrencyAlliance:
                            country1 = _dataLayer.GetCountry(contract.From);
                            country2 = _dataLayer.GetCountry(contract.To);

                            if (country1.CurrencyAlliance != country1.Name || country2.CurrencyAlliance != country2.Name)
                            {
                                _newsHandler.AddNews(contract.From, new Model.News(true) { text = "Не удалось заключить союз со страной" + contract.To });
                                _newsHandler.AddNews(contract.To, new Model.News(true) { text = "Не удалось заключить союз со страной" + contract.From });
                                contract.IsFinished = true;
                                _dataLayer.BreakContract(contract.Id);
                                break;
                            }

                            country2.CurrencyAlliance = country1.CurrencyAlliance;
                            _dataLayer.UpdateCountry(country2);
                            contract.IsFinished = true;

                            break;
                        case Model.Diplomatical.DipMsg.Types.MilitaryTraining:
                            country1 = _dataLayer.GetCountry(contract.From);
                            country2 = _dataLayer.GetCountry(contract.To);
                            country1.MilitaryPower *= 1.02;
                            country2.MilitaryPower *= 1.02;
                            _dataLayer.UpdateCountry(country1);
                            _dataLayer.UpdateCountry(country2);
                            _newsHandler.AddBroadNews(new Model.News(false) { text = "Страна " + country1.Name + " провела военные учения со страной " + country2.Name + "." });
                            contract.IsFinished = true;
                            _dataLayer.BreakContract(contract.Id);
                            break;
                        case Model.Diplomatical.DipMsg.Types.Other:
                            contract.IsFinished = true;
                            _newsHandler.AddNews(contract.From, new Model.News(true) { text = "Мы и страна " + contract.To  + " ратифицировали договор: " + contract.Text});
                            _newsHandler.AddNews(contract.To, new Model.News(true) { text = "Мы и страна " + contract.From + " ратифицировали договор: " + contract.Text });
                            break;
                        case Model.Diplomatical.DipMsg.Types.Peace:
                            var countries = _dataLayer.GetCountries();
                            country1 = _dataLayer.GetCountry(contract.From);
                            country2 = _dataLayer.GetCountry(contract.To);
                            _log.Trace("Заключен мир между " + country1.Name + " и " + country2.Name);

                            if (!country1.IsBoss && country1.Alliance != country1.Name || !country2.IsBoss && country2.Alliance != country2.Name)
                            {
                                _newsHandler.AddNews(contract.From, new Model.News(true) { text = "Мы не имеем права заключать мир со страной " + contract.To + ", полномочия есть только у глав альянсов."});
                                _newsHandler.AddNews(contract.To, new Model.News(true) { text = "Мы не имеем права заключать мир со страной " + contract.From + ", полномочия есть только у глав альянсов." });
                                contract.IsFinished = true;
                                _dataLayer.BreakContract(contract.Id);
                                break;
                            }

                            var alliance1 = countries.Values.Where(x => x.Alliance == country1.Alliance).ToList();
                            var alliance2 = countries.Values.Where(x => x.Alliance == country2.Alliance).ToList();

                            for (int al1 = 0; al1 < alliance1.Count; al1++)
                            {
                                for (int c = 0; c < alliance1[al1].WarList.Count; c++)
                                {
                                    if (alliance2.Any(x => x.Name == alliance1[al1].WarList[c]))
                                        alliance1[al1].WarList.Remove(alliance1[al1].WarList[c]);
                                }
                            }

                            for (int al2 = 0; al2 < alliance2.Count; al2++)
                            {
                                for (int c = 0; c < alliance2[al2].WarList.Count; c++)
                                {
                                    if (alliance1.Any(x => x.Name == alliance2[al2].WarList[c]))
                                        alliance2[al2].WarList.Remove(alliance2[al2].WarList[c]);
                                }
                            }

                            battles.RemoveAll(x => x.Alliances.Any(t => t.Key == country1.Alliance) && x.Alliances.Any(p => p.Key == country2.Alliance));

                            _newsHandler.AddBroadNews(new Model.News(false) { text = "Альянсы " + country1.Alliance + " и " + country2.Alliance + " заключили мир." });
                            contract.IsFinished = true;
                            _dataLayer.BreakContract(contract.Id);
                            break;

                        case Model.Diplomatical.DipMsg.Types.Trade:
                            if (contract.Time == 0)
                            {
                                contract.IsFinished = true;
                                _dataLayer.BreakContract(contract.Id);
                                break;
                            }
                            contract.Time--;

                            country1 = _dataLayer.GetCountry(contract.From);
                            country2 = _dataLayer.GetCountry(contract.To);
                            var res = (double)_dataLayer.GetProperty(country1.Name, "Final" + contract.Res) - (double)_dataLayer.GetProperty(country1.Name, "Used" + contract.Res);
                            var needMoney = FinancialTools.GetExchangeCost(contract.Price, country2.NationalCurrencyDemand, country1.NationalCurrencyDemand, _dataLayer.GetCurrencyOnStock(country2.Name), _dataLayer.GetCurrencyOnStock(country1.Name), country2.FinalHeavyIndustry + country2.FinalLightIndustry, country1.FinalHeavyIndustry + country1.FinalLightIndustry);

                            if (needMoney > country2.Money)
                            {
                                _newsHandler.AddNews(country1.Name, new Model.News(true) { text = "Страна " + country2.Name + " не смогла заплатить по торговому контракту: " + contract.Description});
                                _newsHandler.AddNews(country2.Name, new Model.News(true) { text = "Мы не смогли заплатить стране " + country2.Name + " по торговому контракту: " + contract.Description });
                                break;
                            }

                            if (res >= contract.Count)
                            {
                                _dataLayer.SetProperty(country1.Name, "Used" + contract.Res, (double)_dataLayer.GetProperty(country1.Name, "Used" + contract.Res) + contract.Count);
                                _dataLayer.SetProperty(country2.Name, "Final" + contract.Res, (double)_dataLayer.GetProperty(country2.Name, "Final" + contract.Res) + contract.Count);

                                _dataLayer.SetProperty(country1.Name, "Money", (long)_dataLayer.GetProperty(country1.Name, "Money") + contract.Price);
                                _dataLayer.SetProperty(country2.Name, "Money", (long)_dataLayer.GetProperty(country2.Name, "Money") - needMoney);

                                _dataLayer.SetCurrencyOnStock(country1.Name, _dataLayer.GetCurrencyOnStock(country1.Name) - contract.Price);
                                _dataLayer.SetCurrencyOnStock(country2.Name, _dataLayer.GetCurrencyOnStock(country2.Name) + needMoney);

                                _newsHandler.AddNews(country1.Name, new Model.News(true) { text = "Страна " + country2.Name + " оплатила очередную партию по торговому контракту: " + contract.Description + ", " + String.Format("{0:0,0}",contract.Price) });
                                _newsHandler.AddNews(country2.Name, new Model.News(true) { text = "Мы получили очередную партию товара из страны " + country1.Name + " по торговому контракту: " + contract.Description + ", в количестве " + contract.Count });
                            }
                            else
                            {
                                _newsHandler.AddNews(country2.Name, new Model.News(true) { text = "Страна " + country1.Name + " не предоставить товар по торговому контракту: " + contract.Description });
                                _newsHandler.AddNews(country1.Name, new Model.News(true) { text = "Мы не смогли предоставить товар стране " + country2.Name + " по торговому контракту: " + contract.Description });
                            }

                            break;
                        case Model.Diplomatical.DipMsg.Types.Transfer:
                            country1 = _dataLayer.GetCountry(contract.From);
                            country2 = _dataLayer.GetCountry(contract.To);
                            if (country1.Money >= contract.Count)
                            {
                                country1.Money -= contract.Count;
                                country2.CurrencyAccounts[country1.Name] += contract.Count;
                                _dataLayer.UpdateCountry(country1);
                                _dataLayer.UpdateCountry(country2);
                                _newsHandler.AddNews(country1.Name, new Model.News(true) { text = "В страну " + country2.Name + " совершен перевод: " + String.Format("{0:0,0}", contract.Count)});
                                _newsHandler.AddNews(country2.Name, new Model.News(true) { text = "Страна " + country1.Name + " совершила нам перевод в своей валюте: " + String.Format("{0:0,0}", contract.Count) });
                            }
                            contract.IsFinished = true;
                            _dataLayer.BreakContract(contract.Id);
                            break;
                    }
            }
        }

        public Dictionary<string, long> GetCurrencyDemands() // OPTIMISE
        {
            Dictionary<string, long> demands = new Dictionary<string, long>();
            var countries = _dataLayer.GetCountries();

            foreach (Country c in countries.Values)
            {
                demands.Add(c.Name, c.NationalCurrencyDemand);
            }

            return demands;
        }

        public Dictionary<string, double> GetSumIndPowers()
        {
            Dictionary<string, double> SumIndPowers = new Dictionary<string, double>();
            var countries = _dataLayer.GetCountries();

            foreach (Country c in countries.Values)
            {
                SumIndPowers.Add(c.Name, c.FinalHeavyIndustry + c.FinalLightIndustry);
            }

            return SumIndPowers;
        }

        public Dictionary<string, long> GetCurrencyStock()
        {
            return _dataLayer.GetStock();
        }

        private void updateMoney()
        {
            var countries = _dataLayer.GetCountries();
            foreach (KeyValuePair<string, Queue<Order>> country in _ordersBase)
            {
                countries[country.Key].Money += (long)(countries[country.Key].FinalLightIndustry * Constants.LightPowerProfit * (countries[country.Key].TaxesLvl/100.0));
                _dataLayer.UpdateCountry(countries[country.Key]);
            }
        }



        private void fightBattles()
        {
            

            var countries = _dataLayer.GetCountries();
            foreach (KeyValuePair<string, Queue<Order>> country in _ordersBase)
            {
                for (int i = 0; i < countries[country.Key].WarList.Count; i++)
                {
                    var we = countries[country.Key];
                    var enemy = countries[countries[country.Key].WarList[i]];
                    if (!battles.Any(x => x.Alliances.ContainsKey(we.Alliance) && x.Alliances.ContainsKey(enemy.Alliance)))
                    {
                        var battle = new Battle(we.Alliance, enemy.Alliance);

                        var allies = countries.Values.Where(x => x.Alliance.Equals(we.Alliance)).ToList();
                        foreach (Country al in allies)
                            battle.Alliances[we.Alliance].Add(al.Name);

                        var enemies = countries.Values.Where(x => x.Alliance.Equals(enemy.Alliance)).ToList();
                        foreach (Country en in enemies)
                            battle.Alliances[enemy.Alliance].Add(en.Name);

                        battles.Add(battle);
                    } else
                    {
                        var curBattle = battles.First(x => x.Alliances.ContainsKey(we.Alliance) && x.Alliances.ContainsKey(enemy.Alliance));
                        if (!curBattle.Alliances[we.Alliance].Contains(we.Name)) curBattle.Alliances[we.Alliance].Add(we.Name);
                        if (!curBattle.Alliances[enemy.Alliance].Contains(enemy.Name)) curBattle.Alliances[enemy.Alliance].Add(enemy.Name);
                    }
                }
            }

            for (int i = 0; i < battles.Count; i++)
            {
                battles[i].Alliances.First().Value.RemoveAll(x => _dataLayer.GetCountry(x).Alliance != battles[i].Alliances.First().Key);
                battles[i].Alliances.Last().Value.RemoveAll(x => _dataLayer.GetCountry(x).Alliance != battles[i].Alliances.Last().Key);

                if (battles[i].step == 0)
                {
                    battles[i].step++;
                    continue;
                }

                Dictionary<string, double> powers = new Dictionary<string, double>();
                foreach (KeyValuePair<string, List<string>> al in battles[i].Alliances)
                {
                    powers.Add(al.Key, 0);
                    foreach (string country in al.Value)
                    {
                        powers[al.Key] += countries[country].MilitaryPower;
                    }
                }

                try
                {
                    var armies = battles[i].Alliances.ToList();
                    for (int j = 0; j < 2; j++)
                    {
                        for (int k = 0; k < armies[j].Value.Count; k++)
                        {
                            for (int p = 0; p < 2; p++)
                            {
                                try
                                {
                                    if (p == j) continue;
                                    countries[armies[j].Value[k]].MilitaryPower -= (powers[armies[p].Key]/
                                                                                    armies[j].Value.Count)*0.75;
                                        // ERROR
                                    if (countries[armies[j].Value[k]].MilitaryPower < 0)
                                        countries[armies[j].Value[k]].MilitaryPower = 0;

                                    if (powers[armies[p].Key] > powers[armies[j].Key])
                                    {
                                        if ((powers[armies[p].Key] - powers[armies[j].Key])/
                                            (double) powers[armies[j].Key] >
                                            0.5)
                                            _newsHandler.AddBroadNews(new Model.News(true)
                                            {
                                                text =
                                                    "При подавляющем превосходстве армия " + armies[p].Key +
                                                    " нанесла сокрушительное поражение армии " + armies[j].Key + "."
                                            });
                                        else if ((powers[armies[p].Key] - powers[armies[j].Key])/
                                                 (double) powers[armies[j].Key] >
                                                 0.2)
                                            _newsHandler.AddBroadNews(new Model.News(true)
                                            {
                                                text =
                                                    "Пользуясь превосходством, армия " + armies[p].Key +
                                                    " нанесла поражение армии " + armies[j].Key + "."
                                            });
                                        else if ((powers[armies[p].Key] - powers[armies[j].Key])/
                                                 (double) powers[armies[j].Key] >
                                                 0)
                                            _newsHandler.AddBroadNews(new Model.News(true)
                                            {
                                                text =
                                                    "Пользуясь незначительным превосходством, армия " + armies[p].Key +
                                                    " ослабила армию " + armies[j].Key + "."
                                            });


                                        countries[armies[j].Value[k]].ResOil -= ((powers[armies[p].Key] -
                                                                                  powers[armies[j].Key])/
                                                                                 (armies[j].Value.Count))*0.25;
                                        if (countries[armies[j].Value[k]].ResOil < 0)
                                            countries[armies[j].Value[k]].ResOil = 0;

                                        countries[armies[j].Value[k]].ResSteel -= ((powers[armies[p].Key] -
                                                                                    powers[armies[j].Key])/
                                                                                   (armies[j].Value.Count))*0.25;
                                        if (countries[armies[j].Value[k]].ResSteel < 0)
                                            countries[armies[j].Value[k]].ResSteel = 0;

                                        countries[armies[j].Value[k]].ResWood -= ((powers[armies[p].Key] -
                                                                                   powers[armies[j].Key])/
                                                                                  (armies[j].Value.Count))*0.25;
                                        if (countries[armies[j].Value[k]].ResWood < 0)
                                            countries[armies[j].Value[k]].ResWood = 0;

                                        countries[armies[j].Value[k]].ResAgricultural -= ((powers[armies[p].Key] -
                                                                                           powers[armies[j].Key])/
                                                                                          (armies[j].Value.Count))*0.25;
                                        if (countries[armies[j].Value[k]].ResAgricultural < 0)
                                            countries[armies[j].Value[k]].ResAgricultural = 0;

                                        countries[armies[j].Value[k]].PowerHeavyIndustry -= ((powers[armies[p].Key] -
                                                                                              powers[armies[j].Key])/
                                                                                             (armies[j].Value.Count))*
                                                                                            0.10;
                                        if (countries[armies[j].Value[k]].PowerHeavyIndustry < 0)
                                            countries[armies[j].Value[k]].PowerHeavyIndustry = 0;

                                        countries[armies[j].Value[k]].PowerLightIndustry -= ((powers[armies[p].Key] -
                                                                                              powers[armies[j].Key])/
                                                                                             (armies[j].Value.Count))*
                                                                                            0.10;
                                        if (countries[armies[j].Value[k]].PowerLightIndustry < 0)
                                            countries[armies[j].Value[k]].PowerLightIndustry = 0;
                                    }
                                    else
                                        _newsHandler.AddBroadNews(new Model.News(true)
                                        {
                                            text =
                                                "Атака армии " + armies[p].Key + " позиций " + armies[j].Key +
                                                "оказалась неудачной и не изменила баланса сил."
                                        });
                                }
                                catch (Exception e)
                                {
                                    _log.Error("SMTHNG WITH ARMIES: " + e.Message);
                                }
                            }
                        }
                    }
                } catch (Exception e) { _log.Error("SMTHING WITH ARMIES 2: " + e.Message);}
            }

            var cs = countries.Values.ToList();
            for (int c = 0; c < countries.Values.Count; c++)
            {
                _dataLayer.UpdateCountry(cs[c]);
            }
        }

        public void ProcessOrders()
        {
            while (_ordersBase.Any(x => x.Value.Any()))
            {
                foreach (KeyValuePair<string, Queue<Order>> qu in _ordersBase.Where(x => x.Value.Any()))
                {
                    _currentOrdersLine.Add(qu.Value.Dequeue());
                }

                //_currentOrdersLine.OrderBy(x => ); случайно отсортировать

                for (int i = 0; i < _currentOrdersLine.Count; i++)
                {
                    if (_currentOrdersLine[i].Ministery.Equals((short)Mins.Secret) )
                        _ministeryHandlers[(short)Mins.Security].ProcessOrder(_currentOrdersLine[i]);
                    else
                    _ministeryHandlers[_currentOrdersLine[i].Ministery].ProcessOrder(_currentOrdersLine[i]);
                }
                _currentOrdersLine.Clear();
            }
        }

        private void updateResources()
        {
            foreach (KeyValuePair<string, Queue<Order>> country in _ordersBase)
            {
                int exp0 = (int)_dataLayer.GetProperty(country.Key, "ExtractExperience");
                int exp = 0;

                if ((bool)_dataLayer.GetProperty(country.Key, "IsRepressed"))
                {
                    _newsHandler.AddNews(country.Key, new Model.News(true) { text = "Репрессии наносят урон промышленности " });
                }
                if ((bool)_dataLayer.GetProperty(country.Key, "IsRiot"))
                {
                    _newsHandler.AddNews(country.Key, new Model.News(true) { text = "Бунт наносит урон промышленности! " });
                }

                foreach (string res in new List<string>{ "Steel", "Oil", "Wood", "Agricultural"} )
                {
                    var extract = (double)_dataLayer.GetProperty(country.Key, "Res" + res);
                    if ((bool)_dataLayer.GetProperty(country.Key, "IsRepressed"))
                    {
                        extract *= 0.98;
                    }
                    if ((bool)_dataLayer.GetProperty(country.Key, "IsRiot"))
                    {
                        extract *= 0.92;
                    }
                    _dataLayer.SetProperty(country.Key, "Res" + res, extract);

                    extract *=  Math.Pow(Constants.ScienceBuff, (int)_dataLayer.GetProperty(country.Key, "ExtractScienceLvl" ));
                    // сюда добавить другие баффы
                    _dataLayer.SetProperty(country.Key, "Final" + res, extract);                   
                    double usedRes = 0;
                    switch (res)
                    {
                        case "Steel":
                            usedRes = ((double)_dataLayer.GetProperty(country.Key, "PowerHeavyIndustry")) * Constants.IndustrySteelCoeff;
                            
                            break;
                        case "Oil":
                            usedRes = ((double)_dataLayer.GetProperty(country.Key, "PowerHeavyIndustry")) * Constants.IndustryOilCoeff;
                            break;
                        case "Wood":
                            usedRes = ((double)_dataLayer.GetProperty(country.Key, "PowerLightIndustry")) * Constants.IndustryWoodCoeff;
                            break;
                        case "Agricultural":
                            usedRes = ((double)_dataLayer.GetProperty(country.Key, "PowerLightIndustry")) * Constants.IndustryAgroCoeff;
                            break;
                    }
                    exp += (int)usedRes;
                    _dataLayer.SetProperty(country.Key, "Used" + res, usedRes);
                }

                Random r = new Random();
                exp /= 4;
                exp = (int)(r.NextDouble() * exp) + exp0;
                _dataLayer.SetProperty(country.Key, "ExtractExperience", exp);
            }
        }

        private void updateUranus()
        {
            foreach (KeyValuePair<string, Queue<Order>> country in _ordersBase)
            {
                var uranus = (double)_dataLayer.GetProperty(country.Key, "ResUranus");
                var uranusProduction = (double)_dataLayer.GetProperty(country.Key, "ProdUranus");
                uranus += uranusProduction;
                _dataLayer.SetProperty(country.Key, "ResUranus", uranus);
            }
        }

        private void updateIndustries()
        {
            foreach (KeyValuePair<string, Queue<Order>> country in _ordersBase)
            {
                    _dataLayer.SetProperty(country.Key, "UsedLIpower", 0);
                    var LIpower = (double)_dataLayer.GetProperty(country.Key, "PowerLightIndustry");

                if ((bool)_dataLayer.GetProperty(country.Key, "IsRepressed"))
                    LIpower *= 0.98;
                if ((bool)_dataLayer.GetProperty(country.Key, "IsRiot"))
                    LIpower *= 0.92;
                _dataLayer.SetProperty(country.Key, "PowerLightIndustry", LIpower);

                Random r = new Random();
                var lE = (int)_dataLayer.GetProperty(country.Key, "LightExperience");
                lE += (int)(LIpower * r.NextDouble());
                _dataLayer.SetProperty(country.Key, "LightExperience", lE);

                _dataLayer.SetProperty(country.Key, "UsedHIpower", 0);
                var HIpower = (double)_dataLayer.GetProperty(country.Key, "PowerHeavyIndustry");

                if ((bool)_dataLayer.GetProperty(country.Key, "IsRepressed"))
                    HIpower *= 0.98;
                if ((bool)_dataLayer.GetProperty(country.Key, "IsRiot"))
                    HIpower *= 0.92;
                _dataLayer.SetProperty(country.Key, "PowerHeavyIndustry", HIpower);

                var hE = (int)_dataLayer.GetProperty(country.Key, "HeavyExperience");
                hE += (int)(HIpower * r.NextDouble());
                _dataLayer.SetProperty(country.Key, "HeavyExperience", hE);

                var upgradeCost = (long)(Constants.IndustryUpgradeCostRate * (LIpower + HIpower));
                _dataLayer.SetProperty(country.Key, "IndustryUpgradeCost", upgradeCost);

                LIpower *= Math.Pow(Constants.ScienceBuff, (int)_dataLayer.GetProperty(country.Key, "LightScienceLvl"));
                // сюда добавить другие баффы
                _dataLayer.SetProperty(country.Key, "FinalLightIndustry", LIpower);

                HIpower *= Math.Pow(Constants.ScienceBuff, (int)_dataLayer.GetProperty(country.Key, "HeavyScienceLvl"));
                // сюда добавить другие баффы
                _dataLayer.SetProperty(country.Key, "FinalHeavyIndustry", HIpower);
            }
        }

        private void updateMood()
        {
            foreach (KeyValuePair<string, Queue<Order>> country in _ordersBase)
            {
                Country cur = (Country)_dataLayer.GetCountry(country.Key);
                foreach (KeyValuePair<string, short> prop in cur.MassMedia)
                {
                    Country cur2 = (Country)_dataLayer.GetCountry(prop.Key);
                    if (prop.Value == 1)
                    {
                        cur2.Mood -= 7;
                        cur.Money -= Constants.PropagandaCost;
                        _newsHandler.AddNews(cur.Name, new Model.News(false) { text = "Наши СМИ повышают волнения в стране " + cur2.Name  });
                        _newsHandler.AddNews(cur2.Name, new Model.News(false) { text = "Подлые газетчики из страны " + cur.Name + " повышают волнения в нашей стране!"});
                    }
                    else if (prop.Value == 2)
                    {
                        cur2.Mood += 7;
                        cur2.Money -= Constants.PropagandaCost;
                        _newsHandler.AddNews(cur.Name, new Model.News(false) { text = "Наши СМИ повышают настроение населения в стране " + cur2.Name });
                        _newsHandler.AddNews(cur2.Name, new Model.News(false) { text = "Братские СМИ из страны " + cur.Name + " улучшают настроение населения в нашей стране." });
                    }
                    _dataLayer.UpdateCountry(cur2);
                }
                _dataLayer.UpdateCountry(cur);
            }

                foreach (KeyValuePair<string, Queue<Order>> country in _ordersBase)
            {
                Country cur = (Country)_dataLayer.GetCountry(country.Key);
                cur.Mood -= (cur.TaxesLvl-20) / 2.0;
                cur.Mood *= Math.Pow(1.05, cur.InnerLvl - 1);
                if ( (cur.PowerHeavyIndustry - cur.PowerLightIndustry)/ cur.PowerLightIndustry > 0.3)
                {
                    cur.Mood *= (1 - (cur.PowerHeavyIndustry - cur.PowerLightIndustry) / (cur.PowerLightIndustry * 3));
                }

                if (cur.Mood > 100) cur.Mood = 100;

                if (cur.Mood < 50 && !cur.IsRiot)
                {
                    var chance = 50 - cur.Mood;
                    Random r = new Random();
                    var result = r.Next(1, 100);
                    if (result <= chance)
                    {
                        cur.IsRiot = true;
                        _newsHandler.AddBroadNews(new Model.News(false) { text = "В стране " + cur.Name + " начался бунт!" });
                    }
                }
                _dataLayer.UpdateCountry(cur);

            }

        }

        private void updateCurrencyRatios()
        {
            var countries = _dataLayer.GetCountries();
            foreach (KeyValuePair<string, Queue<Order>> country in _ordersBase)
            {
                foreach (KeyValuePair<string, Queue<Order>> anotherCountry in _ordersBase)
                {
                    if (country.Key.Equals(anotherCountry.Key)) continue;

                    if (countries[country.Key].CurrencyRatios.ContainsKey(anotherCountry.Key))
                        countries[country.Key].CurrencyRatios[anotherCountry.Key] = FinancialTools.GetCurrencyRation(
                            countries[country.Key].NationalCurrencyDemand,
                            countries[anotherCountry.Key].NationalCurrencyDemand,
                            _dataLayer.GetCurrencyOnStock(country.Key),
                            _dataLayer.GetCurrencyOnStock(anotherCountry.Key),
                            countries[country.Key].FinalHeavyIndustry + countries[country.Key].FinalLightIndustry,
                            countries[anotherCountry.Key].FinalHeavyIndustry + countries[anotherCountry.Key].FinalLightIndustry);
                    else
                    {
                        countries[country.Key].CurrencyRatios.Add
                            (anotherCountry.Key, 
                                FinancialTools.GetCurrencyRation(
                                countries[country.Key].NationalCurrencyDemand,
                                countries[anotherCountry.Key].NationalCurrencyDemand,
                                _dataLayer.GetCurrencyOnStock(country.Key),
                                _dataLayer.GetCurrencyOnStock(anotherCountry.Key),
                                countries[country.Key].FinalHeavyIndustry + countries[country.Key].FinalLightIndustry,
                                countries[anotherCountry.Key].FinalHeavyIndustry + countries[anotherCountry.Key].FinalLightIndustry
                                )
                            );
                    }

                    if (!countries[country.Key].CurrencyAccounts.ContainsKey(anotherCountry.Key))
                        countries[country.Key].CurrencyAccounts.Add(anotherCountry.Key, 0);

                }
                _dataLayer.UpdateCountry(countries[country.Key]);
            }
        }


        private void updateClients()
        {
            Transmitter.UpdateClients( _dataLayer.GetCountries() );
        }

        private void updateMilitaryPower()
        {
            foreach (KeyValuePair<string, Queue<Order>> country in _ordersBase)
            {
                var HIpower = (double)_dataLayer.GetProperty(country.Key, "FinalHeavyIndustry");
                var usedHIpower = (double)_dataLayer.GetProperty(country.Key, "UsedHIpower");
                var militaryPower = (double)_dataLayer.GetProperty(country.Key, "MilitaryPower");
                var militaryScience = (int)_dataLayer.GetProperty(country.Key, "MilitaryScienceLvl");

                if ((bool)_dataLayer.GetProperty(country.Key, "IsMobilized"))
                {
                    var delta = HIpower * Constants.MobilizeBuff - militaryPower * Math.Pow(Constants.MilitaryScienceBuffRatio, militaryScience);
                    var increase = (delta) * Constants.MilitaryGrowthRatio;
                    if (delta > 0) increase *= Constants.MobilizeBuff/2;
                    militaryPower += increase;
                    if (militaryPower < 0) militaryPower = 0;

                    var money = (long)_dataLayer.GetProperty(country.Key, "Money");
                    money -= (long)(increase*5000);
                    _newsHandler.AddNews(country.Key, new Model.News(true) { text = "Мобилизация приносит убытки: " + String.Format("{0:0,0}", (long)((long)(increase * 50000))) });
                    _dataLayer.SetProperty(country.Key, "Money", money);
                }
                else
                {
                    var delta = HIpower - militaryPower * Math.Pow(Constants.MilitaryScienceBuffRatio, militaryScience);
                    var increase = (delta) * Constants.MilitaryGrowthRatio;
                    if (delta < 0) increase /= 6;
                    else increase /= 4;
                    militaryPower += increase;
                    if (militaryPower < 0) militaryPower = 0;
                }

                Random r = new Random();
                var mE = (int)_dataLayer.GetProperty(country.Key, "MilitaryExperience");
                mE += (int)(militaryPower * r.NextDouble());
                _dataLayer.SetProperty(country.Key, "MilitaryExperience", mE);

                _dataLayer.SetProperty(country.Key, "MilitaryPower", militaryPower);

            }
        }

        public void AddCountry(string name)
        {
            if (!_ordersBase.ContainsKey(name))
            {
                _ordersBase.Add(name, new Queue<Order>());
                _dataLayer.AddCountry(new Country(name));
            }
        }

        public void CountryReconnected(string name)
        {
            if (_stepOfGame != 0)
            Transmitter.UpdateClient(_dataLayer.GetCountry(name));
        }

        public void RemoveCountry(string name)
        {
            _ordersBase.Remove(name);
        }

        public List<DipContract> AskContracts(string myName, string targetName)
        {
            var contracts = _dataLayer.GetContractList();
            return contracts.Where((x) => x.From == targetName || x.To == targetName).ToList();
        }
    }
}
