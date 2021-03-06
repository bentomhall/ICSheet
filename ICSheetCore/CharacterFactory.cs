﻿using System;
using System.Collections.Generic;
using System.Linq;
using ICSheetCore.Data;

namespace ICSheetCore
{
    /// <summary>
    /// Creates a player character.
    /// </summary>
    public class CharacterFactory
    {
        private string _characterName;

        private SpellManager _spellDB;
        private XMLFeatureFactory _featureFactory;
        private PCRace _race;
        private PlayerClassAggregate _classes;
        private string _alignment;
        private string _background;
        private int _weight;
        private string _deity;
        private string _height;

        /// <summary>
        /// Creates an instance of a PlayerCharacterFactory
        /// </summary>
        /// <param name="name"></param>
        /// <param name="spellDB"></param>
        /// <param name="featureFactory"></param>
        public CharacterFactory(string name, SpellManager spellDB, XMLFeatureFactory featureFactory)
        {
            _characterName = name;
            _spellDB = spellDB;
            _featureFactory = featureFactory;
        }

        /// <summary>
        /// Required. Assigns the new character a race with the appropriate racial features.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="subraceName"></param>
        public void AssignRace(string baseName, string subraceName)
        {
            var features = _featureFactory.ExtractRacialFeatures(baseName, subraceName);
            _race = new PCRace(baseName, subraceName, features);
        }

        /// <summary>
        /// Required. Assigns the classes and levels to the character along with the appropriate class features.
        /// </summary>
        /// <param name="classesAndLevels"></param>
        public void AssignClassLevels(IDictionary<string, int> classesAndLevels)
        {
            var classes = new List<PlayerCharacterClassDetail>();
            foreach (KeyValuePair<string, int> entry in classesAndLevels)
            {
                var f = _featureFactory.ExtractFeaturesFor(entry.Key);
                classes.Add(new PlayerCharacterClassDetail(entry.Key, entry.Value, f));
            }
            _classes = new PlayerClassAggregate(classes, _spellDB);
        }

        /// <summary>
        /// Optional.
        /// </summary>
        /// <param name="value"></param>
        public void AssignBackground(string value)
        {
            _background = value;
        }

