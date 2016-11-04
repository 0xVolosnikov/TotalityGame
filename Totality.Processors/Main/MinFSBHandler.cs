using System;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    class MinFsbHandler : AbstractHandler, IMinisteryHandler
    {
        public MinFsbHandler(IDataLayer dataLayer, ILogger logger) : base(dataLayer, logger)
        {
        }

        public bool ProcessOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
