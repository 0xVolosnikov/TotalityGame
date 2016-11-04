using System;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    class MinIndustryHandler : AbstractHandler, IMinisteryHandler
    {
        public MinIndustryHandler(IDataLayer dataLayer, ILogger logger) : base(dataLayer, logger)
        {
        }

        public bool ProcessOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
