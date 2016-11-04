using System;

namespace Totality.CommonClasses.Diplomatical
{
    public class DipContract
    {
        public Guid Id { get; }
        public string From { get; }
        public string To { get; }
        public DipContract(string from, string to)
        {
            Id = Guid.NewGuid();
            From = from;
            To = to;
        }
    }
}
