using Totality.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Processors.Main
{
    public class MainProcessor : AbstractProcessor
    {
        private Dictionary<string, Queue<Order>> _ordersBase = new Dictionary<string, Queue<Order>>();
        private List<IMinisteryProcessor> _ministeryProcessors = new List<IMinisteryProcessor>();
        private List<Order> _currentOrdersLine = new List<Order>();

        public MainProcessor(IDataLayer dataLayer) : base(dataLayer)
        {
            _ministeryProcessors.Add(new MinIndustryProcessor(dataLayer));
            _ministeryProcessors.Add(new MinFinanceProcessor(dataLayer));
            _ministeryProcessors.Add(new MinMilitaryProcessor(dataLayer));
            _ministeryProcessors.Add(new MinForeignProcessor(dataLayer));
            _ministeryProcessors.Add(new MinMediaProcessor(dataLayer));
            _ministeryProcessors.Add(new MinMVDProcessor(dataLayer));
            _ministeryProcessors.Add(new MinFSBProcessor(dataLayer));
            _ministeryProcessors.Add(new MinScienceProcessor(dataLayer));
            _ministeryProcessors.Add(new MinPremierProcessor(dataLayer));
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
