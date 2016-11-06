using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Totality.Model.Interfaces;
using Totality.Handlers.Main;
using Totality.Model;
using Totality.CommonClasses;

namespace Totality.Tests
{
    [TestClass]
    public class FinanceHandlerTests
    {
        private IDataLayer _data = new DataLayer.DataLayer(null);

        [TestMethod]
        public void ShouldChangeTaxes()
        {
            MinFinanceHandler handler = new MinFinanceHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.TaxesLvl = 0;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Finance, OrderNum = 0, Value = 100 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.TaxesLvl, 0);
        }

        [TestMethod]
        public void ShouldPurchaseCurrency()
        {
            MinFinanceHandler handler = new MinFinanceHandler(_data, null);

            var newCountry1 = new Country(Guid.NewGuid().ToString());
            newCountry1.Money = 1;
            _data.AddCountry(newCountry1);

            var newCountry2 = new Country(Guid.NewGuid().ToString());
            _data.AddCountry(newCountry2);

            handler.ProcessOrder(new Order(newCountry1.Name, newCountry2.Name) { Ministery = (int)Mins.Finance, OrderNum = 1, Count = 1 });

            var countryFromData1 = _data.GetCountry(newCountry1.Name);

            Assert.AreEqual(countryFromData1.CurrencyAccounts[newCountry2.Name], 1);
        }

        [TestMethod]
        public void ShouldSellCurrency()
        {
            MinFinanceHandler handler = new MinFinanceHandler(_data, null);

            var newCountry1 = new Country(Guid.NewGuid().ToString());
            _data.AddCountry(newCountry1);

            var newCountry2 = new Country(Guid.NewGuid().ToString());
            newCountry2.CurrencyAccounts.Add(newCountry1.Name, 1);
            newCountry2.Money = 0;
            _data.AddCountry(newCountry2);

            handler.ProcessOrder(new Order(newCountry2.Name, newCountry1.Name) { Ministery = (int)Mins.Finance, OrderNum = 2, Count = 1 });

            var countryFromData = _data.GetCountry(newCountry2.Name);

            Assert.AreEqual(countryFromData.CurrencyAccounts[newCountry1.Name], 0);
            Assert.AreNotEqual(countryFromData.Money, 0);
        }

    }
}
