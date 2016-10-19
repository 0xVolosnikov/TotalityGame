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
    class MinPremierProcessor : AbstractProcessor, IMinisteryProcessor
    {
        public MinPremierProcessor(IDataLayer dataLayer, ILogger logger) : base(dataLayer, logger)
        {
        }

        public bool ProcessOrder(Order order)
        {
            throw new NotImplementedException();
        }
    }
}
