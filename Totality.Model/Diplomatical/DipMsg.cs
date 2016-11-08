using System;
using System.Runtime.Serialization;

namespace Totality.Model.Diplomatical
{
    [DataContract]
    public class DipMsg
    {
        public enum Types { Trade, Peace, Alliance, CurrencyAlliance, Transfer, MilitaryTraining, Other };

        public string From { get; }
        public string To { get; }
        public string Offer; //  программно сгенерированное описание контракта
        public string Text; //  дополнительный текст
        public Types Type;
        public string Resource;
        public long Price;
        public bool Applied = false;
        public Guid Id;
        public int Time;
        public long Count;

        public DipMsg(string from, string to)
        {
            From = from;
            To = to;
            Id = Guid.NewGuid();
        }
    }
}
