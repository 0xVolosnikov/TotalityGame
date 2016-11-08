using System;

namespace Totality.Model.Diplomatical
{
    public class ContractOther : DipContract
    {
        public string Text { get; set; }

        public ContractOther(Guid id, string from, string to) : base(DipMsg.Types.Other, from, to)
        {
        }
    }
}
