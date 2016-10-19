using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Model.Interfaces;

namespace Totality.Processors.Diplomatical
{
    public class DiplomaticalProcessor : AbstractProcessor
    {
        public DiplomaticalProcessor(IDataLayer dataLayer) : base(dataLayer)
        {
        }
    }
}
