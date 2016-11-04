using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using Totality.Handlers.Main;
using Totality.Model.Interfaces;

namespace Totality.Tests
{
    [TestClass]
    public class MilitaryHandlerTests
    {
        [TestMethod]
        public void ShouldMobilize()
        {
            IDataLayer _data = new DataLayer.DataLayer(null);
            MinMilitaryHandler handler = new MinMilitaryHandler(_data, null);
        }
    }
}
