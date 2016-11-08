using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totality.Model.Diplomatical
{
    public class ContractMilitaryTraining : DipContract
    {
        public string Text { get; set; }

        public ContractMilitaryTraining( Guid id, string from, string to) : base(DipMsg.Types.MilitaryTraining, from, to)
        {
        }
    }
}
