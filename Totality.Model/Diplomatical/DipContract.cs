using System;
using System.Runtime.Serialization;

namespace Totality.Model.Diplomatical
{
    [DataContract]
    public class DipContract
    {
        [DataMember]
        public Guid Id;
        [DataMember]
        public string From;
        [DataMember]
        public string To;
        [DataMember]
        public DipMsg.Types Type;
        [DataMember]
        public bool Broken = false;
        [DataMember]
        public long Price;
        [DataMember]
        public int Count;
        [DataMember]
        public int Time;
        [DataMember]
        public string Res;
        [DataMember]
        public string Description;
        [DataMember]
        public string Text;
        [DataMember]
        public bool IsFinished = false;

        public DipContract(DipMsg.Types type, string from, string to)
        {
            Id = Guid.NewGuid();
            From = from;
            To = to;
            Type = type;
        }
    }
}
