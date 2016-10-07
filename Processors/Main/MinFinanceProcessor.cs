using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using CommonClasses;

namespace Processors.Main
{
    class MinFinanceProcessor : IMinisteryProcessor
    {
        enum actions { action1, action2 };

        public bool processOrder(Order order)
        {
            switch (order.args[1])
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
