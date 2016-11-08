using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totality.Model.Diplomatical
{
    public class ContractPeace : DipContract
    {
        public string Text { get; set; }

        public ContractPeace(Guid id, string from, string to) : base(DipMsg.Types.Peace, from, to)
        {
        }
    }
}
