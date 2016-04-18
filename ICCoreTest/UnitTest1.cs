using System;
using Microsoft.VisualStudio.TestTools.UnitTesting;
using ICSheetCore;
using System.Collections.Generic;

namespace ICCoreTest
{
    [TestClass]
    public class UnitTest1
    {
        [TestMethod]
        public void TestRacialBaseMovement()
        {
            var r1 = new RaceFeature("Speed", "35");
            var r2 = new RaceFeature("Speed", "30a");
            var rce = new PCRace("test", null, new List<IFeature>() { r1 });
            var rce2 = new PCRace("test", "test", new List<IFeature>() { r2 });
            Assert.AreEqual(rce.BaseMovement, 35);
            Assert.AreEqual(rce2.BaseMovement, 30);
        }
    }
}
