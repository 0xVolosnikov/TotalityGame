using System;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    class MinScienceHandler : AbstractHandler, IMinisteryHandler
    {
        public MinScienceHandler(IDataLayer dataLayer, ILogger logger) : base(dataLayer, logger)
        {
        }

        public bool ProcessOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
