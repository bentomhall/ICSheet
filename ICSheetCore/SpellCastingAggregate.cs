using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSheetCore
{
    internal class SpellCastingAggregate
    {
        private List<ISpellcastingFeature> _features;
        private List<int> _totalSpellSlots;
        private List<int> _availableSpellSlots;
        private Dictionary<string, SpellBook> _spellBooks;
        private ISpellManager _spellDB;
        private List<int> _levels;
        
        internal SpellCastingAggregate(IEnumerable<ISpellcastingFeature> spellcastingFeatures, IEnumerable<int> levels, ISpellManager spellDB)
        {

            _features = spellcastingFeatures.ToList();
            _spellDB = spellDB;
            _levels = levels.ToList();
            setSpellSlots(_levels);
            setSpellbooks();
            
        }

        internal bool CanCastSpells { get { return _features.Count != 0; } }

        internal IEnumerable<int> AvailableSpellSlots
        {
            get { return _availableSpellSlots; }
        }

        internal string PreparedSpellUtilization(IAbilityDataSource abilities, IDictionary<string, int> levels)
        {
            var s = new System.Text.StringBuilder();
            foreach (KeyValuePair<string, int> entry in levels)
            {
                var f = _features.SingleOrDefault(x => x.Name == entry.Key);
                if (f == null) { continue; } //not a casting class
                var prepared = _spellBooks[entry.Key].PreparedSpellCount;
                var max = f.SpellsPrepared(entry.Value, abilities.AbilityModifierFor(f.CastingAbility));
                s.AppendLine($"{f.Name}: {prepared} / {max}");
            }
            return s.ToString();
        }

        private void setSpellbooks()
        {
            _spellBooks = new Dictionary<string, SpellBook>();
            foreach (var f in _features)
            {
                _spellBooks[f.SpellBookName] = new SpellBook(_spellDB, f.SpellBookName, f.IsPreparedCaster);
            }
        }

        private void setSpellSlots(IList<int> levels)
        {
            if (_features.Count == 0) { _totalSpellSlots = Enumerable.Repeat(0, 9).ToList(); }
            else if (_features.Count == 1)
            {
                _totalSpellSlots = _features[0].SpellSlots(levels[0]).ToList();
            }
            else
            {
                var casterLevel = 0;
                for (var ii = 0; ii < _features.Count; ii++)
                {
                    var current = _features[ii];
                    
                    if (current.CasterType == SpellcastingLookup.CastingType.Warlock)
                    {
                        _totalSpellSlots = _totalSpellSlots.Zip(current.SpellSlots(levels[ii]), (x, y) => x + y).ToList();
                    }
                    else if (current.CasterType == SpellcastingLookup.CastingType.Full)
                    {
                        casterLevel += levels[ii];
                    }
                    else if (current.CasterType == SpellcastingLookup.CastingType.Half)
                    {
                        casterLevel += levels[ii] / 2;
                    }
                    else if (current.CasterType == SpellcastingLookup.CastingType.Martial)
                    {
                        casterLevel += levels[ii] / 3;
                    }
                }
                _totalSpellSlots = _totalSpellSlots.Zip(SpellcastingLookup.SpellSlotsFor(SpellcastingLookup.CastingType.Full, casterLevel), (x, y) => x + y).ToList();
            }
            _availableSpellSlots = new List<int>(_totalSpellSlots); //copy constructor
        }

        internal void IncreaseLevel(string name)
        {
            var indx = _features.FindIndex(x => x.SpellBookName == name);
            if (indx >= 0)
            {
                _levels[indx] += 1;
                setSpellSlots(_levels);
            }
        }

        internal void AddSpellcasting(IFeature feature)
        {
            if (feature == null) { return; }
            var s = feature as ISpellcastingFeature;
            if (s == null) { throw new ArgumentException($"Must be spellcasting feature: Got {feature.Name}"); }
            _spellBooks[s.SpellBookName] = new SpellBook(_spellDB, s.SpellBookName, s.IsPreparedCaster);
            _features.Add(s);
            _levels.Add(1);
            setSpellSlots(_levels);
        }

        internal void RegainSpellSlot(int level, int numberOfSlots)
        {
            var current = _availableSpellSlots[level - 1];
            var max = _totalSpellSlots[level - 1];
            if (current + numberOfSlots > max)
            {
                _availableSpellSlots[level - 1] = max;
            }
            else { _availableSpellSlots[level - 1] += numberOfSlots; }
        }

        internal void ResetAllSlots()
        {
            _availableSpellSlots = new List<int>(_totalSpellSlots);
        }

        internal void UseSpellSlot(int level)
        {
            _availableSpellSlots[level - 1] = Math.Max(_availableSpellSlots[level - 1] - 1, 0);
        }

        internal IEnumerable<Spell> PreparedSpells
        {
            get
            {
                var spells = new List<Spell>();
                foreach (KeyValuePair<string, SpellBook> entry in _spellBooks)
                {
                    spells.AddRange(entry.Value.AllPreparedSpells);
                }
                return spells;
            }
        }

        internal IReadOnlyDictionary<string, int> SpellAttackBonusesWith(IAbilityDataSource abilities, int proficiency)
        {
            var output = new Dictionary<string, int>();
            foreach (var f in _features)
            {
                var name = f.SpellBookName;
                var abilityMod = abilities.AbilityModifierFor(f.CastingAbility);
                output[name] = abilityMod + proficiency;
            }
            return output;
        }

        internal IReadOnlyDictionary<string, int> SpellDCsWith(IAbilityDataSource abilities, int proficiency)
        {
            var output = new Dictionary<string, int>();
            foreach (var f in _features)
            {
                var name = f.SpellBookName;
                var abilityMod = abilities.AbilityModifierFor(f.CastingAbility);
                output[name] = 8 + abilityMod + proficiency;
            }
            return output;
        }

        internal void LearnSpell(string spellName, string asClass, bool isBonusSpell)
        {
            try
            {
                var sb = _spellBooks[asClass];
                sb.AddKnownSpell(spellName, isBonusSpell);
            }
            catch (KeyNotFoundException) { return; }
        }

        internal void UnlearnSpell(string spellName, string asClass)
        {
            try
            {
                var sb = _spellBooks[asClass];
                sb.UnlearnSpell(spellName);
            }
            catch (KeyNotFoundException) { return; }
        }

        internal void PrepareSpell(string spellName, string asClass, bool asBonus)
        {
            try
            {
                var sb = _spellBooks[asClass];
                sb.PrepareSpell(spellName, asBonus);
            }
            catch (KeyNotFoundException) { return; }
        }

        internal void UnprepareSpell(string spellName, string asClass)
        {
            try
            {
                var sb = _spellBooks[asClass];
                sb.UnprepareSpell(spellName);
            }
            catch (KeyNotFoundException) { return; }
        }

        internal IEnumerable<Spell> KnownSpells
        {
            get
            {
                var spells = new List<Spell>();
                foreach (var sb in _spellBooks)
                {
                    spells.AddRange(sb.Value.AllKnownSpells);
                }
                return spells;
            }
        }

    }
}