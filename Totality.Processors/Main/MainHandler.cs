using System;
using System.Collections.Generic;
using System.Linq;
using Totality.CommonClasses;
using Totality.Handlers.Diplomatical;
using Totality.Handlers.News;
using Totality.Handlers.Nuke;
using Totality.Model;
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
            ProcessOrders();

            foreach (KeyValuePair<string, Queue<Order>> country in _ordersBase)
            {
                Country cur = (Country)_dataLayer.GetCountry(country.Key);
                cur.Mood = 100;
                if (cur.IsRepressed) cur.Mood += 10;
                _dataLayer.UpdateCountry(cur);
            }

            _nukeHandler.StartAttack();

        }

        private void _nukeHandler_AttackEnded()
        {
            fightBattles();

            updateMoney();

            // followDipContracts

            updateMilitaryPower();

            updateResources();

            updateUranus();

            updateIndustries();

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

            updateCurrencyRatios();

            updateClients();

            DipHandler.SendContractsToAll();

            _newsHandler.SendNews();
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

                var armies = battles[i].Alliances.ToList();
                for (int j = 0; j < 2; j++)
                {
                    for (int k = 0; k < armies[j].Value.Count; k++)
                    {
                        for (int p = 0; p < 2; p++)
                        {
                            if (p == j) continue;
                            countries[armies[j].Value[k]].MilitaryPower -= (powers[armies[p].Key] / armies[j].Value.Count)* 0.75;
                            if (countries[armies[j].Value[k]].MilitaryPower < 0) countries[armies[j].Value[k]].MilitaryPower = 0;

                            if (powers[armies[p].Key] > powers[armies[j].Key])
                            {
                                countries[armies[j].Value[k]].ResOil -= ((powers[armies[p].Key] - powers[armies[j].Key]) / (armies[j].Value.Count))*0.75;
                                if (countries[armies[j].Value[k]].ResOil < 0) countries[armies[j].Value[k]].ResOil = 0;

                                    countries[armies[j].Value[k]].ResSteel -= ((powers[armies[p].Key] - powers[armies[j].Key]) / (armies[j].Value.Count)) * 0.75;
                                if (countries[armies[j].Value[k]].ResSteel < 0) countries[armies[j].Value[k]].ResSteel = 0;

                                countries[armies[j].Value[k]].ResWood -= ((powers[armies[p].Key] - powers[armies[j].Key]) / (armies[j].Value.Count)) * 0.75;
                                if (countries[armies[j].Value[k]].ResWood < 0) countries[armies[j].Value[k]].ResWood = 0;

                                countries[armies[j].Value[k]].ResAgricultural -= ((powers[armies[p].Key] - powers[armies[j].Key]) / (armies[j].Value.Count)) * 0.75;
                                if (countries[armies[j].Value[k]].ResAgricultural < 0) countries[armies[j].Value[k]].ResAgricultural = 0;

                                countries[armies[j].Value[k]].PowerHeavyIndustry -= ((powers[armies[p].Key] - powers[armies[j].Key]) / (armies[j].Value.Count)) * 0.75;
                                if (countries[armies[j].Value[k]].PowerHeavyIndustry < 0) countries[armies[j].Value[k]].PowerHeavyIndustry = 0;

                                countries[armies[j].Value[k]].PowerLightIndustry -= ((powers[armies[p].Key] - powers[armies[j].Key]) / (armies[j].Value.Count)) * 0.75;
                                if (countries[armies[j].Value[k]].PowerLightIndustry < 0) countries[armies[j].Value[k]].PowerLightIndustry = 0;
                            }
                        }
                    }
                }
               
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
                    _ministeryHandlers[_currentOrdersLine[i].Ministery].ProcessOrder(_currentOrdersLine[i]);
                }
                _currentOrdersLine.Clear();
            }
        }

        private void updateResources()
        {
            foreach (KeyValuePair<string, Queue<Order>> country in _ordersBase)
            {
                foreach (string res in new List<string>{ "Steel", "Oil", "Wood", "Agricultural"} )
                {
                    var extract = (double)_dataLayer.GetProperty(country.Key, "Res" + res);
                    if ((bool)_dataLayer.GetProperty(country.Key, "IsRepressed"))
                        extract *= 0.98;
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
                    _dataLayer.SetProperty(country.Key, "Used" + res, usedRes);
                }
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
                _dataLayer.SetProperty(country.Key, "PowerLightIndustry", LIpower);

                LIpower *= Math.Pow(Constants.ScienceBuff, (int)_dataLayer.GetProperty(country.Key, "LightScienceLvl"));
                    // сюда добавить другие баффы
                    _dataLayer.SetProperty(country.Key, "FinalLightIndustry", LIpower);

                _dataLayer.SetProperty(country.Key, "UsedHIpower", 0);
                var HIpower = (double)_dataLayer.GetProperty(country.Key, "PowerHeavyIndustry");

                if ((bool)_dataLayer.GetProperty(country.Key, "IsRepressed"))
                    HIpower *= 0.98;
                _dataLayer.SetProperty(country.Key, "PowerHeavyIndustry", HIpower);

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
                cur.Mood -= cur.TaxesLvl / 10.0;
                cur.Mood *= Math.Pow(1.05, cur.InnerLvl - 1);
                if ( (cur.PowerHeavyIndustry - cur.PowerLightIndustry)/ cur.PowerLightIndustry > 0.3)
                {
                    cur.Mood *= (1 - (cur.PowerHeavyIndustry - cur.PowerLightIndustry) / (cur.PowerLightIndustry * 3));
                }

                if (cur.Mood > 100) cur.Mood = 100;
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
                            _dataLayer.GetCurrencyOnStock(anotherCountry.Key));
                    else
                    {
                        countries[country.Key].CurrencyRatios.Add
                            (anotherCountry.Key, 
                                FinancialTools.GetCurrencyRation(
                                countries[country.Key].NationalCurrencyDemand,
                                countries[anotherCountry.Key].NationalCurrencyDemand,
                                _dataLayer.GetCurrencyOnStock(country.Key),
                                _dataLayer.GetCurrencyOnStock(anotherCountry.Key)
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
                    money -= (long)(HIpower*(Constants.MobilizeBuff - 1)*(long)_dataLayer.GetProperty(country.Key, "IndustryUpgradeCost"));
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

        public void RemoveCountry(string name)
        {
            _ordersBase.Remove(name);
        }

    }
}
