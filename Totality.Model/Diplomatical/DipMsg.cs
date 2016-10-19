using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Totality.CommonClasses
{
    [DataContract]
    public class DipMsg
    {
        public string From { get; }
        public string To { get; }
        public string Offer { get; } //  программно сгенерированное описание контракта
        public string Text { get; }  //  дополнительный текст
        public Guid Id;

        public DipMsg(string from, string to, string offer, string text)
        {
            From = from;
            To = to;
            Offer = offer;
            Text = text;
            Id = Guid.NewGuid();
        }
    }
}
