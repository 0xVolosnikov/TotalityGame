using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Totality.Client.ClientComponents
{
    public class OrderRecord
    {
        public string text { get; set; }
        public string cost { get; set; }
        public OrderRecord(string text, string cost = "")
        {
            this.text = text;
            this.cost = cost;
        }
    }
}
