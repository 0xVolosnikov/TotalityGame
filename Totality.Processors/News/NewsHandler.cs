using Totality.CommonClasses;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Totality.Model.Interfaces;

namespace Totality.Handlers.News
{
    public class NewsHandler
    {
        public ITransmitter Transmitter { get; set; }
        private Dictionary<string, List<Model.News>> _newsBase = new Dictionary<string, List<Model.News>>();


        public void AddNews(string countryName, Model.News news)
        {
            if (!_newsBase.ContainsKey(countryName))
            {
                var newsList = new List<Model.News>();
                newsList.Add(news);
                _newsBase.Add(countryName, newsList);
            }
            else
            {
                _newsBase[countryName].Add(news);
            }
        }

        public void SendNews()
        {
            Transmitter.SendNews(_newsBase);
            _newsBase.Clear();
        }
    }
}
