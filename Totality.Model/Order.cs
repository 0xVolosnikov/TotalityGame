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
        public string CountryName;
        public List<int> Args { get; }

        public Order(string countryName, params int[] args)
        {
            CountryName = countryName; 
            Args = new List<int>();
            Args.AddRange(args);
        }
    }
}
