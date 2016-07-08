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


        private PlayerCharacter buildTestCharacter()
        {
            var builder = new CharacterFactory("Test Character", new SpellManagerStub(), new FeatureFactoryStub());
            builder.AssignRace("TestRace", "Subrace TestRace");
            builder.AssignClassLevels(new Dictionary<string, int> { { "Test", 1 } });
            return builder.ToPlayerCharacter(null);
        }

        [TestMethod]
        public void TestACOnChangingAttributes()
        {
            var c = buildTestCharacter();
            var a = new ArmorItem("Test", 0.0, 0.0, true, "", ArmorType.Light, 0);
            c.AddItemToInventory(a);
            c.Equip(a);
            var ac_old = c.ArmorClassBonus;
            c.ModifyAbilityScore(AbilityType.Strength, 14);
            Assert.AreEqual(c.ArmorClassBonus, ac_old);
            c.ModifyAbilityScore(AbilityType.Dexterity, 14);
            Assert.AreEqual(c.ArmorClassBonus, ac_old + 2);
            c.Equip(new ArmorItem("Shield", 0, 0, true, "", ArmorType.Shield, 0));
            Assert.AreEqual(c.ArmorClassBonus, ac_old + 4);
            c.ModifyAbilityScore(AbilityType.Charisma, 12);
            Assert.AreEqual(c.ArmorClassBonus, ac_old + 4);
        }
    }
}
