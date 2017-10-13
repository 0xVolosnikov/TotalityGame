using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Totality.Model
{
    [DataContract]
    public class OrderResult
    {
        [DataMember]
        public string Text { get; set; }

        [DataMember]
        public bool IsDone { get; set; }

        [DataMember]
        public long Price { get; set; }

        public string CountryName;

        public OrderResult(string countryName, string text, bool isDone, long price = 0)
        {
            CountryName = countryName;
            Text = text;
            IsDone = isDone;
            Price = price;
        }
    }
}