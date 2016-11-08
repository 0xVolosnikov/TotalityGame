using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Totality.Handlers.Main;
using Totality.Model.Interfaces;
using Totality.Model;
using Totality.CommonClasses;

namespace Totality.Tests
{
    [TestClass]
    public class IndustryHandlerTests
    {
        private IDataLayer _data = new DataLayer.DataLayer(null);

        [TestMethod]
        public void ShouldImproveHeavyIndustry()
        {
            MinIndustryHandler handler = new MinIndustryHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.IndustryUpgradeCost;
            newCountry.PowerHeavyIndustry = 1;
            newCountry.FinalSteel = 500;
            newCountry.FinalOil = 500;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Industry, OrderNum = 0 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.PowerHeavyIndustry, 1);
        }

        [TestMethod]
        public void ShouldImproveLightIndustry()
        {
            MinIndustryHandler handler = new MinIndustryHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.IndustryUpgradeCost;
            newCountry.PowerLightIndustry = 1;
            newCountry.FinalWood = 500;
            newCountry.FinalAgricultural = 500;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Industry, OrderNum = 1 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.PowerLightIndustry, 1);
        }

        [TestMethod]
        public void ShouldIncreaseSteel()
        {
            MinIndustryHandler handler = new MinIndustryHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.ProductionUpgradeCost;
            newCountry.ResSteel = 1;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Industry, OrderNum = 2 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.ResSteel, 1);
        }

        [TestMethod]
        public void ShouldIncreaseOil()
        {
            MinIndustryHandler handler = new MinIndustryHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.ProductionUpgradeCost;
            newCountry.ResOil = 1;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Industry, OrderNum = 3 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.ResOil, 1);
        }

        [TestMethod]
        public void ShouldIncreaseWood()
        {
            MinIndustryHandler handler = new MinIndustryHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.ProductionUpgradeCost;
            newCountry.ResWood = 1;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Industry, OrderNum = 4 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.ResWood, 1);
        }

        [TestMethod]
        public void ShouldIncreaseAgricultural()
        {
            MinIndustryHandler handler = new MinIndustryHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.ProductionUpgradeCost;
            newCountry.ResAgricultural = 1;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Industry, OrderNum = 5 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.ResAgricultural, 1);
        }
    }
}
