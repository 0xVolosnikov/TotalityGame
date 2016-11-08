using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totality.Model.Diplomatical
{
    public class ContractAlliance : DipContract
    {
        public string Text { get; set; }

        public ContractAlliance(Guid id, string from, string to) : base(DipMsg.Types.Alliance, from, to)
        {
        }
    }
}
