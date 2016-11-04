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
        public List<int> Args { get; }

        public Order(string countryName, string targetCountryName = null, params int[] args)
        {
            CountryName = countryName; 
            Args = new List<int>(args);
        }
    }
}
