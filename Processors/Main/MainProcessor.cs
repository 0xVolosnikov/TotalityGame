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
        private Dictionary<string, Queue<Order>> ordersBase = new Dictionary<string, Queue<Order>>();
        private List<IMinisteryProcessor> ministeryProcessors = new List<IMinisteryProcessor>();
        private List<Order> currentOrdersLine = new List<Order>();

        public MainProcessor()
        {
            ministeryProcessors.Add(new MinIndustryProcessor());
            ministeryProcessors.Add(new MinFinanceProcessor());
            ministeryProcessors.Add(new MinMilitaryProcessor());
            ministeryProcessors.Add(new MinForeignProcessor());
            ministeryProcessors.Add(new MinMediaProcessor());
            ministeryProcessors.Add(new MinMVDProcessor());
            ministeryProcessors.Add(new MinFSBProcessor());
            ministeryProcessors.Add(new MinScienceProcessor());
            ministeryProcessors.Add(new MinPremierProcessor());
        }

        public void addCountry(string name)
        {
            ordersBase.Add(name, new Queue<Order>());
        }

        public void removeCountry(string name)
        {
            ordersBase.Remove(name);
        }

        public void addOrder(string name, Order newOrder)
        {
            ordersBase[name].Enqueue(newOrder);
        }

        public void processOrders()
        {
            while(ordersBase.Any(x => x.Value.Any()))
            {
                foreach (KeyValuePair<string, Queue<Order>> qu in ordersBase.Where(x => x.Value.Any()))
                {
                    currentOrdersLine.Add(qu.Value.Dequeue());
                }
                // захуярить фсбшную обработку
                // и новостную
                for (int i = 0; i < currentOrdersLine.Count; i++)
                {
                    ministeryProcessors[currentOrdersLine[i].args[0]].processOrder(currentOrdersLine[i]);
                }
                currentOrdersLine.Clear();
            }
        }

    }
}
