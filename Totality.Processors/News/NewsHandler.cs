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
        private List<Model.News> _broadNewsBase = new List<Model.News>();
        public List<string> Countries;


        public void AddBroadNews(Model.News news)
        {
            news.IsOur = false;
            _broadNewsBase.Add(news);
        }

        public void AddNews(string countryName, Model.News news)
        {
            news.IsOur = true;
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
            for (int i = 0; i < Countries.Count(); i++)
            {
                if (!_newsBase.ContainsKey(Countries[i]))
                    _newsBase.Add(Countries[i], new List<Model.News>());
                _newsBase[Countries[i]].AddRange(_broadNewsBase);
            }

            Transmitter.SendNews(_newsBase);
            _newsBase.Clear();
        }
    }
}
