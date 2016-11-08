using System;
using System.Collections.Generic;
using System.Linq;
using Totality.CommonClasses;
using Totality.Handlers.Diplomatical;
using Totality.Handlers.Nuke;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    public class MainHandler : AbstractHandler
    {
        private Dictionary<string, Queue<Order>> _ordersBase = new Dictionary<string, Queue<Order>>();
        private ITransmitter _transmitter;
        private List<IMinisteryHandler> _ministeryHandlers = new List<IMinisteryHandler>();
        private List<Order> _currentOrdersLine = new List<Order>();
        private NukeHandler _nukeHandler;
        private DiplomaticalHandler _dipHandler;

        public MainHandler(IDataLayer dataLayer, ILogger logger, NukeHandler nukeHandler) : base(dataLayer, logger)
        {
            _nukeHandler = nukeHandler;

            _ministeryHandlers.Add(new MinIndustryHandler(dataLayer, logger));
            _ministeryHandlers.Add(new MinFinanceHandler(dataLayer, logger));
            _ministeryHandlers.Add(new MinMilitaryHandler(dataLayer, nukeHandler, logger));
            _ministeryHandlers.Add(new MinForeignHandler(dataLayer, logger));
            _ministeryHandlers.Add(new MinMediaHandler(dataLayer, logger));
            _ministeryHandlers.Add(new MinInnerHandler(dataLayer, logger));
            _ministeryHandlers.Add(new MinSecurityHandler(dataLayer, logger));
            _ministeryHandlers.Add(new MinScienceHandler(dataLayer, logger));
            _ministeryHandlers.Add(new MinPremierHandler(dataLayer, logger));

            (_ministeryHandlers[(short)Mins.Security] as MinSecurityHandler).SecretOrderProcessed += SecretOrderProcessed;
        }

        private void SecretOrderProcessed(Order order)
        {
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

            _nukeHandler.StartAttack();

            // fightBattles

            // followDipContracts

            updateMilitaryPower();

            updateResources();

            updateUranus();

            updateIndustries();

            updateClients();

            _dipHandler.SendContractsToAll();

            // send News


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
                    _dataLayer.SetProperty(country.Key, "Used" + res, 0);
                    var extract = (double)_dataLayer.GetProperty(country.Key, "Res" + res);
                    extract *=  Math.Pow(Constants.ScienceBuff, (int)_dataLayer.GetProperty(country.Key, "ExtractScienceLvl" ));
                    // сюда добавить другие баффы
                    _dataLayer.SetProperty(country.Key, "Final" + res, extract);
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
                    LIpower *= Math.Pow(Constants.ScienceBuff, (int)_dataLayer.GetProperty(country.Key, "LightScienceLvl"));
                    // сюда добавить другие баффы
                    _dataLayer.SetProperty(country.Key, "FinalLightIndustry", LIpower);

                _dataLayer.SetProperty(country.Key, "UsedHIpower", 0);
                var HIpower = (double)_dataLayer.GetProperty(country.Key, "PowerHeavyIndustry");
                HIpower *= Math.Pow(Constants.ScienceBuff, (int)_dataLayer.GetProperty(country.Key, "HeavyScienceLvl"));
                // сюда добавить другие баффы
                _dataLayer.SetProperty(country.Key, "FinalHeavyIndustry", HIpower);
            }
        }

        private void updateClients()
        {
            _transmitter.UpdateClients( _dataLayer.GetCountries() );
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
        }

        public void RemoveCountry(string name)
        {
            _ordersBase.Remove(name);
        }

    }
}
