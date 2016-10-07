using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses.Diplomatical
{
    class ContractOther : DipContract
    {
        public string text;

        public ContractOther(int id, string from, string to) : base(id, from, to)
        {
            this.id = id;
            this.from = from;
            this.to = to;
        }
    }
}
