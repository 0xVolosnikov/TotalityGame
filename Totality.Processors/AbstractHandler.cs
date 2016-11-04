using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers
{
    public class AbstractHandler : AbstractLoggable
    {
        protected IDataLayer _dataLayer;
        public AbstractHandler(IDataLayer dataLayer, ILogger logger) : base (logger)
        {
            _dataLayer = dataLayer;
        }
    }
}
