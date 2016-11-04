using System.Collections.Generic;
using System.Linq;
using Totality.Handlers.Nuke;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    public class MainHandler : AbstractHandler
    {
        private Dictionary<string, Queue<Order>> _ordersBase = new Dictionary<string, Queue<Order>>();
        private List<IMinisteryHandler> _ministeryHandlers = new List<IMinisteryHandler>();
        private List<Order> _currentOrdersLine = new List<Order>();

        public MainHandler(IDataLayer dataLayer, ILogger logger, NukeHandler nukeHandler) : base(dataLayer, logger)
        {
            _ministeryHandlers.Add(new MinIndustryHandler(dataLayer, logger));
            _ministeryHandlers.Add(new MinFinanceHandler(dataLayer, logger));
            _ministeryHandlers.Add(new MinMilitaryHandler(dataLayer, nukeHandler, logger));
            _ministeryHandlers.Add(new MinForeignHandler(dataLayer, logger));
            _ministeryHandlers.Add(new MinMediaHandler(dataLayer, logger));
            _ministeryHandlers.Add(new MinInnerHandler(dataLayer, logger));
            _ministeryHandlers.Add(new MinFsbHandler(dataLayer, logger));
            _ministeryHandlers.Add(new MinScienceHandler(dataLayer, logger));
            _ministeryHandlers.Add(new MinPremierHandler(dataLayer, logger));
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

        public void ProcessOrder()
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
                    _ministeryHandlers[_currentOrdersLine[i].Ministery].ProcessOrder(_currentOrdersLine[i]);
                }
                _currentOrdersLine.Clear();
            }
        }

    }
}
