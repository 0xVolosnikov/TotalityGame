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

        public bool ProcessOrder(Order order)
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

        private bool reorganize(Order order)
        {
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

            return true;
        }

        private bool lvlUp(Order order)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var PremierLvlUpCost = (long)_dataLayer.GetProperty(order.CountryName, "PremierLvlUpCost");

            if (money < PremierLvlUpCost)
                return false;

            money -= PremierLvlUpCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);
            PremierLvlUpCost = (long)(PremierLvlUpCost * Constants.PremierLvlUpCostRatio);
            _dataLayer.SetProperty(order.CountryName, "PremierLvlUpCost", PremierLvlUpCost);

            var lvl = (int)_dataLayer.GetProperty(order.CountryName, "PremierLvl") + 1;
            _dataLayer.SetProperty(order.CountryName, "PremierLvl", lvl);

            return true;
        }

        private bool alert(Order order)
        {
            _dataLayer.SetProperty(order.CountryName, "IsAlerted", true);
            return true;
        }

        private bool unAlert(Order order)
        {
            _dataLayer.SetProperty(order.CountryName, "IsAlerted", false);
            return true;
        }
    }
}
