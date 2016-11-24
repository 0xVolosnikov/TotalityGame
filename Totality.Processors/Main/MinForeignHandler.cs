using System;
using Totality.Handlers.News;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    class MinForeignHandler : AbstractHandler, IMinisteryHandler
    {
        public enum Orders { BreakContract };

        public MinForeignHandler(NewsHandler newsHandler, IDataLayer dataLayer, ILogger logger) : base(newsHandler, dataLayer, logger)
        {

        }

        public bool ProcessOrder(Order order)
        {
            switch (order.OrderNum)
            {
                case (int)Orders.BreakContract : return BreakContract(order);

                default: throw new ArgumentException("Order " + order + " not found in " + typeof(MinPremierHandler));
            }
        }

        private bool BreakContract(Order order)
        {
            try
            {
                var contract = _dataLayer.GetContractList().Find(x => x.Id == order.TargetId);
                _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Нами разорван договор со страной " + order.TargetCountryName + ", " + contract.Description });

                _dataLayer.BreakContract(order.TargetId);
                return true;
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                return false;
            }
        }
    }
}
