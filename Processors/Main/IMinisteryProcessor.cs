using Totality.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Model;

namespace Totality.Processors.Main
{
    interface IMinisteryProcessor
    {
        bool ProcessOrder(Order order);
    }
}
