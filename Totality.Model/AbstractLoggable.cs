using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Model.Interfaces;

namespace Totality.Model
{
    public class AbstractLoggable
    {
        protected ILogger _log;

        public AbstractLoggable(ILogger log = null)
        {
            _log = log;
        }
    }
}
