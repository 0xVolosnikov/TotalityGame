using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    class MinFinanceHandler : AbstractHandler, IMinisteryHandler
    {
        public MinFinanceHandler(IDataLayer dataLayer, ILogger logger) : base(dataLayer, logger)
        {
        }

        enum actions { action1, action2 };

        public bool ProcessOrder(Order order)
        {
            switch (order.Args[1])
            {
                case (int)actions.action1:
                    return true;
                    break;

                case (int)actions.action2:
                    return true;
                    break;

                default:
                    return false;
                    break;
            }
        }
    }
}
