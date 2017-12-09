using System;
using Totality.CommonClasses;
using Totality.Handlers.News;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    public class MinIndustryHandler : AbstractHandler, IMinisteryHandler
    {
        private enum Orders { ImproveHeavy, ImproveLight, IncreaseSteel, IncreaseOil, IncreaseWood, IncreaseAgricultural }

        public MinIndustryHandler(NewsHandler newsHandler, IDataLayer dataLayer, ILogger logger) : base(newsHandler, dataLayer, logger)
        {
        }

        public OrderResult ProcessOrder(Order order)
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

        private OrderResult ImproveIndustry(Order order, string industry)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var upgradeCost = (long)_dataLayer.GetProperty(order.CountryName, "IndustryUpgradeCost");
            var InflationCoeff = (double)_dataLayer.GetProperty(order.CountryName, "InflationCoeff");
            upgradeCost = (long)(upgradeCost *InflationCoeff);
            if (money < upgradeCost)
            {
                if (industry == "Heavy")
                {
                    _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Государство не смогло профинансировать развитие мощи Тяжелой Промышленности!" });
                    return new OrderResult(order.CountryName, "Улучшение Тяжелой Промышленности", false, upgradeCost);
                }
                else
                {
                    _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Государство не смогло профинансировать развитие мощи Легкой Промышленности!" });
                    return new OrderResult(order.CountryName, "Улучшение Легкой Промышленности", false, upgradeCost);
                }
            }

            var industryPower = (double)_dataLayer.GetProperty(order.CountryName, "Power" + industry + "Industry");

            double res1, res2;
            if (industry == "Heavy")
            {
                res1 = (double)_dataLayer.GetProperty(order.CountryName, "FinalSteel");
                res2 = (double)_dataLayer.GetProperty(order.CountryName, "FinalOil");
                if (res1 < industryPower + Constants.IndustryUpgrade*Constants.IndustrySteelCoeff ||
                    res2 < industryPower + Constants.IndustryUpgrade*Constants.IndustryOilCoeff)
                {
                    _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Не получилось повысить мощь Тяжелой Промышленности из-за дефицита ресурсов!" });
                    return new OrderResult(order.CountryName, "Улучшение Тяжелой Промышленности", false, upgradeCost);
                }

                _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Повышена мощь Тяжелой Промышленности!" });
            }
            else
            {
                res1 = (double)_dataLayer.GetProperty(order.CountryName, "FinalWood");
                res2 = (double)_dataLayer.GetProperty(order.CountryName, "FinalAgricultural");
                if (res1 < industryPower + Constants.IndustryUpgrade*Constants.IndustryWoodCoeff ||
                    res2 < industryPower + Constants.IndustryUpgrade*Constants.IndustryAgroCoeff)
                {
                    _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Не получилось повысить мощь Легкой Промышленности из-за дефицита ресурсов!" });
                    return new OrderResult(order.CountryName, "Улучшение Легкой Промышленности", false, upgradeCost);
                }

                _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Повышена мощь Легкой Промышленности!" });
            }


            money -= upgradeCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);

            industryPower += Constants.IndustryUpgrade;

            //var demand = (long)_dataLayer.GetProperty(order.CountryName, "NationalCurrencyDemand");
            //demand = (long)(demand * 1.01);
            //_dataLayer.SetProperty(order.CountryName, "NationalCurrencyDemand", demand);
                
            _dataLayer.SetProperty(order.CountryName, "Power" + industry + "Industry", industryPower);


            var LIpower = (double)_dataLayer.GetProperty(order.CountryName, "PowerLightIndustry");
            var HIpower = (double)_dataLayer.GetProperty(order.CountryName, "PowerHeavyIndustry");
            var newUpgradeCost = (long)(Constants.IndustryUpgradeCostRate * (LIpower + HIpower));
            _dataLayer.SetProperty(order.CountryName, "IndustryUpgradeCost", newUpgradeCost);

            if (industry == "Heavy")
            {
                return new OrderResult(order.CountryName, "Улучшение Тяжелой Промышленности", true, upgradeCost);
            }
            else
            {
                return new OrderResult(order.CountryName, "Улучшение Легкой Промышленности", true, upgradeCost);
            }
        }

        private OrderResult IncreaseRes(Order order, string res)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var upgradeCost = (long)_dataLayer.GetProperty(order.CountryName, "ProductionUpgradeCost");
            var InflationCoeff = (double)_dataLayer.GetProperty(order.CountryName, "InflationCoeff");
            if (money < upgradeCost* InflationCoeff)
            {
                switch (res)
                {
                    case "Oil":
                        _newsHandler.AddNews(order.CountryName,
                            new Model.News(true)
                            {
                                text = "Государство не смогло профинансировать повышение добычи нефти!"
                            });
                        return new OrderResult(order.CountryName, "Повышение добычи нефти", false, upgradeCost);
                    case "Steel":
                        _newsHandler.AddNews(order.CountryName,
                            new Model.News(true)
                            {
                                text = "Государство не смогло профинансировать повышение выплавки стали!"
                            });
                        return new OrderResult(order.CountryName, "Повышение выплавки стали", false, upgradeCost);
                    case "Wood":
                        _newsHandler.AddNews(order.CountryName,
                            new Model.News(true)
                            {
                                text = "Государство не смогло профинансировать повышение производства древесины!"
                            });
                        return new OrderResult(order.CountryName, "Повышение производства древесины", false, (long)(upgradeCost* InflationCoeff));
                    case "Agricultural":
                        _newsHandler.AddNews(order.CountryName,
                            new Model.News(true)
                            {
                                text = "Государство не смогло профинансировать повышение сельскохозяйственного производства!"
                            });
                        return new OrderResult(order.CountryName, "Повышение сельскохозяйственного производства", false, (long)(upgradeCost* InflationCoeff));
                }
            }

            money -= (long)(upgradeCost* InflationCoeff);
            _dataLayer.SetProperty(order.CountryName, "Money", money);
            upgradeCost = (long)(Constants.UpgradeCostRate * upgradeCost);
            _dataLayer.SetProperty(order.CountryName, "ProductionUpgradeCost", upgradeCost);

            var resProduction = (double)_dataLayer.GetProperty(order.CountryName, "Res" + res);
            resProduction += Constants.ProductionUpgrade;
            _dataLayer.SetProperty(order.CountryName, "Res" + res, resProduction);

            switch(res)
            {
                case "Oil":
                    _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Повышена добыча нефти!" });
                    return new OrderResult(order.CountryName, "Повышение добычи нефти", true, (long)(upgradeCost* InflationCoeff));
                    break;
                case "Steel":
                    _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Повышена выплавка стали!" });
                    return new OrderResult(order.CountryName, "Повышение выплавки стали", true, (long)(upgradeCost* InflationCoeff));
                    break;
                case "Wood":
                    _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Повышено производство древесины!" });
                    return new OrderResult(order.CountryName, "Повышение производства древесины", true, (long)(upgradeCost* InflationCoeff));
                    break;
                case "Agricultural":
                    _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Повышено сельскохозяйственное производство!" });
                    return new OrderResult(order.CountryName, "Повышение сельскохозяйственного производства", true, (long)(upgradeCost* InflationCoeff));
                    break;
            }
            return new OrderResult(order.CountryName, "____", false, 0);

        }
    }
}