        /// <summary>
        /// Optional
        /// </summary>
        /// <param name="value"></param>
        public void AssignAlignment(string value)
        {
            _alignment = value;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="weight"></param>
        public void AssignWeight(int weight)
        {
            _weight = weight;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="deity"></param>
        public void AssignDeity(string deity)
        {
            _deity = deity;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="height"></param>
        public void AssignHeight(string height)
        {
            _height = height;
        }


        /// <summary>
        /// Creates a new PlayerCharacter instance. Throws InvalidOperationException if AssignRace and AssignClassLevels not called first.
        /// </summary>
        /// <param name="info">Can be null</param>
        /// <returns></returns>
        public PlayerCharacter ToPlayerCharacter(CharacterRPInformation info)
        {
            if (_race == null || _classes ==  null)
            {
                throw new InvalidOperationException("Must set race and classes before building");
            }
            if (info == null)
            {
                var newCharInfo = makeCharacterInfo();
                return new PlayerCharacter(newCharInfo, _race, _classes);
            }
            else { return new PlayerCharacter(info, _race, _classes); }
        }

        private CharacterRPInformation makeCharacterInfo()
        {
            return new CharacterRPInformation(_characterName, _alignment ?? "", _weight, _height ?? "5' 0\"", _background ?? "", _deity ?? "");
        }

        #region Deserialization
        /// <summary>
        /// Replaces ToPlayerCharacter for restoring from serialization. No preconditions--the needed information is constructed from the stored data.
        /// </summary>
        /// <param name="dataObject"></param>
        /// <returns></returns>
        public PlayerCharacter BuildFromStoredData(CharacterData dataObject)
        {
            AssignRace(dataObject.RaceInformation.Item1, dataObject.RaceInformation.Item2);
            AssignClassLevels(dataObject.ClassLevelInformation);
            _alignment = dataObject.Alignment;
            _background = dataObject.Background;

            PlayerCharacter c;
            if (dataObject.Information == null)
            {
                c = ToPlayerCharacter(null);
                c.Notes = dataObject.Notes;
            }
            else
            {
                c = ToPlayerCharacter(dataObject.Information);
            }
            c.Experience = dataObject.Experience;
            setAbilityScores(c, dataObject);
            setInventory(c, dataObject);
            setFeatures(c, dataObject);
            setHealth(c, dataObject);
            setDefenseOverrides(c, dataObject);
            setSkillProficiencies(c, dataObject);
            setSubclasses(c, dataObject);
            if (c.IsSpellcaster)
            {
                setSpells(c, dataObject);
                setSpellSlots(c, dataObject);
            }
            return c;
        }

        private void setSpellSlots(PlayerCharacter c, CharacterData dataObject)
        {
            var savedSlots = dataObject.CurrentSpellSlots.ToList();
            var maxSlots = c.SpellSlots.ToList();
            for (var ii = 0; ii < 9; ii++)
            {
                if (maxSlots[ii] == 0) { continue; } //no point in working when there are no slots for that level. Single-caster warlocks only have one spell level, so can't break the loop here.
                while (maxSlots[ii] > savedSlots[ii])
                {
                    c.UseSpellSlot(ii + 1);
                    maxSlots[ii] -= 1;
                } //slots are 1 indexed, list is 0 indexed.
            }
        }

        private void setSkillProficiencies(PlayerCharacter c, CharacterData dataObject)
        {
            c.SkillProficiencies = new Dictionary<string, ProficiencyType>();
            foreach (var entry in dataObject.Skills)
            {
                c.SkillProficiencies[entry.Key] = entry.Value;
            }
        }

        private void setDefenseOverrides(PlayerCharacter c, CharacterData dataObject)
        {
            foreach (var entry in dataObject.DefenseOverrides)
            {
                if (entry.Key == DefenseType.Armor) { c.ArmorClassOverride = entry.Value; }
                else { c.SetNonACDefenseOverride(entry.Key, entry.Value); }
            }
        }

        private void setHealth(PlayerCharacter c, CharacterData dataObject)
        {
            c.MaxHealth = dataObject.HealthInformation.Item2;
            c.HealDamage(dataObject.HealthInformation.Item1);
            c.AddTHP(dataObject.HealthInformation.Item3);
        }

        private void setSpells(PlayerCharacter c, CharacterData dataObject)
        {
            var spellcastingClass = c.SpellAttackBonuses.Keys.First();
            foreach (var sp in dataObject.KnownSpells)
            {
                c.Learn(sp.Name, spellcastingClass, false);
                if (sp.IsPrepared) { c.Prepare(sp.Name, spellcastingClass, sp.IsBonusSpell); }
            }
        }

        private void setFeatures(PlayerCharacter c, CharacterData dataObject)
        {
            foreach (var f in dataObject.CustomFeatures)
            {
                c.AddFeature(f);
            }
        }

        private void setInventory(PlayerCharacter c, CharacterData dataObject)
        {
            foreach (var item in dataObject.Items)
            {
                c.AddItemToInventory(item);
            }
            foreach (var entry in dataObject.EquippedItems)
            {
                c.Equip(entry.Value);
            }
            c.DoGoldTransaction(dataObject.Cash.Total);
        }

        private void setAbilityScores(PlayerCharacter pc, CharacterData data)
        {
            foreach (var entry in data.AbilityScores)
            {
                pc.ModifyAbilityScore(entry.Key, entry.Value);
            }
        }

        private void setSubclasses(PlayerCharacter pc, CharacterData data)
        {
            if (data.Subclasses == null) { return; }
            foreach (var entry in data.Subclasses)
            {
                var features = _featureFactory.ExtractSubclassFeaturesFor(entry.Value);
                pc.AddSubclass(entry.Key, entry.Value, features);
            }
        }
        #endregion
    }
}
