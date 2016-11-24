using System;
using System.Collections.Generic;
using Totality.CommonClasses;
using Totality.Handlers.News;
using Totality.Handlers.Nuke;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    public class MinMilitaryHandler : AbstractHandler, IMinisteryHandler
    {
        private enum Orders { GeneralMobilization, Demobilization, IncreaseUranium, MakeNukes, MakeMissiles, NukeStrike, StartWar }
        private NukeHandler _nukeHandler;

        public MinMilitaryHandler(NewsHandler newsHandler, IDataLayer dataLayer, NukeHandler nukeHandler, ILogger logger) : base(newsHandler, dataLayer, logger)
        {
            _nukeHandler = nukeHandler;
        }

        public bool ProcessOrder(Order order)
        {
            switch (order.OrderNum)
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
            _dataLayer.SetProperty(order.CountryName, "IsMobilized", true);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Объявлена всеобщая мобилизация." });
            return true;
        }

        private bool Demobilize(Order order)
        {
            _dataLayer.SetProperty(order.CountryName, "IsMobilized", false);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Всеобщая мобилизация прекращена." });
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
            uraniumProduction += Constants.ProductionUpgrade;
            _dataLayer.SetProperty(order.CountryName, "ProdUranus", uraniumProduction);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Повышено производство оружейного урана." });
            return true;
        }

        private bool MakeNukes(Order order)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var uranus = (double)_dataLayer.GetProperty(order.CountryName, "ResUranus");
            var heavyPower = (double)_dataLayer.GetProperty(order.CountryName, "FinalHeavyIndustry");
            var usedHeavyPower = (double)_dataLayer.GetProperty(order.CountryName, "UsedHIpower");

            if ( money < Constants.NukeCost * order.Count || uranus < Constants.NukeUranusCost || heavyPower - usedHeavyPower < order.Count * Constants.NukeHeavyPower)
                return false;

            money -= Constants.NukeCost * order.Count;
            _dataLayer.SetProperty(order.CountryName, "Money", money);
            uranus -= Constants.NukeUranusCost;
            _dataLayer.SetProperty(order.CountryName, "ResUranus", uranus);
            usedHeavyPower += order.Count * Constants.NukeHeavyPower;
            _dataLayer.SetProperty(order.CountryName, "UsedHIpower", usedHeavyPower);

            var nukes = (int)_dataLayer.GetProperty(order.CountryName, "NukesCount");
            nukes += (int)order.Count;
            _dataLayer.SetProperty(order.CountryName, "NukesCount", nukes);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Произведены ядерные ракеты, в количестве " + order.Count + "." });
            return true;
        }

        private bool MakeMissiles(Order order)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var heavyPower = (double)_dataLayer.GetProperty(order.CountryName, "FinalHeavyIndustry");
            var usedHeavyPower = (double)_dataLayer.GetProperty(order.CountryName, "UsedHIpower");

            if (money < Constants.MissileCost * order.Count || heavyPower - usedHeavyPower < order.Count * Constants.MissileHeavyPower)
                return false;

            money -= Constants.MissileCost * order.Count;
            _dataLayer.SetProperty(order.CountryName, "Money", money);
            usedHeavyPower += order.Count * Constants.MissileHeavyPower;
            _dataLayer.SetProperty(order.CountryName, "UsedHIpower", usedHeavyPower);

            var missiles = (int)_dataLayer.GetProperty(order.CountryName, "MissilesCount");
            missiles += (int)order.Count;
            _dataLayer.SetProperty(order.CountryName, "MissilesCount", missiles);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Произведены системы ПРО, в количестве " + order.Count+ "." });
            return true;
        }

        private bool NukeStrike(Order order)
        {
            var nukes = (int)_dataLayer.GetProperty(order.CountryName, "NukesCount");

            if (nukes < order.Count)
                return false;

            nukes -= (int)order.Count;
            _dataLayer.SetProperty(order.CountryName, "NukesCount", nukes);

            _nukeHandler.AddRocket(new NukeRocket(order.CountryName, order.TargetCountryName, (int)order.Count));

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Запущены ядерные ракеты по стране " + order.TargetCountryName + "." });
            return true;
        }

        private bool StartWar(Order order)
        {
            var warList = (List<string>)_dataLayer.GetProperty(order.CountryName, "WarList");
            warList.Add(order.TargetCountryName);
            _dataLayer.SetProperty(order.CountryName, "WarList", warList);

            var targetWarList = (List<string>)_dataLayer.GetProperty(order.TargetCountryName, "WarList");
            targetWarList.Add(order.CountryName);
            _dataLayer.SetProperty(order.TargetCountryName, "WarList", targetWarList);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Объявлена война стране " + order.TargetCountryName  + "!"});
            _newsHandler.AddNews(order.TargetCountryName, new Model.News(true) { text = "Стране " + order.CountryName + " объявила нам войну!" });
            return true;
        }

    }
}
