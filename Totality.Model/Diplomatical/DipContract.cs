using System;

namespace Totality.Model.Diplomatical
{
    [DataContract]
    public class DipContract
    {
        public Guid Id { get; }
        public string From { get; }
        public string To { get; }
        public DipMsg.Types Type;
        public bool Broken = false;
        public DipContract(DipMsg.Types type, string from, string to)
        {
            Id = Guid.NewGuid();
            From = from;
            To = to;
            Type = type;
        }
    }
}
