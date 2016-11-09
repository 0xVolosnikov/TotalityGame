using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace Totality.Model
{
    [DataContract]
    public class Order
    {
        [DataMember]
        public string CountryName { get; }
        [DataMember]
        public string TargetCountryName { get; }
        [DataMember]
        public string TargetCountryName2 { get; set; }
        [DataMember]
        public long Count { get; set; }
        [DataMember]
        public short Value { get; set; }
        [DataMember]
        public short OrderNum { get; set; }
        [DataMember]
        public short Ministery { get; set; }
        [DataMember]
        public short TargetMinistery { get; set; }
        [DataMember]
        public Guid TargetId { get; set; }
        [DataMember]
        public bool isSecret = false;

        public Order(string countryName, string targetCountryName = null)
        {
            CountryName = countryName;
            TargetCountryName = targetCountryName;
        }
    }
}
