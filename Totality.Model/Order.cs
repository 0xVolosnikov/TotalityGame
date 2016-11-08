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
        public string CountryName { get; }
        public string TargetCountryName { get; }
        public string TargetCountryName2 { get; set; }
        public long Count { get; set; }
        public short Value { get; set; }
        public short Value2 { get; set; }
        public short OrderNum { get; set; }
        public short Ministery { get; set; }
        public short TargetMinistery { get; set; }
        public Guid TargetId { get; set; }
        public bool isSecret = false;

        public Order(string countryName, string targetCountryName = null)
        {
            CountryName = countryName;
            TargetCountryName = targetCountryName;
        }
    }
}
