using System;
using System.Runtime.Serialization;

namespace Totality.Model.Diplomatical
{
    [DataContract]
    public class DipMsg
    {
        public enum Types { Trade, Peace, Alliance, CurrencyAlliance, Transfer, MilitaryTraining, Other };

        [DataMember]
        public string From;
        [DataMember]
        public string To;
        [DataMember]
        public string Description;
        [DataMember]
        public string Text;
        [DataMember]
        public Types Type;
        [DataMember]
        public string Resource;
        [DataMember]
        public long Price;
        [DataMember]
        public bool Applied = false;
        [DataMember]
        public Guid Id;
        [DataMember]
        public int Time;
        [DataMember]
        public long Count;

        public DipMsg(string from, string to)
        {
            From = from;
            To = to;
            Id = Guid.NewGuid();
        }
    }
}
