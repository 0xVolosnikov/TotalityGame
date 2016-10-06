using CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processors.Main
{
    interface IMinisteryProcessor
    {
        bool processOrder(Order order);
    }
}
