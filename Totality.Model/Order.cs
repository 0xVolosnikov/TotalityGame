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
        public long Count { get; set; }
        public short OrderNum { get; set; }
        public short Ministery { get; set; }
        public short TargetMinistery { get; set; }
        public Guid TargetId { get; set; }

        public Order(string countryName, string targetCountryName = null)
        {
            CountryName = countryName;
            TargetCountryName = targetCountryName;
        }
    }
}
