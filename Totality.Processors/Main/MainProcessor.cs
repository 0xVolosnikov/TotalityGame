using Totality.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Model;

namespace Totality.Processors.Main
{
    public class MainProcessor
    {
        private Dictionary<string, Queue<Order>> _ordersBase = new Dictionary<string, Queue<Order>>();
        private List<IMinisteryProcessor> _ministeryProcessors = new List<IMinisteryProcessor>();
        private List<Order> _currentOrdersLine = new List<Order>();

        public MainProcessor()
        {
            _ministeryProcessors.Add(new MinIndustryProcessor());
            _ministeryProcessors.Add(new MinFinanceProcessor());
            _ministeryProcessors.Add(new MinMilitaryProcessor());
            _ministeryProcessors.Add(new MinForeignProcessor());
            _ministeryProcessors.Add(new MinMediaProcessor());
            _ministeryProcessors.Add(new MinMVDProcessor());
            _ministeryProcessors.Add(new MinFSBProcessor());
            _ministeryProcessors.Add(new MinScienceProcessor());
            _ministeryProcessors.Add(new MinPremierProcessor());
        }

        public void AddCountry(string name)
        {
            _ordersBase.Add(name, new Queue<Order>());
        }

        public void RemoveCountry(string name)
        {
            _ordersBase.Remove(name);
        }

        public void AddOrder(string name, Order newOrder)
        {
            _ordersBase[name].Enqueue(newOrder);
        }

        public void ProcessOrders()
        {
            while(_ordersBase.Any(x => x.Value.Any()))
            {
                foreach (KeyValuePair<string, Queue<Order>> qu in _ordersBase.Where(x => x.Value.Any()))
                {
                    _currentOrdersLine.Add(qu.Value.Dequeue());
                }
                // захуярить фсбшную обработку
                // и новостную
                for (int i = 0; i < _currentOrdersLine.Count; i++)
                {
                    _ministeryProcessors[_currentOrdersLine[i].Args[0]].ProcessOrder(_currentOrdersLine[i]);
                }
                _currentOrdersLine.Clear();
            }
        }

    }
}
