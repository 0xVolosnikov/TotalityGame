using System;
using Totality.CommonClasses;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    public class MinIndustryHandler : AbstractHandler, IMinisteryHandler
    {
        private enum Orders { ImproveHeavy, ImproveLight, IncreaseSteel, IncreaseOil, IncreaseWood, IncreaseAgricultural }

        public MinIndustryHandler(IDataLayer dataLayer, ILogger logger) : base(dataLayer, logger)
        {
        }

        public bool ProcessOrder(Order order)
        {
            switch (order.OrderNum)
            {
                case (int)Orders.ImproveHeavy: return ImproveIndustry(order, "Heavy");

                case (int)Orders.ImproveLight: return ImproveIndustry(order, "Light");

                case (int)Orders.IncreaseSteel: return IncreaseRes(order, "Steel");

                case (int)Orders.IncreaseOil: return IncreaseRes(order, "Oil");

                case (int)Orders.IncreaseWood: return IncreaseRes(order, "Wood");

                case (int)Orders.IncreaseAgricultural: return IncreaseRes(order, "Agricultural");

                default: throw new ArgumentException("Order " + order + " not found in " + typeof(MinIndustryHandler));
            }
        }

        private bool ImproveIndustry(Order order, string industry)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var upgradeCost = (long)_dataLayer.GetProperty(order.CountryName, "IndustryUpgradeCost");

            if (money < upgradeCost)
                return false;

            var industryPower = (double)_dataLayer.GetProperty(order.CountryName, "Power" + industry + "Industry");

            double res1, res2;
            if (industry == "Heavy")
            {
                res1 = (double)_dataLayer.GetProperty(order.CountryName, "FinalSteel");
                res2 = (double)_dataLayer.GetProperty(order.CountryName, "FinalOil");
            }
            else
            {
                res1 = (double)_dataLayer.GetProperty(order.CountryName, "FinalWood");
                res2 = (double)_dataLayer.GetProperty(order.CountryName, "FinalAgricultural");
            }

            if (res1 < industryPower * Constants.IndustrySteelCoeff || res2 < industryPower * Constants.IndustryOilCoeff)
                return false;

            money -= upgradeCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);
            upgradeCost = (long)(Constants.IndustryUpgradeCostRate * upgradeCost);
            _dataLayer.SetProperty(order.CountryName, "IndustryUpgradeCost", upgradeCost);

            industryPower *= Constants.IndustryUpgrade;
            _dataLayer.SetProperty(order.CountryName, "Power" + industry + "Industry", industryPower);

            return true;
        }

        private bool IncreaseRes(Order order, string res)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var upgradeCost = (long)_dataLayer.GetProperty(order.CountryName, "ProductionUpgradeCost");

            if (money < upgradeCost)
                return false;

            money -= upgradeCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);
            upgradeCost = (long)(Constants.UpgradeCostRate * upgradeCost);
            _dataLayer.SetProperty(order.CountryName, "ProductionUpgradeCost", upgradeCost);

            var resProduction = (double)_dataLayer.GetProperty(order.CountryName, "Res" + res);
            resProduction += Constants.ProductionUpgrade;
            _dataLayer.SetProperty(order.CountryName, "Res" + res, resProduction);
            return true;
        }
    }
}
