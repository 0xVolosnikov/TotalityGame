using System;
using System.Collections.Generic;
using System.Linq;
using Totality.CommonClasses;
using Totality.Handlers.Diplomatical;
using Totality.Handlers.News;
using Totality.Handlers.Nuke;
using Totality.Model;
using Totality.Model.Interfaces;

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
            // fightBattles

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

            updateClients();

            DipHandler.SendContractsToAll();

            _newsHandler.SendNews();
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
                if ( (cur.PowerHeavyIndustry - cur.PowerLightIndustry)/ cur.PowerLightIndustry > 0.3)
                {
                    cur.Mood *= (1 - (cur.PowerHeavyIndustry - cur.PowerLightIndustry) / (cur.PowerLightIndustry * 3));
                }
                else
                {
                    cur.Mood *= 1 + 0.1 + (cur.PowerLightIndustry - cur.PowerHeavyIndustry) / (cur.PowerLightIndustry * 3);
                }
                if (cur.Mood > 100) cur.Mood = 100;
                _dataLayer.UpdateCountry(cur);
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
                    if (delta > 0) increase *= Constants.MobilizeBuff;
                    militaryPower += increase;

                    var clearHIpower = (double)_dataLayer.GetProperty(country.Key, "PowerHeavyIndustry");
                    clearHIpower *= Constants.MobilizeDebuff;
                    _dataLayer.SetProperty(country.Key, "PowerHeavyIndustry", clearHIpower);

                    var clearLIpower = (double)_dataLayer.GetProperty(country.Key, "PowerLightIndustry");
                    clearHIpower *= Constants.MobilizeDebuff;
                    _dataLayer.SetProperty(country.Key, "PowerLightIndustry", clearLIpower);
                }
                else
                {
                    var delta = HIpower - militaryPower * Math.Pow(Constants.MilitaryScienceBuffRatio, militaryScience);
                    var increase = (delta) * Constants.MilitaryGrowthRatio;
                    if (delta < 0) increase /= 2;
                    militaryPower += increase;
                }

                _dataLayer.SetProperty(country.Key, "MilitaryPower", militaryPower);

            }
        }

        public void AddCountry(string name)
        {
            _ordersBase.Add(name, new Queue<Order>());
            _dataLayer.AddCountry(new Country(name));
        }

        public void RemoveCountry(string name)
        {
            _ordersBase.Remove(name);

        }

    }
}
