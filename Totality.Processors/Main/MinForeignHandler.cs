using System;
using Totality.Handlers.News;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    class MinForeignHandler : AbstractHandler, IMinisteryHandler
    {
        public enum Orders { BreakContract };

        public MinForeignHandler(NewsHandler newsHandler, IDataLayer dataLayer, ILogger logger) : base(newsHandler, dataLayer, logger)
        {

        }

        public bool ProcessOrder(Order order)
        {
            switch (order.OrderNum)
            {
                case (int)Orders.BreakContract : return BreakContract(order);

                default: throw new ArgumentException("Order " + order + " not found in " + typeof(MinPremierHandler));
            }
        }

        private bool BreakContract(Order order)
        {
            try
            {
                var contract = _dataLayer.GetContractList().Find(x => x.Id == order.TargetId);
                _newsHandler.AddNews(order.CountryName, new Model.News(true) { text = "Нами разорван договор со страной " + order.TargetCountryName + ", " + contract.Description });
                _newsHandler.AddNews(order.TargetCountryName, new Model.News(true) { text = "Страна " + order.CountryName + ", разорвала с нами договор: " + contract.Description });

                var contracts = _dataLayer.GetContractList();
                if (contracts.Find(x => x.Id == order.TargetId).Type == Model.Diplomatical.DipMsg.Types.Alliance)
                {
                    var country = _dataLayer.GetCountry(order.CountryName);
                    if (country.IsBoss)
                    {
                        _newsHandler.AddBroadNews(new Model.News(false) { text = "Страна " + order.CountryName + " исключила страну " + order.TargetCountryName + " из альянса " + country.Alliance + "!" });
                        _dataLayer.SetProperty(order.TargetCountryName, "Alliance", order.TargetCountryName);
                        int num = 0;
                        var countries = _dataLayer.GetCountries().Keys;
                       foreach (string c in countries)
                        {
                            if (_dataLayer.GetCountry(c).Alliance == country.Alliance)
                                num++;
                        }

                       if (num == 1)
                        {
                            _dataLayer.SetProperty(order.CountryName, "Alliance", order.CountryName);
                        }
                    }
                    else
                    {
                        _newsHandler.AddBroadNews(new Model.News(false) { text = "Страна " + order.CountryName + " покинула альянс " + country.Alliance + "!" });
                        _dataLayer.SetProperty(order.CountryName, "Alliance", order.CountryName);
                    }
                }

                _dataLayer.BreakContract(order.TargetId);
                return true;
            }
            catch (Exception e)
            {
                _log.Error(e.Message);
                return false;
            }
        }
    }
}
