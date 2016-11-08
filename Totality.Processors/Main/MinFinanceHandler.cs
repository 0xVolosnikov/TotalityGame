using System;
using System.Collections.Generic;
using Totality.CommonClasses;
using Totality.Model;
using Totality.Model.Interfaces;

namespace Totality.Handlers.Main
{
    public class MinFinanceHandler : AbstractHandler, IMinisteryHandler
    {
        private enum Orders { ChangeTaxes , PurchaseCurrency, SellCurrency, CurrencyInfusion }

        public MinFinanceHandler(IDataLayer dataLayer, ILogger logger) : base(dataLayer, logger)
        {
        }

        public bool ProcessOrder(Order order)
        {
            switch (order.OrderNum)
            {
                case (int)Orders.ChangeTaxes: return ChangeTaxes(order);

                case (int)Orders.PurchaseCurrency: return PurchaseCurrency(order);

                case (int)Orders.SellCurrency: return SellCurrency(order);

                case (int)Orders.CurrencyInfusion: return CurrencyInfusion(order);

                default: throw new ArgumentException("Order " + order + " not found in " + typeof(MinPremierHandler));
            }
        }

        private bool ChangeTaxes(Order order)
        {
            if (order.Value > 100)
                return false;

            _dataLayer.SetProperty(order.CountryName, "TaxesLvl", order.Value);
            return true;
        }

        private bool PurchaseCurrency(Order order)
        {
            var ourDemand = (long)_dataLayer.GetProperty(order.CountryName, "NationalCurrencyDemand");
            var theirDemand = (long)_dataLayer.GetProperty(order.TargetCountryName, "NationalCurrencyDemand");

            var ourQuontityOnStock = _dataLayer.GetCurrencyOnStock(order.CountryName);
            var theirQuontityOnStock = _dataLayer.GetCurrencyOnStock(order.TargetCountryName);

            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            var exchangeCost = FinancialTools.GetExchangeCost(order.Count, ourDemand, theirDemand, ourQuontityOnStock, theirQuontityOnStock);

            if (money < exchangeCost)
                return false;

            money -= exchangeCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);

            ourQuontityOnStock += exchangeCost;
            _dataLayer.SetCurrencyOnStock(order.CountryName, ourQuontityOnStock);

            theirQuontityOnStock -= order.Count;
            _dataLayer.SetCurrencyOnStock(order.TargetCountryName, theirQuontityOnStock);

            var ourAccounts = (Dictionary<string, long>)_dataLayer.GetProperty(order.CountryName, "CurrencyAccounts");
            if (!ourAccounts.ContainsKey(order.TargetCountryName))
                ourAccounts.Add(order.TargetCountryName, order.Count);
            else
                ourAccounts[order.TargetCountryName] += order.Count;
            _dataLayer.SetProperty(order.CountryName, "CurrencyAccounts", ourAccounts);

            return true;
        }

        private bool SellCurrency(Order order)
        {
            var ourDemand = (long)_dataLayer.GetProperty(order.CountryName, "NationalCurrencyDemand");
            var theirDemand = (long)_dataLayer.GetProperty(order.TargetCountryName, "NationalCurrencyDemand");

            var ourQuontityOnStock = _dataLayer.GetCurrencyOnStock(order.CountryName);
            var theirQuontityOnStock = _dataLayer.GetCurrencyOnStock(order.TargetCountryName);


            var exchangeCost = FinancialTools.GetExchangeCost(order.Count, theirDemand, ourDemand, theirQuontityOnStock, ourQuontityOnStock );
            var ourAccounts = (Dictionary<string, long>)_dataLayer.GetProperty(order.CountryName, "CurrencyAccounts");

            if (!ourAccounts.ContainsKey(order.TargetCountryName) ||  ourAccounts[order.TargetCountryName] < order.Count)
                return false;

            ourAccounts[order.TargetCountryName] -= order.Count;
            _dataLayer.SetProperty(order.CountryName, "CurrencyAccounts", ourAccounts);

            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");
            money += exchangeCost;
            _dataLayer.SetProperty(order.CountryName, "Money", money);

            ourQuontityOnStock -= exchangeCost;
            _dataLayer.SetCurrencyOnStock(order.CountryName, ourQuontityOnStock);

            theirQuontityOnStock += order.Count;
            _dataLayer.SetCurrencyOnStock(order.TargetCountryName, theirQuontityOnStock);

            return true;
        }

        private bool CurrencyInfusion(Order order)
        {
            var money = (long)_dataLayer.GetProperty(order.CountryName, "Money");

            if (money < order.Count)
                return false;

            money -= order.Count;
            _dataLayer.SetProperty(order.CountryName, "Money", money);

            var ourCurrencyOnStock = _dataLayer.GetCurrencyOnStock(order.CountryName);
            ourCurrencyOnStock += order.Count;
            _dataLayer.SetCurrencyOnStock(order.CountryName, ourCurrencyOnStock);
            return true;
        }
    }
}
