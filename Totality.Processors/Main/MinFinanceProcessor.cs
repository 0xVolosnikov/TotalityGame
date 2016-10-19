using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.CommonClasses;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Processors.Main
{
    class MinFinanceProcessor : AbstractProcessor, IMinisteryProcessor
    {
        public MinFinanceProcessor(IDataLayer dataLayer) : base(dataLayer)
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
