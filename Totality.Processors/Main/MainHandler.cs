using System;
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
        private readonly List<Order> _currentOrdersLine = new List<Order>();
        private readonly List<IMinisteryHandler> _ministeryHandlers = new List<IMinisteryHandler>();
        private readonly NukeHandler _nukeHandler;
        private readonly Dictionary<string, Queue<Order>> _ordersBase = new Dictionary<string, Queue<Order>>();

        private readonly Dictionary<string, List<OrderResult>> _resultsBase = new Dictionary<string, List<OrderResult>>();

        private int _stepOfGame;
        private readonly List<Battle> battles = new List<Battle>();


        public MainHandler(NewsHandler newsHandler, IDataLayer dataLayer, ILogger logger, NukeHandler nukeHandler)
            : base(newsHandler, dataLayer, logger)
        {
            _nukeHandler = nukeHandler;

            _ministeryHandlers.Add(new MinIndustryHandler(newsHandler, dataLayer, logger));
            _ministeryHandlers.Add(new MinFinanceHandler(newsHandler, dataLayer, logger));
            _ministeryHandlers.Add(new MinMilitaryHandler(newsHandler, dataLayer, nukeHandler, logger));
            _ministeryHandlers.Add(new MinForeignHandler(newsHandler, dataLayer, logger));
            _ministeryHandlers.Add(new MinMediaHandler(newsHandler, dataLayer, logger));
            _ministeryHandlers.Add(new MinInnerHandler(newsHandler, dataLayer, logger));
            _ministeryHandlers.Add(new MinSecurityHandler(newsHandler, dataLayer, logger));
            _ministeryHandlers.Add(new MinScienceHandler(newsHandler, dataLayer, logger));
            _ministeryHandlers.Add(new MinPremierHandler(newsHandler, dataLayer, logger));

            (_ministeryHandlers[(short) Mins.Security] as MinSecurityHandler).SecretOrderProcessed +=
                SecretOrderProcessed;

            _nukeHandler.AttackEnded += _nukeHandler_AttackEnded;
        }

        public ITransmitter Transmitter { get; set; }
        public DiplomaticalHandler DipHandler { get; set; }


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
            for (var i = 0; i < orders.Count; i++)
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

            foreach (var country in _ordersBase)
            {
                var cur = _dataLayer.GetCountry(country.Key);
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

            foreach (var cou in _ordersBase)
            {
                var cur = _dataLayer.GetCountry(cou.Key);
                for (var i = 0; i < cur.MinsBlocks.Count(); i++)
                    if (cur.MinsBlocks[i] > 0)
                        cur.MinsBlocks[i]--;
                _dataLayer.UpdateCountry(cur);
            }

            updateMood();

            try
            {
                updateCurrencyRatios();
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
            }

            DipHandler.SendContractsToAll();

            _newsHandler.Countries = _ordersBase.Keys.ToList();
            _newsHandler.SendNews();
            _newsHandler.SendResults(_resultsBase);

            try
            {
                updateClients();
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
            }

            _dataLayer.Save("lastSave.json");
        }

        private void followDipContracts()
        {
            var contracts = _dataLayer.GetContractList();
            for (var i = 0; i < contracts.Count; i++)
            {
                Country country1, country2;
                var contract = contracts[i];

                if (!contract.IsFinished)
                    switch (contract.Type)
                    {
                        case DipMsg.Types.Alliance:
                            country1 = _dataLayer.GetCountry(contract.From);
                            country2 = _dataLayer.GetCountry(contract.To);
                            if ((country1.Alliance != country1.Name) || (country2.Alliance != country2.Name))
                            {
                                _newsHandler.AddNews(contract.From,
                                    new Model.News(true) {text = "Не удалось заключить союз со страной" + contract.To});
                                _newsHandler.AddNews(contract.To,
                                    new Model.News(true) {text = "Не удалось заключить союз со страной" + contract.From});
                                contract.IsFinished = true;
                                break;
                            }
                            country1.Alliance = contract.Text;
                            country1.IsBoss = true;
                            country2.Alliance = contract.Text;
                            _dataLayer.UpdateCountry(country1);
                            _dataLayer.UpdateCountry(country2);
                            contract.IsFinished = true;
                            _newsHandler.AddBroadNews(new Model.News(false)
                            {
                                text =
                                    "Страна " + country1.Name + " добавила страну " + country2.Name + " в альянс " +
                                    contract.Text + "."
                            });
                            break;
                        case DipMsg.Types.CurrencyAlliance:
                            country1 = _dataLayer.GetCountry(contract.From);
                            country2 = _dataLayer.GetCountry(contract.To);

                            if ((country1.CurrencyAlliance != country1.Name) ||
                                (country2.CurrencyAlliance != country2.Name))
                            {
                                _newsHandler.AddNews(contract.From,
                                    new Model.News(true) {text = "Не удалось заключить союз со страной" + contract.To});
                                _newsHandler.AddNews(contract.To,
                                    new Model.News(true) {text = "Не удалось заключить союз со страной" + contract.From});
                                contract.IsFinished = true;
                                _dataLayer.BreakContract(contract.Id);
                                break;
                            }

                            country2.CurrencyAlliance = country1.CurrencyAlliance;
                            _dataLayer.UpdateCountry(country2);
                            contract.IsFinished = true;

                            break;
                        case DipMsg.Types.MilitaryTraining:
                            country1 = _dataLayer.GetCountry(contract.From);
                            country2 = _dataLayer.GetCountry(contract.To);
                            country1.MilitaryPower *= 1.02;
                            country2.MilitaryPower *= 1.02;
                            _dataLayer.UpdateCountry(country1);
                            _dataLayer.UpdateCountry(country2);
                            _newsHandler.AddBroadNews(new Model.News(false)
                            {
                                text =
                                    "Страна " + country1.Name + " провела военные учения со страной " + country2.Name +
                                    "."
                            });
                            contract.IsFinished = true;
                            _dataLayer.BreakContract(contract.Id);
                            break;
                        case DipMsg.Types.Other:
                            contract.IsFinished = true;
                            _newsHandler.AddNews(contract.From,
                                new Model.News(true)
                                {
                                    text = "Мы и страна " + contract.To + " ратифицировали договор: " + contract.Text
                                });
                            _newsHandler.AddNews(contract.To,
                                new Model.News(true)
                                {
                                    text = "Мы и страна " + contract.From + " ратифицировали договор: " + contract.Text
                                });
                            break;
                        case DipMsg.Types.Peace:
                            var countries = _dataLayer.GetCountries();
                            country1 = _dataLayer.GetCountry(contract.From);
                            country2 = _dataLayer.GetCountry(contract.To);
                            _log.Trace("Заключен мир между " + country1.Name + " и " + country2.Name);

                            if ((!country1.IsBoss && (country1.Alliance != country1.Name)) ||
                                (!country2.IsBoss && (country2.Alliance != country2.Name)))
                            {
                                _newsHandler.AddNews(contract.From,
                                    new Model.News(true)
                                    {
                                        text =
                                            "Мы не имеем права заключать мир со страной " + contract.To +
                                            ", полномочия есть только у глав альянсов."
                                    });
                                _newsHandler.AddNews(contract.To,
                                    new Model.News(true)
                                    {
                                        text =
                                            "Мы не имеем права заключать мир со страной " + contract.From +
                                            ", полномочия есть только у глав альянсов."
                                    });
                                contract.IsFinished = true;
                                _dataLayer.BreakContract(contract.Id);
                                break;
                            }

                            var alliance1 = countries.Values.Where(x => x.Alliance == country1.Alliance).ToList();
                            var alliance2 = countries.Values.Where(x => x.Alliance == country2.Alliance).ToList();

                            for (var al1 = 0; al1 < alliance1.Count; al1++)
                                for (var c = 0; c < alliance1[al1].WarList.Count; c++)
                                    if (alliance2.Any(x => x.Name == alliance1[al1].WarList[c]))
                                        alliance1[al1].WarList.Remove(alliance1[al1].WarList[c]);

                            for (var al2 = 0; al2 < alliance2.Count; al2++)
                                for (var c = 0; c < alliance2[al2].WarList.Count; c++)
                                    if (alliance1.Any(x => x.Name == alliance2[al2].WarList[c]))
                                        alliance2[al2].WarList.Remove(alliance2[al2].WarList[c]);

                            battles.RemoveAll(
                                x =>
                                    x.Alliances.Any(t => t.Key == country1.Alliance) &&
                                    x.Alliances.Any(p => p.Key == country2.Alliance));

                            _newsHandler.AddBroadNews(new Model.News(false)
                            {
                                text = "Альянсы " + country1.Alliance + " и " + country2.Alliance + " заключили мир."
                            });
                            contract.IsFinished = true;
                            _dataLayer.BreakContract(contract.Id);
                            break;

                        case DipMsg.Types.Trade:
                            if (contract.Time == 0)
                            {
                                contract.IsFinished = true;
                                _dataLayer.BreakContract(contract.Id);
                                break;
                            }
                            contract.Time--;

                            country1 = _dataLayer.GetCountry(contract.From);
                            country2 = _dataLayer.GetCountry(contract.To);
                            var res = (double) _dataLayer.GetProperty(country1.Name, "Final" + contract.Res) -
                                      (double) _dataLayer.GetProperty(country1.Name, "Used" + contract.Res);
                            var needMoney = FinancialTools.GetExchangeCost(contract.Price,
                                country2.NationalCurrencyDemand, country1.NationalCurrencyDemand,
                                _dataLayer.GetCurrencyOnStock(country2.Name),
                                _dataLayer.GetCurrencyOnStock(country1.Name),
                                country2.FinalHeavyIndustry + country2.FinalLightIndustry,
                                country1.FinalHeavyIndustry + country1.FinalLightIndustry);

                            if (needMoney > country2.Money)
                            {
                                _newsHandler.AddNews(country1.Name,
                                    new Model.News(true)
                                    {
                                        text =
                                            "Страна " + country2.Name + " не смогла заплатить по торговому контракту: " +
                                            contract.Description
                                    });
                                _newsHandler.AddNews(country2.Name,
                                    new Model.News(true)
                                    {
                                        text =
                                            "Мы не смогли заплатить стране " + country2.Name +
                                            " по торговому контракту: " + contract.Description
                                    });
                                break;
                            }

                            if (res >= contract.Count)
                            {
                                _dataLayer.SetProperty(country1.Name, "Used" + contract.Res,
                                    (double) _dataLayer.GetProperty(country1.Name, "Used" + contract.Res) +
                                    contract.Count);
                                _dataLayer.SetProperty(country2.Name, "Final" + contract.Res,
                                    (double) _dataLayer.GetProperty(country2.Name, "Final" + contract.Res) +
                                    contract.Count);

                                _dataLayer.SetProperty(country1.Name, "Money",
                                    (long) _dataLayer.GetProperty(country1.Name, "Money") + contract.Price);
                                _dataLayer.SetProperty(country2.Name, "Money",
                                    (long) _dataLayer.GetProperty(country2.Name, "Money") - needMoney);

                                _dataLayer.SetCurrencyOnStock(country1.Name,
                                    _dataLayer.GetCurrencyOnStock(country1.Name) - contract.Price);
                                _dataLayer.SetCurrencyOnStock(country2.Name,
                                    _dataLayer.GetCurrencyOnStock(country2.Name) + needMoney);

                                _newsHandler.AddNews(country1.Name,
                                    new Model.News(true)
                                    {
                                        text =
                                            "Страна " + country2.Name +
                                            " оплатила очередную партию по торговому контракту: " + contract.Description +
                                            ", " + string.Format("{0:0,0}", contract.Price)
                                    });
                                _newsHandler.AddNews(country2.Name,
                                    new Model.News(true)
                                    {
                                        text =
                                            "Мы получили очередную партию товара из страны " + country1.Name +
                                            " по торговому контракту: " + contract.Description + ", в количестве " +
                                            contract.Count
                                    });
                            }
                            else
                            {
                                _newsHandler.AddNews(country2.Name,
                                    new Model.News(true)
                                    {
                                        text =
                                            "Страна " + country1.Name +
                                            " не предоставить товар по торговому контракту: " + contract.Description
                                    });
                                _newsHandler.AddNews(country1.Name,
                                    new Model.News(true)
                                    {
                                        text =
                                            "Мы не смогли предоставить товар стране " + country2.Name +
                                            " по торговому контракту: " + contract.Description
                                    });
                            }

                            break;
                        case DipMsg.Types.Transfer:
                            country1 = _dataLayer.GetCountry(contract.From);
                            country2 = _dataLayer.GetCountry(contract.To);
                            if (country1.Money >= contract.Count)
                            {
                                country1.Money -= contract.Count;
                                country2.CurrencyAccounts[country1.Name] += contract.Count;
                                _dataLayer.UpdateCountry(country1);
                                _dataLayer.UpdateCountry(country2);
                                _newsHandler.AddNews(country1.Name,
                                    new Model.News(true)
                                    {
                                        text =
                                            "В страну " + country2.Name + " совершен перевод: " +
                                            string.Format("{0:0,0}", contract.Count)
                                    });
                                _newsHandler.AddNews(country2.Name,
                                    new Model.News(true)
                                    {
                                        text =
                                            "Страна " + country1.Name + " совершила нам перевод в своей валюте: " +
                                            string.Format("{0:0,0}", contract.Count)
                                    });
                            }
                            contract.IsFinished = true;
                            _dataLayer.BreakContract(contract.Id);
                            break;
                    }
            }
        }

        public Dictionary<string, long> GetCurrencyDemands() // OPTIMISE
        {
            var demands = new Dictionary<string, long>();
            var countries = _dataLayer.GetCountries();

            foreach (var c in countries.Values)
                demands.Add(c.Name, c.NationalCurrencyDemand);

            return demands;
        }

        public Dictionary<string, double> GetSumIndPowers()
        {
            var SumIndPowers = new Dictionary<string, double>();
            var countries = _dataLayer.GetCountries();

            foreach (var c in countries.Values)
                SumIndPowers.Add(c.Name, c.FinalHeavyIndustry + c.FinalLightIndustry);

            return SumIndPowers;
        }

        public Dictionary<string, long> GetCurrencyStock()
        {
            return _dataLayer.GetStock();
        }

        private void updateMoney()
        {
            var countries = _dataLayer.GetCountries();
            foreach (var country in _ordersBase)
            {
                long profit;
                var currency = _dataLayer.GetCurrencyOnStock(country.Key);

                if (countries[country.Key].InflationCoeff > 1)
                {
                   profit =
                    (long)
                    (countries[country.Key].FinalLightIndustry*Constants.LightPowerProfit*
                     (countries[country.Key].TaxesLvl/15.0)*Math.Sqrt(countries[country.Key].InflationCoeff));

                }
                else
                {
                    profit =
                    (long)
                    (countries[country.Key].FinalLightIndustry * Constants.LightPowerProfit *
                     (countries[country.Key].TaxesLvl / 15.0) * Math.Pow(countries[country.Key].InflationCoeff, 1.2));

                }

                if (currency >= profit / 4)
                {
                    countries[country.Key].Money += profit;
                    _dataLayer.SetCurrencyOnStock(country.Key, currency - profit / 4);
                }
                else
                {
                    countries[country.Key].Money += currency - 100000;
                    _dataLayer.SetCurrencyOnStock(country.Key, 100000);
                }



                _dataLayer.UpdateCountry(countries[country.Key]);
            }
        }


        private void fightBattles()
        {
            var countries = _dataLayer.GetCountries();
            foreach (var country in _ordersBase)
                for (var i = 0; i < countries[country.Key].WarList.Count; i++)
                {
                    var we = countries[country.Key];
                    var enemy = countries[countries[country.Key].WarList[i]];
                    if (
                        !battles.Any(
                            x => x.Alliances.ContainsKey(we.Alliance) && x.Alliances.ContainsKey(enemy.Alliance)))
                    {
                        var battle = new Battle(we.Alliance, enemy.Alliance);

                        var allies = countries.Values.Where(x => x.Alliance.Equals(we.Alliance)).ToList();
                        foreach (var al in allies)
                            battle.Alliances[we.Alliance].Add(al.Name);

                        var enemies = countries.Values.Where(x => x.Alliance.Equals(enemy.Alliance)).ToList();
                        foreach (var en in enemies)
                            battle.Alliances[enemy.Alliance].Add(en.Name);

                        battles.Add(battle);
                    }
                    else
                    {
                        var curBattle =
                            battles.First(
                                x => x.Alliances.ContainsKey(we.Alliance) && x.Alliances.ContainsKey(enemy.Alliance));
                        if (!curBattle.Alliances[we.Alliance].Contains(we.Name))
                            curBattle.Alliances[we.Alliance].Add(we.Name);
                        if (!curBattle.Alliances[enemy.Alliance].Contains(enemy.Name))
                            curBattle.Alliances[enemy.Alliance].Add(enemy.Name);
                    }
                }

            for (var i = 0; i < battles.Count; i++)
            {
                battles[i].Alliances.First()
                    .Value.RemoveAll(x => _dataLayer.GetCountry(x).Alliance != battles[i].Alliances.First().Key);
                battles[i].Alliances.Last()
                    .Value.RemoveAll(x => _dataLayer.GetCountry(x).Alliance != battles[i].Alliances.Last().Key);

                if (battles[i].step == 0)
                {
                    battles[i].step++;
                    continue;
                }

                var powers = new Dictionary<string, double>();
                foreach (var al in battles[i].Alliances)
                {
                    powers.Add(al.Key, 0);
                    foreach (var country in al.Value)
                        powers[al.Key] += countries[country].FinalMilitaryPower;
                }

                try
                {
                    var armies = battles[i].Alliances.ToList();
                    for (var j = 0; j < 2; j++)
                        for (var k = 0; k < armies[j].Value.Count; k++)
                            for (var p = 0; p < 2; p++)
                                try
                                {
                                    if (p == j) continue;
                                    countries[armies[j].Value[k]].MilitaryPower -= powers[armies[p].Key]/
                                                                                   armies[j].Value.Count*0.25;
                                    // ERROR
                                    if (countries[armies[j].Value[k]].MilitaryPower < 0)
                                        countries[armies[j].Value[k]].MilitaryPower = 0;

                                    if (powers[armies[p].Key] > powers[armies[j].Key])
                                    {
                                        if ((powers[armies[p].Key] - powers[armies[j].Key])/
                                            powers[armies[j].Key] >
                                            0.5)
                                            _newsHandler.AddBroadNews(new Model.News(true)
                                            {
                                                text =
                                                    "При подавляющем превосходстве армия " + armies[p].Key +
                                                    " нанесла сокрушительное поражение армии " + armies[j].Key + "."
                                            });
                                        else if ((powers[armies[p].Key] - powers[armies[j].Key])/
                                                 powers[armies[j].Key] >
                                                 0.2)
                                            _newsHandler.AddBroadNews(new Model.News(true)
                                            {
                                                text =
                                                    "Пользуясь превосходством, армия " + armies[p].Key +
                                                    " нанесла поражение армии " + armies[j].Key + "."
                                            });
                                        else if ((powers[armies[p].Key] - powers[armies[j].Key])/
                                                 powers[armies[j].Key] >
                                                 0)
                                            _newsHandler.AddBroadNews(new Model.News(true)
                                            {
                                                text =
                                                    "Пользуясь незначительным превосходством, армия " + armies[p].Key +
                                                    " ослабила армию " + armies[j].Key + "."
                                            });


                                        countries[armies[j].Value[k]].ResOil -= (powers[armies[p].Key] -
                                                                                 powers[armies[j].Key])/
                                                                                armies[j].Value.Count*0.25;
                                        if (countries[armies[j].Value[k]].ResOil < 7)
                                            countries[armies[j].Value[k]].ResOil = 7;

                                        countries[armies[j].Value[k]].ResSteel -= (powers[armies[p].Key] -
                                                                                   powers[armies[j].Key])/
                                                                                  armies[j].Value.Count*0.25;
                                        if (countries[armies[j].Value[k]].ResSteel < 7)
                                            countries[armies[j].Value[k]].ResSteel = 7;

                                        countries[armies[j].Value[k]].ResWood -= (powers[armies[p].Key] -
                                                                                  powers[armies[j].Key])/
                                                                                 armies[j].Value.Count*0.25;
                                        if (countries[armies[j].Value[k]].ResWood < 7)
                                            countries[armies[j].Value[k]].ResWood = 7;

                                        countries[armies[j].Value[k]].ResAgricultural -= (powers[armies[p].Key] -
                                                                                          powers[armies[j].Key])/
                                                                                         armies[j].Value.Count*0.25;
                                        if (countries[armies[j].Value[k]].ResAgricultural < 7)
                                            countries[armies[j].Value[k]].ResAgricultural = 7;

                                        countries[armies[j].Value[k]].PowerHeavyIndustry -= (powers[armies[p].Key] -
                                                                                             powers[armies[j].Key])/
                                                                                            armies[j].Value.Count*
                                                                                            0.10;
                                        if (countries[armies[j].Value[k]].PowerHeavyIndustry < 5)
                                            countries[armies[j].Value[k]].PowerHeavyIndustry = 5;

                                        countries[armies[j].Value[k]].PowerLightIndustry -= (powers[armies[p].Key] -
                                                                                             powers[armies[j].Key])/
                                                                                            armies[j].Value.Count*
                                                                                            0.10;
                                        if (countries[armies[j].Value[k]].PowerLightIndustry < 5)
                                            countries[armies[j].Value[k]].PowerLightIndustry = 5;
                                    }
                                    else
                                        _newsHandler.AddBroadNews(new Model.News(true)
                                        {
                                            text =
                                                "Атака армии " + armies[p].Key + " позиций " + armies[j].Key +
                                                " оказалась неудачной."
                                        });
                                }
                                catch (Exception e)
                                {
                                    _log.Error("SMTHNG WITH ARMIES: " + e.Message);
                                }
                }
                catch (Exception e)
                {
                    _log.Error("SMTHING WITH ARMIES 2: " + e.Message);
                }
            }

            var cs = countries.Values.ToList();
            for (var c = 0; c < countries.Values.Count; c++)
                _dataLayer.UpdateCountry(cs[c]);
        }

        public void ProcessOrders()
        {
            _resultsBase.Clear();

            while (_ordersBase.Any(x => x.Value.Any()))
            {
                foreach (var qu in _ordersBase.Where(x => x.Value.Any()))
                    _currentOrdersLine.Add(qu.Value.Dequeue());

                //_currentOrdersLine.OrderBy(x => ); случайно отсортировать

                for (var i = 0; i < _currentOrdersLine.Count; i++)
                {
                    OrderResult result;
                    if (_currentOrdersLine[i].Ministery.Equals((short) Mins.Secret))
                        result = _ministeryHandlers[(short) Mins.Security].ProcessOrder(_currentOrdersLine[i]);
                    else
                        result = _ministeryHandlers[_currentOrdersLine[i].Ministery].ProcessOrder(_currentOrdersLine[i]);

                    if (!_resultsBase.ContainsKey(result.CountryName))
                       _resultsBase.Add(result.CountryName, new List<OrderResult>());
                    _resultsBase[result.CountryName].Add(result);
                }
                _currentOrdersLine.Clear();
            }
        }

        private void updateResources()
        {
            foreach (var country in _ordersBase)
            {
                var exp0 = (int) _dataLayer.GetProperty(country.Key, "ExtractExperience");
                var exp = 0;

                if ((bool) _dataLayer.GetProperty(country.Key, "IsRepressed"))
                    _newsHandler.AddNews(country.Key,
                        new Model.News(true) {text = "Репрессии наносят урон промышленности "});
                if ((bool) _dataLayer.GetProperty(country.Key, "IsRiot"))
                    _newsHandler.AddNews(country.Key, new Model.News(true) {text = "Бунт наносит урон промышленности! "});

                foreach (var res in new List<string> {"Steel", "Oil", "Wood", "Agricultural"})
                {
                    var extract = (double) _dataLayer.GetProperty(country.Key, "Res" + res);
                    if ((bool) _dataLayer.GetProperty(country.Key, "IsRepressed"))
                        extract *= 0.98;
                    if ((bool) _dataLayer.GetProperty(country.Key, "IsRiot"))
                        extract *= 0.92;
                    _dataLayer.SetProperty(country.Key, "Res" + res, extract);

                    extract *= Math.Pow(Constants.ScienceBuff,
                        (int) _dataLayer.GetProperty(country.Key, "ExtractScienceLvl"));
                    // сюда добавить другие баффы
                    _dataLayer.SetProperty(country.Key, "Final" + res, extract);
                    double usedRes = 0;
                    switch (res)
                    {
                        case "Steel":
                            usedRes = (double) _dataLayer.GetProperty(country.Key, "PowerHeavyIndustry")*
                                      Constants.IndustrySteelCoeff;

                            break;
                        case "Oil":
                            usedRes = (double) _dataLayer.GetProperty(country.Key, "PowerHeavyIndustry")*
                                      Constants.IndustryOilCoeff;
                            break;
                        case "Wood":
                            usedRes = (double) _dataLayer.GetProperty(country.Key, "PowerLightIndustry")*
                                      Constants.IndustryWoodCoeff;
                            break;
                        case "Agricultural":
                            usedRes = (double) _dataLayer.GetProperty(country.Key, "PowerLightIndustry")*
                                      Constants.IndustryAgroCoeff;
                            break;
                    }
                    exp += (int) usedRes;
                    _dataLayer.SetProperty(country.Key, "Used" + res, usedRes);
                }

                var r = new Random();
                exp /= 4;
                exp = (int) (r.NextDouble()*exp) + exp0;
                _dataLayer.SetProperty(country.Key, "ExtractExperience", exp);
            }
        }

        private void updateUranus()
        {
            foreach (var country in _ordersBase)
            {
                var uranus = (double) _dataLayer.GetProperty(country.Key, "ResUranus");
                var uranusProduction = (double) _dataLayer.GetProperty(country.Key, "ProdUranus");
                uranus += uranusProduction;
                _dataLayer.SetProperty(country.Key, "ResUranus", uranus);
            }
        }

        private void updateIndustries()
        {
            foreach (var country in _ordersBase)
            {
                _dataLayer.SetProperty(country.Key, "UsedLIpower", 0);
                var LIpower = (double) _dataLayer.GetProperty(country.Key, "PowerLightIndustry");

                if ((bool) _dataLayer.GetProperty(country.Key, "IsRepressed"))
                    LIpower *= 0.98;
                if ((bool) _dataLayer.GetProperty(country.Key, "IsRiot"))
                    LIpower *= 0.92;
                _dataLayer.SetProperty(country.Key, "PowerLightIndustry", LIpower);

                var r = new Random();
                var lE = (int) _dataLayer.GetProperty(country.Key, "LightExperience");
                lE += (int) (LIpower*r.NextDouble());
                _dataLayer.SetProperty(country.Key, "LightExperience", lE);

                _dataLayer.SetProperty(country.Key, "UsedHIpower", 0);
                var HIpower = (double) _dataLayer.GetProperty(country.Key, "PowerHeavyIndustry");

                if ((bool) _dataLayer.GetProperty(country.Key, "IsRepressed"))
                    HIpower *= 0.98;
                if ((bool) _dataLayer.GetProperty(country.Key, "IsRiot"))
                    HIpower *= 0.92;
                _dataLayer.SetProperty(country.Key, "PowerHeavyIndustry", HIpower);

                var hE = (int) _dataLayer.GetProperty(country.Key, "HeavyExperience");
                hE += (int) (HIpower*r.NextDouble());
                _dataLayer.SetProperty(country.Key, "HeavyExperience", hE);

                var upgradeCost = (long) (Constants.IndustryUpgradeCostRate*(LIpower + HIpower));
                _dataLayer.SetProperty(country.Key, "IndustryUpgradeCost", upgradeCost);

                LIpower *= Math.Pow(Constants.ScienceBuff, (int) _dataLayer.GetProperty(country.Key, "LightScienceLvl"));
                // сюда добавить другие баффы
                _dataLayer.SetProperty(country.Key, "FinalLightIndustry", LIpower);

                HIpower *= Math.Pow(Constants.ScienceBuff, (int) _dataLayer.GetProperty(country.Key, "HeavyScienceLvl"));
                // сюда добавить другие баффы
                _dataLayer.SetProperty(country.Key, "FinalHeavyIndustry", HIpower);
            }
        }

        private void updateMood()
        {
            foreach (var country in _ordersBase)
            {
                var cur = _dataLayer.GetCountry(country.Key);
                if (cur.InflationCoeff > 1)
                    cur.Mood -= cur.InflationCoeff*10 - 10;
                else if (cur.InflationCoeff > 0.1)
                    cur.Mood += 5/cur.InflationCoeff;

                foreach (var prop in cur.MassMedia)
                {
                    var cur2 = _dataLayer.GetCountry(prop.Key);
                    if (prop.Value == 1)
                    {
                        cur2.Mood -= 7;
                        cur.Money -= Constants.PropagandaCost;
                        _newsHandler.AddNews(cur.Name,
                            new Model.News(false) {text = "Наши СМИ повышают волнения в стране " + cur2.Name});
                        _newsHandler.AddNews(cur2.Name,
                            new Model.News(false)
                            {
                                text = "Подлые газетчики из страны " + cur.Name + " повышают волнения в нашей стране!"
                            });
                    }
                    else if (prop.Value == 2)
                    {
                        cur2.Mood += 7;
                        cur2.Money -= Constants.PropagandaCost;
                        _newsHandler.AddNews(cur.Name,
                            new Model.News(false)
                            {
                                text = "Наши СМИ повышают настроение населения в стране " + cur2.Name
                            });
                        _newsHandler.AddNews(cur2.Name,
                            new Model.News(false)
                            {
                                text =
                                    "Братские СМИ из страны " + cur.Name +
                                    " улучшают настроение населения в нашей стране."
                            });
                    }
                    _dataLayer.UpdateCountry(cur2);
                }
                _dataLayer.UpdateCountry(cur);
            }

            foreach (var country in _ordersBase)
            {
                var cur = _dataLayer.GetCountry(country.Key);
                cur.Mood -= (cur.TaxesLvl - 15);
                cur.Mood *= Math.Pow(1.02, cur.InnerLvl - 1);
                if ((cur.PowerHeavyIndustry - cur.PowerLightIndustry)/cur.PowerLightIndustry > 0.3)
                    cur.Mood *= 1 - (cur.PowerHeavyIndustry - cur.PowerLightIndustry)/(cur.PowerLightIndustry*3);

                if (cur.Mood > 100) cur.Mood = 100;

                if ((cur.Mood < 50) && !cur.IsRiot)
                {
                    var chance = 50 - cur.Mood;
                    var r = new Random();
                    var result = r.Next(1, 100);
                    if (result <= chance)
                    {
                        cur.IsRiot = true;
                        _newsHandler.AddBroadNews(new Model.News(false)
                        {
                            text = "В стране " + cur.Name + " бунтует население!"
                        });
                    }
                }
                _dataLayer.UpdateCountry(cur);
            }
        }

        private void updateCurrencyRatios()
        {
            var countries = _dataLayer.GetCountries();
            foreach (var country in _ordersBase)
            {
                countries[country.Key].InflationCoeff = (double)_dataLayer.GetCurrencyOnStock(country.Key)/
                                                       (
                                                        (countries[country.Key].FinalLightIndustry +
                                                         countries[country.Key].FinalHeavyIndustry +
                                                         0.20*
                                                         (countries[country.Key].FinalAgricultural +
                                                          countries[country.Key].FinalOil +
                                                          countries[country.Key].FinalSteel +
                                                          countries[country.Key].FinalWood) + 2) * 333333);
                if (countries[country.Key].InflationCoeff > 10)
                {
                    _newsHandler.AddNews(country.Key,
                       new Model.News(true)
                        {
                               text = "В результате глубокого кризиса в нашей стране объявлен дефолт!"
                        });

                    countries[country.Key].Money = 1000000;
                    _dataLayer.SetCurrencyOnStock(country.Key, (long)((countries[country.Key].FinalLightIndustry +
                                                         countries[country.Key].FinalHeavyIndustry +
                                                         0.20 *
                                                         (countries[country.Key].FinalAgricultural +
                                                          countries[country.Key].FinalOil +
                                                          countries[country.Key].FinalSteel +
                                                          countries[country.Key].FinalWood) + 5) * 333333));
                }

                foreach (var anotherCountry in _ordersBase)
                {
                    if (country.Key.Equals(anotherCountry.Key)) continue;

                    if (countries[country.Key].CurrencyRatios.ContainsKey(anotherCountry.Key))
                        countries[country.Key].CurrencyRatios[anotherCountry.Key] = FinancialTools.GetCurrencyRation(
                            countries[country.Key].NationalCurrencyDemand,
                            countries[anotherCountry.Key].NationalCurrencyDemand,
                            _dataLayer.GetCurrencyOnStock(country.Key),
                            _dataLayer.GetCurrencyOnStock(anotherCountry.Key),
                            countries[country.Key].FinalHeavyIndustry + countries[country.Key].FinalLightIndustry,
                            countries[anotherCountry.Key].FinalHeavyIndustry +
                            countries[anotherCountry.Key].FinalLightIndustry);
                    else
                        countries[country.Key].CurrencyRatios.Add
                        (anotherCountry.Key,
                            FinancialTools.GetCurrencyRation(
                                countries[country.Key].NationalCurrencyDemand,
                                countries[anotherCountry.Key].NationalCurrencyDemand,
                                _dataLayer.GetCurrencyOnStock(country.Key),
                                _dataLayer.GetCurrencyOnStock(anotherCountry.Key),
                                countries[country.Key].FinalHeavyIndustry + countries[country.Key].FinalLightIndustry,
                                countries[anotherCountry.Key].FinalHeavyIndustry +
                                countries[anotherCountry.Key].FinalLightIndustry
                            )
                        );

                    if (!countries[country.Key].CurrencyAccounts.ContainsKey(anotherCountry.Key))
                        countries[country.Key].CurrencyAccounts.Add(anotherCountry.Key, 0);
                }
                _dataLayer.UpdateCountry(countries[country.Key]);
            }
        }


        private void updateClients()
        {
            Transmitter.UpdateClients(_dataLayer.GetCountries());
        }

        private void updateMilitaryPower()
        {
            foreach (var country in _ordersBase)
            {
                var HIpower = (double) _dataLayer.GetProperty(country.Key, "FinalHeavyIndustry");
                var usedHIpower = (double) _dataLayer.GetProperty(country.Key, "UsedHIpower");
                var militaryPower = (double) _dataLayer.GetProperty(country.Key, "MilitaryPower");
                var finalMilitaryPower = (double)_dataLayer.GetProperty(country.Key, "FinalMilitaryPower");
                var militaryScience = (int) _dataLayer.GetProperty(country.Key, "MilitaryScienceLvl");

                if ((bool) _dataLayer.GetProperty(country.Key, "IsMobilized"))
                {
                    var delta = HIpower*Constants.MobilizeBuff -
                                finalMilitaryPower*Math.Pow(Constants.MilitaryScienceBuffRatio, militaryScience);
                    var increase = delta*Constants.MilitaryGrowthRatio;
                    if (delta > 0) increase *= Constants.MobilizeBuff/2;
                    finalMilitaryPower += increase;
                    if (militaryPower < 0) finalMilitaryPower = 0;

                    var money = (long) _dataLayer.GetProperty(country.Key, "Money");
                    money -= (long) ((finalMilitaryPower-militaryPower)*5000);
                    _newsHandler.AddNews(country.Key,
                        new Model.News(true)
                        {
                            text = "Мобилизация приносит убытки: " + string.Format("{0:0,0}", (long)((finalMilitaryPower - militaryPower) * 5000))
                        });
                    _dataLayer.SetProperty(country.Key, "Money", money);
                }
                else
                {
                    var delta = HIpower - finalMilitaryPower*Math.Pow(Constants.MilitaryScienceBuffRatio, militaryScience);
                    var increase = delta*Constants.MilitaryGrowthRatio;
                    if (delta < 0) increase /= 6;
                    else increase /= 4;
                    finalMilitaryPower += increase;
                    if (finalMilitaryPower < 0) finalMilitaryPower = 0;
                }

                var r = new Random();
                var mE = (int) _dataLayer.GetProperty(country.Key, "MilitaryExperience");
                mE += (int) (militaryPower*r.NextDouble());
                _dataLayer.SetProperty(country.Key, "MilitaryExperience", mE);

                _dataLayer.SetProperty(country.Key, "FinalMilitaryPower", finalMilitaryPower);
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
            return contracts.Where(x => (x.From == targetName) || (x.To == targetName)).ToList();
        }
    }
}