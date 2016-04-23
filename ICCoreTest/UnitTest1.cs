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

        [TestMethod]
        public void TestAddMoney()
        {
            var m = new Money();
            m.Add(1);
            Assert.AreEqual(m.Gold, 1);
            m.Add(0.5m);
            Assert.AreEqual(m.Silver, 5);
            m.Add(12.34m);
            Assert.AreEqual(m.Platinum, 1);
            Assert.AreEqual(m.Gold, 3);
            Assert.AreEqual(m.Silver, 8);
            Assert.AreEqual(m.Copper, 4);
        }

        [TestMethod]
        public void TestRemoveMoney()
        {
            var m = new Money();
            m.Add(12.34m);
            m.Remove(0.5m);
            Assert.AreEqual(m.Gold, 1);
            Assert.AreEqual(m.Silver, 8);
            Assert.AreEqual(m.Copper, 4);
            Assert.AreEqual(m.Platinum, 1);

        }

        [TestMethod]
        [ExpectedException(typeof(OutOfMoneyExeption), "More money was removed than was present.")]
        public void TestRemoveTooMuchCash()
        {
            var m = new Money();
            m.Add(12.34m);
            m.Remove(50m);
        }
    }
}
