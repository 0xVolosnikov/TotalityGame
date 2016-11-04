using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Totality.Handlers.Main;
using Totality.Model.Interfaces;
using Totality.Model;
using Totality.CommonClasses;

namespace Totality.Tests
{
    [TestClass]
    public class InnerHandlerTests
    {
        private IDataLayer _data = new DataLayer.DataLayer(null);

        [TestMethod]
        public void ShouldSuppresRiot()
        {
            MinInnerHandler handler = new MinInnerHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.IsRiot = true;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Inner, OrderNum = 0 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreEqual(countryFromData.IsRiot, false);
        }

        [TestMethod]
        public void ShouldRepress()
        {
            MinInnerHandler handler = new MinInnerHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Inner, OrderNum = 1 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreEqual(countryFromData.IsRepressed, true);
        }

        [TestMethod]
        public void ShouldEndRepress()
        {
            MinInnerHandler handler = new MinInnerHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.IsRepressed = true;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Inner, OrderNum = 2 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreEqual(countryFromData.IsRepressed, false);
        }


        [TestMethod]
        public void ShouldLvlUp()
        {
            MinInnerHandler handler = new MinInnerHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.InnerLvlUpCost;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Inner, OrderNum = 3 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.InnerLvl, 1);
        }
    }
}
