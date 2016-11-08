using System;
using System.Collections.Generic;
using Totality.Handlers.News;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    public class MinMediaHandler : AbstractHandler, IMinisteryHandler
    {
        private enum Orders { ChangePropDirection }

        public MinMediaHandler(NewsHandler newsHandler, IDataLayer dataLayer, ILogger logger) : base(newsHandler, dataLayer, logger)
        {
        }

        public bool ProcessOrder(Order order)
        {
            switch (order.OrderNum)
            {
                case (int)Orders.ChangePropDirection: return ChangePropDirection(order);

                default: throw new ArgumentException("Order " + order + " not found in " + typeof(MinMediaHandler));
            }
        }

        private bool ChangePropDirection(Order order)
        {
            Dictionary<string, short> massMedia;
            massMedia = (Dictionary<string, short>)_dataLayer.GetProperty(order.CountryName, "MassMedia");
            massMedia[order.TargetCountryName] = order.Value;
            _dataLayer.SetProperty(order.CountryName, "MassMedia", massMedia);
            return true;
        }
    }
}
