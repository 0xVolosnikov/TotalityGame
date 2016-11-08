using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Totality.Handlers.Main;
using Totality.Model.Interfaces;
using Totality.Model;
using Totality.CommonClasses;
using Totality.Handlers.News;

namespace Totality.Tests
{
    [TestClass]
    public class PremierHandlerTests
    {
        private IDataLayer _data = new DataLayer.DataLayer(null);
        private NewsHandler _newsHandler = new NewsHandler();

        [TestMethod]
        public void ShouldReorganize()
        {
            MinPremierHandler handler = new MinPremierHandler(_newsHandler, _data, null);

            var newCountry1 = new Country(Guid.NewGuid().ToString());
            var newCountry2 = new Country(Guid.NewGuid().ToString());

            newCountry1.SpyNetworks.Add(newCountry2.Name, new Country.SpyNetwork());
            newCountry1.SpyNetworks[newCountry2.Name].Recruit[0] = true;
            _data.AddCountry(newCountry1);

            newCountry2.ForeignSpyes[0].Add(newCountry1.Name);
            _data.AddCountry(newCountry2);

            handler.ProcessOrder(new Order(newCountry2.Name) { Ministery = (int)Mins.Premier, OrderNum = 0, TargetMinistery = 0 });

            var countryFromData1 = _data.GetCountry(newCountry1.Name);
            var countryFromData2 = _data.GetCountry(newCountry2.Name);

            Assert.IsTrue(countryFromData1.SpyNetworks[newCountry2.Name].Recruit[0] == false);
            Assert.IsTrue(countryFromData2.ForeignSpyes[0].Count == 0);
        }

        [TestMethod]
        public void ShouldLvlUp()
        {
            MinPremierHandler handler = new MinPremierHandler(_newsHandler, _data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.PremierLvlUpCost;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Premier, OrderNum = 1 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.PremierLvl, 1);
        }

        [TestMethod]
        public void ShouldAlert()
        {
            MinPremierHandler handler = new MinPremierHandler(_newsHandler, _data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Premier, OrderNum = 2 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.IsAlerted, false);
        }
    }
}
