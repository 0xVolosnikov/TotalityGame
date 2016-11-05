using System;
using System.Text;
using System.Collections.Generic;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Totality.Handlers.Main;
using Totality.Model;
using Totality.CommonClasses;
using Totality.Model.Interfaces;

namespace Totality.Tests
{

    [TestClass]
    public class ScienceHandlerTests
    {
        private IDataLayer _data = new DataLayer.DataLayer(null);

        [TestMethod]
        public void ShouldExtractUp()
        {
            MinScienceHandler handler = new MinScienceHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.ExtractScLvlUpCost;
            newCountry.ExtractExperience = newCountry.ExtractScLvlUpExp;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Science, OrderNum = 0 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.ExtractScienceLvl, 1);
        }

        [TestMethod]
        public void ShouldHeavyUp()
        {
            MinScienceHandler handler = new MinScienceHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.HeavyScLvlUpCost;
            newCountry.HeavyExperience = newCountry.HeavyScLvlUpExp;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Science, OrderNum = 1 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.HeavyScienceLvl, 1);
        }

        [TestMethod]
        public void ShouldLightUp()
        {
            MinScienceHandler handler = new MinScienceHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.LightScLvlUpCost;
            newCountry.LightExperience = newCountry.LightScLvlUpExp;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Science, OrderNum = 2 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.LightScienceLvl, 1);
        }

        [TestMethod]
        public void ShouldMilitaryUp()
        {
            MinScienceHandler handler = new MinScienceHandler(_data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.MilitaryScLvlUpCost;
            newCountry.MilitaryExperience = newCountry.MilitaryScLvlUpExp;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Science, OrderNum = 3 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreNotEqual(countryFromData.MilitaryScienceLvl, 1);
        }
    }
}
