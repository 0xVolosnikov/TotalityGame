using System;
using System.Collections.Generic;
using Totality.CommonClasses;
using Totality.Handlers.News;
using Totality.Model;
using Totality.Model.Interfaces;
using static Totality.Model.Country;

namespace Totality.Handlers.Main
{
    public class MinPremierHandler : AbstractHandler, IMinisteryHandler
    {
        private enum Orders { MinisteryReorganization, LvlUp, Alert, UnAlert }

        public MinPremierHandler(NewsHandler newsHandler, IDataLayer dataLayer, ILogger logger) : base(newsHandler, dataLayer, logger)
        {
        }

        public OrderResult ProcessOrder(Order order)
        {
            switch (order.OrderNum)
            {
                case (int)Orders.MinisteryReorganization: return reorganize(order);

                case (int)Orders.LvlUp: return lvlUp(order);

                case (int)Orders.Alert: return alert(order);

                case (int)Orders.UnAlert: return unAlert(order);

                default: throw new ArgumentException("Order " + order + " not found in " + typeof(MinPremierHandler));
            }
        }

        private OrderResult reorganize(Order order)
        {

            var c = _dataLayer.GetCountry(order.CountryName);
            if (c.Money < 600000)
            {
                _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Не хватает денег на реорганизацию министерства." });
                return new OrderResult(order.CountryName, "Реорганизация министерства " + order.TargetMinistery, false, 6000000);
            }
            c.Money -= 600000;
            _dataLayer.UpdateCountry(c);

            var foreignSpyes = (List<List<string>>)_dataLayer.GetProperty(order.CountryName, "ForeignSpyes");
            foreach (string country in foreignSpyes[order.TargetMinistery])
            {
                var foreignSpyNetwork = (Dictionary<string, SpyNetwork>)_dataLayer.GetProperty(country, "SpyNetworks");
                foreignSpyNetwork[order.CountryName].Recruit[order.TargetMinistery] = false;
                _dataLayer.SetProperty(country, "SpyNetworks", foreignSpyNetwork);
            }
            foreignSpyes[order.TargetMinistery].Clear();
            _dataLayer.SetProperty(order.CountryName, "ForeignSpyes", foreignSpyes);

            var minsBlocks = (int[])_dataLayer.GetProperty(order.CountryName, "MinsBlocks");
            minsBlocks[order.TargetMinistery] += Constants.ReorganizingTime;
            _dataLayer.SetProperty(order.CountryName, "MinsBlocks", minsBlocks);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Министерство " + order.TargetMinistery + "реорганизовано." });
            return new OrderResult(order.CountryName, "Реорганизация министерства " + order.TargetMinistery, true, 6000000);
        }

        private OrderResult lvlUp(Order order)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var PremierLvlUpCost = (long)_dataLayer.GetProperty(order.CountryName, "PremierLvlUpCost");

            if (money < PremierLvlUpCost)
            {
                _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Не хватает денег на усиление власти." });
                return new OrderResult(order.CountryName, "Усиление власти" + order.TargetMinistery, false, PremierLvlUpCost);
            }

            money -= PremierLvlUpCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);
            PremierLvlUpCost = (long)(PremierLvlUpCost * Constants.PremierLvlUpCostRatio);
            _dataLayer.SetProperty(order.CountryName, "PremierLvlUpCost", PremierLvlUpCost);

            var lvl = (int)_dataLayer.GetProperty(order.CountryName, "PremierLvl") + 1;
            _dataLayer.SetProperty(order.CountryName, "PremierLvl", lvl);

            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Усилена власть." });
            return new OrderResult(order.CountryName, "Усиление власти" + order.TargetMinistery, true, PremierLvlUpCost);
        }

        private OrderResult alert(Order order)
        {
            _dataLayer.SetProperty(order.CountryName, "IsAlerted", true);
            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Объявлено чрезвычайное положение!" });
            return new OrderResult(order.CountryName, "Объявление чрезвычайного положения", true);
        }

        private OrderResult unAlert(Order order)
        {
            _dataLayer.SetProperty(order.CountryName, "IsAlerted", false);
            _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Чрезвычайное положение отменено!" });
            return new OrderResult(order.CountryName, "Отмена чрезвычайного положения", true);
        }
    }
}
