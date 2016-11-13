using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Model;

namespace Totality.Client.ClientComponents
{
    public class OrderRecord
    {
        public string Text { get; set; }
        public string Cost { get; set; }
        public Order Order { get; set; }
        public OrderRecord(string text, string cost = "", Order order = null)
        {
            Text = text;
            Cost = cost;
            Order = order;
        }
    }
}
