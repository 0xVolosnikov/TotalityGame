using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Model.Interfaces;

namespace Totality.Processors
{
    public class AbstractProcessor
    {
        protected IDataLayer _dataLayer;
        public AbstractProcessor(IDataLayer dataLayer)
        {
            _dataLayer = dataLayer;
        }
    }
}
