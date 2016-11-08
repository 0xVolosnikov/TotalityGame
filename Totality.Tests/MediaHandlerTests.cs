using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Totality.Model.Interfaces;
using Totality.Handlers.Main;
using Totality.Model;
using Totality.CommonClasses;
using Totality.Handlers.News;

namespace Totality.Tests
{
    [TestClass]
    public class MediaHandlerTests
    {
        private IDataLayer _data = new DataLayer.DataLayer(null);
        private NewsHandler _newsHandler = new NewsHandler();

        [TestMethod]
        public void ShouldChangePropDirection()
        {
            MinMediaHandler handler = new MinMediaHandler(_newsHandler, _data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, newCountry.Name) { Ministery = (int)Mins.Media, OrderNum = 0, Value = 1 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreEqual(countryFromData.MassMedia[countryFromData.Name], 1);
        }
    }
}
