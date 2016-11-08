using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totality.Model.Diplomatical
{
    public class ContractCurrencyAlliance : DipContract
        {
            public string Text { get; set; }

            public ContractCurrencyAlliance( Guid id, string from, string to) : base(DipMsg.Types.CurrencyAlliance, from, to)
            {
            }
        }
}
