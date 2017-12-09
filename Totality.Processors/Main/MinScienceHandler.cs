using System;
using Totality.CommonClasses;
using Totality.Handlers.News;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    public class MinScienceHandler : AbstractHandler, IMinisteryHandler
    {
        private enum Orders { ImproveExtract , ImproveHeavy , ImproveLight , ImproveMilitary }

        public MinScienceHandler(NewsHandler newsHandler, IDataLayer dataLayer, ILogger logger) : base(newsHandler, dataLayer, logger)
        {
        }

        public OrderResult ProcessOrder(Order order)
        {
            switch (order.OrderNum)
            {
                case (int)Orders.ImproveExtract: return Improve(order, "Extract");

                case (int)Orders.ImproveHeavy: return Improve(order, "Heavy");

                case (int)Orders.ImproveLight: return Improve(order, "Light");

                case (int)Orders.ImproveMilitary: return Improve(order, "Military");

                default: throw new ArgumentException("Order " + order + " not found in " + typeof(MinScienceHandler));
            }
        }

        private OrderResult Improve(Order order, string sector)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var sectorLvlUpCost = (long)_dataLayer.GetProperty(order.CountryName, sector + "ScLvlUpCost");
            var inflationCoeff = (double)_dataLayer.GetProperty(order.CountryName, "InflationCoeff");

            if (money < sectorLvlUpCost*inflationCoeff)
                switch (sector)
                {
                    case "Extract":
                        _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Не хватило денег для улучшения технологий добывающей промышленности!" });
                        return new OrderResult(order.CountryName, "Улучшение технологий добывающей промышленности", false, (long)(sectorLvlUpCost*inflationCoeff));
                        break;
                    case "Heavy":
                        _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Не хватило денег для улучшения технологий тяжелой промышленности!" });
                        return new OrderResult(order.CountryName, "Улучшение технологий тяжелой промышленности", false, (long)(sectorLvlUpCost * inflationCoeff));
                        break;
                    case "Light":
                        _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Не хватило денег для улучшения технологий легкой промышленности!" });
                        return new OrderResult(order.CountryName, "Улучшение технологий легкой промышленности", false, (long)(sectorLvlUpCost * inflationCoeff));
                        break;
                    case "Military":
                        _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Не хватило денег для улучшения технологий военной промышленности!" });
                        return new OrderResult(order.CountryName, "Улучшение технологий военной промышленности", false, (long)(sectorLvlUpCost * inflationCoeff));
                        break;
                }

            var exp = (int)_dataLayer.GetProperty(order.CountryName, sector + "Experience");
            var sectorLvlUpExp = (int)_dataLayer.GetProperty(order.CountryName, sector + "ScLvlUpExp");
            if (exp < sectorLvlUpExp)
                switch (sector)
                {
                    case "Extract":
                        _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Не хватило опыта для улучшения технологий добывающей промышленности!" });
                        return new OrderResult(order.CountryName, "Улучшение технологий добывающей промышленности", false, (long)(sectorLvlUpCost * inflationCoeff));
                        break;
                    case "Heavy":
                        _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Не хватило опыта для улучшения технологий тяжелой промышленности!" });
                        return new OrderResult(order.CountryName, "Улучшение технологий тяжелой промышленности", false, (long)(sectorLvlUpCost * inflationCoeff));
                        break;
                    case "Light":
                        _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Не хватило опыта для улучшения технологий легкой промышленности!" });
                        return new OrderResult(order.CountryName, "Улучшение технологий легкой промышленности", false, (long)(sectorLvlUpCost * inflationCoeff));
                        break;
                    case "Military":
                        _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Не хватило опыта для улучшения технологий военной промышленности!" });
                        return new OrderResult(order.CountryName, "Улучшение технологий военной промышленности", false, (long)(sectorLvlUpCost * inflationCoeff));
                        break;
                }

            money -= (long)(sectorLvlUpCost * inflationCoeff);
            _dataLayer.SetProperty(order.CountryName, "Money", money);

            switch (sector)
            {
                case "Extract":
                    sectorLvlUpCost = (long)(sectorLvlUpCost * Constants.ExtractScLvlUpCostRatio);
                    break;
                case "Heavy":
                    sectorLvlUpCost = (long)(sectorLvlUpCost * Constants.HeavyScLvlUpCostRatio);
                    break;
                case "Light":
                    sectorLvlUpCost = (long)(sectorLvlUpCost * Constants.LightScLvlUpCostRatio);
                    break;
                case "Military":
                    sectorLvlUpCost = (long)(sectorLvlUpCost * Constants.MilitaryScLvlUpCostRatio);
                    break;
            }
            _dataLayer.SetProperty(order.CountryName, sector + "ScLvlUpCost", sectorLvlUpCost);

            exp -= sectorLvlUpExp;
            _dataLayer.SetProperty(order.CountryName, sector + "Experience", exp);
            switch (sector)
            {
                case "Extract":
                    sectorLvlUpExp = (int)(sectorLvlUpExp * Constants.ExtractScLvlUpExpRatio);
                    _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Улучшены технологии добывающей промышленности." });
                    break;
                case "Heavy":
                    sectorLvlUpExp = (int)(sectorLvlUpExp * Constants.HeavyScLvlUpExpRatio);
                    _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Улучшены технологии тяжелой промышленности." });
                    break;
                case "Light":
                    sectorLvlUpExp = (int)(sectorLvlUpExp * Constants.LightScLvlUpExpRatio);
                    _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Улучшены технологии легкой промышленности." });
                    break;
                case "Military":
                    sectorLvlUpExp = (int)(sectorLvlUpExp * Constants.MilitaryScLvlUpExpRatio);
                    _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Улучшены военные технологии." });
                    break;
            }
            _dataLayer.SetProperty(order.CountryName, sector + "ScLvlUpExp", sectorLvlUpExp);
            _dataLayer.SetProperty(order.CountryName, sector + "Experience", 0);

            var lvl = (int)_dataLayer.GetProperty(order.CountryName, sector + "ScienceLvl") + 1;
            _dataLayer.SetProperty(order.CountryName, sector + "ScienceLvl", lvl);

            switch (sector)
            {
                case "Extract":
                    return new OrderResult(order.CountryName, "Улучшение технологий добывающей промышленности", true, (long)(sectorLvlUpCost * inflationCoeff));
                    break;
                case "Heavy":
                    return new OrderResult(order.CountryName, "Улучшение технологий тяжелой промышленности", true, (long)(sectorLvlUpCost * inflationCoeff));
                    break;
                case "Light":
                    return new OrderResult(order.CountryName, "Улучшение технологий легкой промышленности", true, (long)(sectorLvlUpCost * inflationCoeff));
                    break;
                case "Military":
                    return new OrderResult(order.CountryName, "Улучшение технологий военной промышленности", true, (long)(sectorLvlUpCost * inflationCoeff));
                    break;
                default:
                    return new OrderResult(order.CountryName, "_______", false, 0);

            }
        }
    }
}
