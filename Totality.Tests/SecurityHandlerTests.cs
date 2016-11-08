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
    public class SecurityHandlerTests
    {
        private IDataLayer _data = new DataLayer.DataLayer(null);
        private NewsHandler _newsHandler = new NewsHandler();

        [TestMethod]
        public void ShouldImproveNetwork()
        {
            MinSecurityHandler handler = new MinSecurityHandler(_newsHandler, _data, null);

            var newCountry1 = new Country(Guid.NewGuid().ToString());
            var newCountry2 = new Country(Guid.NewGuid().ToString());
            newCountry1.Money = Constants.InitialNetworkLvlUpCost;
            newCountry1.IntelligenceLvl = 100;
            newCountry2.CounterSpyLvl = 0;
            _data.AddCountry(newCountry1);
            _data.AddCountry(newCountry2);

            handler.ProcessOrder(new Order(newCountry1.Name, newCountry2.Name) { Ministery = (int)Mins.Security, OrderNum = 0, TargetMinistery = 0 });

            var countryFromData1 = _data.GetCountry(newCountry1.Name);
            var countryFromData2 = _data.GetCountry(newCountry2.Name);

            Assert.IsTrue(countryFromData1.SpyNetworks[newCountry2.Name].NetLvl == 1);
        }

        [TestMethod]
        public void ShouldAddAgents()
        {
            MinSecurityHandler handler = new MinSecurityHandler(_newsHandler, _data, null);

            var newCountry1 = new Country(Guid.NewGuid().ToString());
            var newCountry2 = new Country(Guid.NewGuid().ToString());

            newCountry1.Money = Constants.InitialAgentCost + Constants.InitialNetworkLvlUpCost;
            newCountry1.IntelligenceLvl = 100;
            newCountry2.ShadowingLvl = 0;
            newCountry2.CounterSpyLvl = 0;
            _data.AddCountry(newCountry1);
            _data.AddCountry(newCountry2);

            handler.ProcessOrder(new Order(newCountry1.Name, newCountry2.Name) { Ministery = (int)Mins.Security, OrderNum = 0, TargetMinistery = 0 });
            var countryFromData1 = _data.GetCountry(newCountry1.Name);
            countryFromData1.SpyNetworks[newCountry2.Name].NetLvl = Constants.MinAgentLvl;
            _data.UpdateCountry(countryFromData1);

            handler.ProcessOrder(new Order(newCountry1.Name, newCountry2.Name) { Ministery = (int)Mins.Security, OrderNum = 1, TargetMinistery = 0 });

            countryFromData1 = _data.GetCountry(newCountry1.Name);
            var countryFromData2 = _data.GetCountry(newCountry2.Name);

            Assert.IsTrue(countryFromData1.SpyNetworks[newCountry2.Name].Recruit[0]);
            Assert.IsTrue(countryFromData2.ForeignSpyes[0].Contains(newCountry1.Name));
        }

        [TestMethod]
        public void ShouldPurge()
        {
            MinSecurityHandler handler = new MinSecurityHandler(_newsHandler, _data, null);

            var newCountry1 = new Country(Guid.NewGuid().ToString());
            var newCountry2 = new Country(Guid.NewGuid().ToString());

            newCountry1.SpyNetworks.Add(newCountry2.Name, new Country.SpyNetwork());
            newCountry1.SpyNetworks[newCountry2.Name].Recruit[0] = true;
            newCountry1.IntelligenceLvl = 0;
            _data.AddCountry(newCountry1);

            newCountry2.ForeignSpyes[0].Add(newCountry1.Name);
            newCountry2.ShadowingLvl = 100;
            _data.AddCountry(newCountry2);

            handler.ProcessOrder(new Order(newCountry2.Name) { Ministery = (int)Mins.Security, OrderNum = 3, TargetMinistery = 0 });

            var countryFromData1 = _data.GetCountry(newCountry1.Name);
            var countryFromData2 = _data.GetCountry(newCountry2.Name);

            Assert.IsTrue(countryFromData1.SpyNetworks[newCountry2.Name].Recruit[0] == false);
            Assert.IsTrue(countryFromData2.ForeignSpyes[0].Count == 0);
            Assert.IsTrue(countryFromData2.MinsBlocks[0] != 0);
        }

        [TestMethod]
        public void ShouldEvacuateAgent()
        {
            MinSecurityHandler handler = new MinSecurityHandler(_newsHandler, _data, null);

            var newCountry1 = new Country(Guid.NewGuid().ToString());
            var newCountry2 = new Country(Guid.NewGuid().ToString());

            newCountry1.SpyNetworks.Add(newCountry2.Name, new Country.SpyNetwork());
            newCountry1.SpyNetworks[newCountry2.Name].Recruit[0] = true;
            _data.AddCountry(newCountry1);

            newCountry2.ForeignSpyes[0].Add(newCountry1.Name);
            _data.AddCountry(newCountry2);

            handler.ProcessOrder(new Order(newCountry1.Name, newCountry2.Name) { Ministery = (int)Mins.Security, OrderNum = 2, TargetMinistery = 0, Value = -1 });

            var countryFromData1 = _data.GetCountry(newCountry1.Name);
            var countryFromData2 = _data.GetCountry(newCountry2.Name);

            Assert.IsTrue(countryFromData1.SpyNetworks[newCountry2.Name].Recruit[0] == false);
            Assert.IsTrue(countryFromData2.ForeignSpyes[0].Count == 0);
        }

        [TestMethod]
        public void ShouldCounterSpyLvlUp()
        {
            MinSecurityHandler handler = new MinSecurityHandler(_newsHandler, _data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.CounterSpyLvlUpCost;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Security, OrderNum = 4 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreEqual(countryFromData.CounterSpyLvl, 1);
        }

        [TestMethod]
        public void ShouldShadowingLvlUp()
        {
            MinSecurityHandler handler = new MinSecurityHandler(_newsHandler, _data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.ShadowingLvlUpCost;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Security, OrderNum = 5 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreEqual(countryFromData.ShadowingLvl, 1);
        }

        [TestMethod]
        public void ShouldIntelligenceLvlUp()
        {
            MinSecurityHandler handler = new MinSecurityHandler(_newsHandler, _data, null);

            var newCountry = new Country(Guid.NewGuid().ToString());
            newCountry.Money = newCountry.IntelligenceLvlUpCost;
            _data.AddCountry(newCountry);

            handler.ProcessOrder(new Order(newCountry.Name, null) { Ministery = (int)Mins.Security, OrderNum = 6 });

            var countryFromData = _data.GetCountry(newCountry.Name);

            Assert.AreEqual(countryFromData.IntelligenceLvl, 1);
        }

        [TestMethod]
        public void ShouldSabotage()
        {
            MinSecurityHandler handler = new MinSecurityHandler(_newsHandler, _data, null);

            var newCountry1 = new Country(Guid.NewGuid().ToString());
            var newCountry2 = new Country(Guid.NewGuid().ToString());

            newCountry1.SpyNetworks.Add(newCountry2.Name, new Country.SpyNetwork());
            newCountry1.SpyNetworks[newCountry2.Name].Recruit[0] = true;
            _data.AddCountry(newCountry1);

            newCountry2.ForeignSpyes[0].Add(newCountry1.Name);
            _data.AddCountry(newCountry2);

            handler.ProcessOrder(new Order(newCountry1.Name, newCountry2.Name) { Ministery = (int)Mins.Security, OrderNum = 7, TargetMinistery = 0 });

            var countryFromData1 = _data.GetCountry(newCountry1.Name);
            var countryFromData2 = _data.GetCountry(newCountry2.Name);

            Assert.IsTrue(countryFromData1.SpyNetworks[newCountry2.Name].Recruit[0] == false);
            Assert.IsTrue(countryFromData2.ForeignSpyes[0].Count == 0);
            Assert.IsTrue(countryFromData2.MinsBlocks[0] != 0);
        }

    }
}
