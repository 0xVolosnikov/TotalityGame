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
    public class MilitaryHandlerTests
    {
        private IDataLayer _data = new DataLayer.DataLayer(null);
        private NewsHandler _newsHandler = new NewsHandler();

        [TestMethod]
        public void ShouldMobilize()
        {         
            MinMilitaryHandler handler = new MinMilitaryHandler(_newsHandler, _data, null, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null){ Ministery = (int)Mins.Military, OrderNum = 0 } );

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.IsMobilized, false);
        }

        [TestMethod]
        public void ShouldMakeNukes()
        {
            MinMilitaryHandler handler = new MinMilitaryHandler(_newsHandler, _data,null, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = Constants.NukeCost;
            newCountry.ResUranus = Constants.NukeUranusCost;
            newCountry.FinalHeavyIndustry = Constants.NukeHeavyPower;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null){ Ministery = (int)Mins.Military, OrderNum = 3, Count = 1 } );

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.NukesCount, 0);

        }

        [TestMethod]
        public void ShouldMakeMissiles()
        {
            MinMilitaryHandler handler = new MinMilitaryHandler(_newsHandler, _data, null, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = Constants.MissileCost;
            newCountry.FinalHeavyIndustry = Constants.MissileHeavyPower;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Military, OrderNum = 4, Count = 1 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.MissilesCount, 0);

        }

        [TestMethod]
        public void ShouldIncreaseUranium()
        {
            MinMilitaryHandler handler = new MinMilitaryHandler(_newsHandler, _data, null, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.ProductionUpgradeCost;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Military, OrderNum = 2 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.ProdUranus, Constants.InitialUranusProd);
        }

        [TestMethod]
        public void ShouldStartWar()
        {
            MinMilitaryHandler handler = new MinMilitaryHandler(_newsHandler, _data, null, null);

            var newCountry1 = new Country(Guid.NewGuid().ToString());
            var newCountry2 = new Country(Guid.NewGuid().ToString());
            _data.AddCountry(newCountry1);
            _data.AddCountry(newCountry2);

            handler.ProcessOrder(new Order(newCountry1.Name, newCountry2.Name) { Ministery = (int)Mins.Military, OrderNum = 6 });

            var countryFromData1 = _data.GetCountry(newCountry1.Name);
            var countryFromData2 = _data.GetCountry(newCountry2.Name);

            Assert.IsTrue(countryFromData1.WarList.Contains(countryFromData2.Name));
            Assert.IsTrue(countryFromData2.WarList.Contains(countryFromData1.Name));

        }
    }
}
