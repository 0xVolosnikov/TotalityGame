using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Totality.Model;

namespace Totality.Tests
{
    [TestClass]
    public class DataLayerTests
    {
        private DataLayer.DataLayer _data = new DataLayer.DataLayer(null);

        [TestMethod]
        public void ShouldAddCountry()
        {
            var newCountry = new Country(Guid.NewGuid().ToString());

            _data.AddCountry(newCountry);

            var CountryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreEqual(CountryFromData, newCountry);
        }
    }
}
