using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace CommonClasses
{
    [DataContract]
    public class Order
    {
        public List<int> args = new List<int>();
        public Order(params int[] args)
        {
            this.args.AddRange(args);
        }
    }
}
