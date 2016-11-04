using System;
using Totality.CommonClasses;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    public class MinMilitaryHandler : AbstractHandler, IMinisteryHandler
    {
        private enum Orders { GeneralMobilization, Demobilization, IncreaseUranium, MakeNukes, MakeMissiles, NukeStrike, StartWar }

        public MinMilitaryHandler(IDataLayer dataLayer, ILogger logger) : base(dataLayer, logger)
        {
        }

        public bool ProcessOrder(Order order)
        {
            switch (order.Args[Constants.OrderIndex])
            {
                case (int)Orders.GeneralMobilization: return Mobilize(order);

                case (int)Orders.Demobilization: return Demobilize(order);

                case (int)Orders.IncreaseUranium: return IncreaseUranium(order);

                case (int)Orders.MakeNukes: return MakeNukes(order);

                case (int)Orders.MakeMissiles: return MakeMissiles(order);

                case (int)Orders.NukeStrike: return NukeStrike(order);

                case (int)Orders.StartWar: return StartWar(order);

                default: throw new ArgumentException("Order " + order + " not found in " + typeof(MinMilitaryHandler));
            }
        }


        private bool Mobilize(Order order)
        {
            _dataLayer.SetProperty(order.CountryName, "isMobilized", true);
            return true;
        }

        private bool Demobilize(Order order)
        {
            _dataLayer.SetProperty(order.CountryName, "isMobilized", false);
            return true;
        }

        private bool IncreaseUranium(Order order)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var upgradeCost = (long)_dataLayer.GetProperty(order.CountryName, "ProductionUpgradeCost");

            if (money < upgradeCost)
                return false;

            money -= upgradeCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);
            upgradeCost = (long)(Constants.UpgradeCostRate*upgradeCost);
            _dataLayer.SetProperty(order.CountryName, "ProductionUpgradeCost", upgradeCost);

            var uraniumProduction = (double)_dataLayer.GetProperty(order.CountryName, "ProdUranus");
            uraniumProduction *= Constants.ProductionUpgrade;
            _dataLayer.SetProperty(order.CountryName, "ProdUranus", uraniumProduction);
            return true;
        }

        private bool MakeNukes(Order order)
        {
            throw new NotImplementedException();
        }

        private bool MakeMissiles(Order order)
        {
            throw new NotImplementedException();
        }

        private bool NukeStrike(Order order)
        {
            throw new NotImplementedException();
        }

        private bool StartWar(Order order)
        {
            throw new NotImplementedException();
        }

    }
}
