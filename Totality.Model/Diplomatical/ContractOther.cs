using System;

namespace Totality.CommonClasses.Diplomatical
{
    class ContractOther : DipContract
    {
        public string Text { get; set; }

        public ContractOther(Guid id, string from, string to, string text) : base(from, to)
        {
            Text = text;
        }
    }
}
