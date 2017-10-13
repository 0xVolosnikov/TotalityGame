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
        private MinForeignHandler _foreignHandler;

        public MinMilitaryHandler(NewsHandler newsHandler, IDataLayer dataLayer, NukeHandler nukeHandler, ILogger logger) : base(newsHandler, dataLayer, logger)
        {
            _nukeHandler = nukeHandler;
        }

        public OrderResult ProcessOrder(Order order)
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

        private OrderResult Mobilize(Order order)
        {
            _dataLayer.SetProperty(order.CountryName, "IsMobilized", true);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Объявлена всеобщая мобилизация." });
            _newsHandler.AddBroadNews(new Model.News(false) { text = "Страна " + order.CountryName + " начала активную мобилизацию!" });
            return new OrderResult(order.CountryName, "Начало мобилизации", true);
        }

        private OrderResult Demobilize(Order order)
        {
            _dataLayer.SetProperty(order.CountryName, "IsMobilized", false);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Всеобщая мобилизация прекращена." });
            return new OrderResult(order.CountryName, "Окончание мобилизации", true);
        }

        private OrderResult IncreaseUranium(Order order)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var upgradeCost = (long)_dataLayer.GetProperty(order.CountryName, "ProductionUpgradeCost");

            if (money < upgradeCost)
            {
                _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "МинОбороны не хватило денег на повышение добычи урана!" });
                return new OrderResult(order.CountryName, "Повышение добычи урана", false, upgradeCost);
            }

            money -= upgradeCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);
            upgradeCost = (long)(Constants.UpgradeCostRate*upgradeCost);
            _dataLayer.SetProperty(order.CountryName, "ProductionUpgradeCost", upgradeCost);

            var uraniumProduction = (double)_dataLayer.GetProperty(order.CountryName, "ProdUranus");
            uraniumProduction += Constants.ProductionUpgrade/5;
            _dataLayer.SetProperty(order.CountryName, "ProdUranus", uraniumProduction);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Повышено производство оружейного урана." });
            return new OrderResult(order.CountryName, "Повышение добычи урана", true, upgradeCost);
        }

        private OrderResult MakeNukes(Order order)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var uranus = (double)_dataLayer.GetProperty(order.CountryName, "ResUranus");
            var heavyPower = (double)_dataLayer.GetProperty(order.CountryName, "FinalHeavyIndustry");
            var usedHeavyPower = (double)_dataLayer.GetProperty(order.CountryName, "UsedHIpower");

            if ( money < Constants.NukeCost * order.Count)
            {
                _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "МинОбороны не хватило денег на создание ядерных ракет!" });
                return new OrderResult(order.CountryName, "Производство ядерных ракет в количестве: " + order.Count, false, Constants.NukeCost * order.Count);
            }

            if (uranus < Constants.NukeUranusCost)
            {
                _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "МинОбороны не хватило урана на создание ядерных ракет!" });
                return new OrderResult(order.CountryName, "Производство ядерных ракет в количестве: " + order.Count, false, Constants.NukeCost * order.Count);
            }

            if (heavyPower - usedHeavyPower < order.Count*Constants.NukeHeavyPower)
            {
                _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "МинОбороны не хватило Тяжелой промышленной мощи на создание ядерных ракет!" });
                return new OrderResult(order.CountryName, "Производство ядерных ракет в количестве: " + order.Count, false, Constants.NukeCost * order.Count);
            }


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
            return new OrderResult(order.CountryName, "Производство ядерных ракет в количестве: " + order.Count, true, Constants.NukeCost * order.Count);
        }

        private OrderResult MakeMissiles(Order order)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var heavyPower = (double)_dataLayer.GetProperty(order.CountryName, "FinalHeavyIndustry");
            var usedHeavyPower = (double)_dataLayer.GetProperty(order.CountryName, "UsedHIpower");

            if (money < Constants.MissileCost * order.Count)
            {
                _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "МинОбороны не хватило денег на ПРО!" });
                return new OrderResult(order.CountryName, "Производство систем ПРО в количестве: " + order.Count, false, Constants.MissileCost * order.Count);
            }
            if (heavyPower - usedHeavyPower < order.Count * Constants.MissileHeavyPower)
            {
                _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "МинОбороны не хватило Тяжелой промышленной мощи для создания ПРО!" });
                return new OrderResult(order.CountryName, "Производство систем ПРО в количестве: " + order.Count, false, Constants.MissileCost * order.Count);
            }


            money -= Constants.MissileCost * order.Count;
            _dataLayer.SetProperty(order.CountryName, "Money", money);
            usedHeavyPower += order.Count * Constants.MissileHeavyPower;
            _dataLayer.SetProperty(order.CountryName, "UsedHIpower", usedHeavyPower);

            var missiles = (int)_dataLayer.GetProperty(order.CountryName, "MissilesCount");
            missiles += (int)order.Count;
            _dataLayer.SetProperty(order.CountryName, "MissilesCount", missiles);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Произведены системы ПРО, в количестве " + order.Count+ "." });
            return new OrderResult(order.CountryName, "Производство систем ПРО в количестве: " + order.Count, true, Constants.MissileCost * order.Count);
        }

        private OrderResult NukeStrike(Order order)
        {
            var nukes = (int)_dataLayer.GetProperty(order.CountryName, "NukesCount");


            if (nukes < order.Count || order.Count == 0)
            {
                _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Не удалось запустить ядерные ракеты!"});
                return new OrderResult(order.CountryName, "Запуск ядерных ракет", false);
            }

            nukes -= (int)order.Count;
            _dataLayer.SetProperty(order.CountryName, "NukesCount", nukes);

            _nukeHandler.AddRocket(new NukeRocket(order.CountryName, order.TargetCountryName, (int)order.Count) { AttackerLvl = (int)_dataLayer.GetProperty(order.CountryName, "MilitaryScienceLvl") });

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Запущены ядерные ракеты по стране " + order.TargetCountryName + "." });
            return new OrderResult(order.CountryName, "Запуск ядерных ракет", true);
        }

        private OrderResult StartWar(Order order)
        {

            if (_dataLayer.GetCountry(order.CountryName).Alliance ==
                _dataLayer.GetCountry(order.TargetCountryName).Alliance)
            {
                if (_dataLayer.GetCountry(order.CountryName).IsBoss)
                    _dataLayer.SetProperty(order.TargetCountryName, "Alliance", order.TargetCountryName);
                else
                    _dataLayer.SetProperty(order.CountryName, "Alliance", order.CountryName);
            }

            var warList = (List<string>)_dataLayer.GetProperty(order.CountryName, "WarList");
            warList.Add(order.TargetCountryName);
            _dataLayer.SetProperty(order.CountryName, "WarList", warList);

            var targetWarList = (List<string>)_dataLayer.GetProperty(order.TargetCountryName, "WarList");
            targetWarList.Add(order.CountryName);
            _dataLayer.SetProperty(order.TargetCountryName, "WarList", targetWarList);
            _log.Trace("Начата война между " + order.CountryName + " и " + order.TargetCountryName);
            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Объявлена война стране " + order.TargetCountryName  + "!"});
            _newsHandler.AddNews(order.TargetCountryName, new Model.News(true) { text = "Страна " + order.CountryName + " объявила нам войну!" });
            _newsHandler.AddBroadNews(new Model.News(false) { text = "Страна " + order.CountryName + " объявила войну стране " + order.TargetCountryName + "!" });
            return new OrderResult(order.CountryName, "Объявление войны стране " + order.TargetCountryName, true);
        }

    }
}
