using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totality.Model.Diplomatical
{
    public class ContractTrade : DipContract
    {
        public string Text { get; set; }
        public long Price;
        public int Count;
        public int Time;
        public string Res;

        public ContractTrade(Guid id, string from, string to) : base(DipMsg.Types.Trade, from, to)
        {
        }
    }
}
