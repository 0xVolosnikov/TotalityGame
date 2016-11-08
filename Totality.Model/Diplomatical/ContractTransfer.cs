using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totality.Model.Diplomatical
{
    public class ContractTransfer : DipContract
    {
        public string Text { get; set; }
        public long Count;
        public int Time;

        public ContractTransfer(Guid id, string from, string to) : base(DipMsg.Types.Transfer, from, to)
        {
        }
    }
}
