using CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Processors.Main
{
    public class MainProcessor
    {
        private Dictionary<string, List<Order>> ordersBase = new Dictionary<string, List<Order>>();
        private List<IMinisteryProcessor> ministeryProcessors = new List<IMinisteryProcessor>();

        public void addCountry(string name)
        {
            ordersBase.Add(name, new List<Order>());
        }

        public void removeCountry(string name)
        {
            ordersBase.Remove(name);
        }

        public void addOrder(string name, Order newOrder)
        {
            ordersBase[name].Add(newOrder);
        }
    }
}
