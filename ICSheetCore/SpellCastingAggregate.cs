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
        private SpellManager _spellDB;
        
        internal SpellCastingAggregate(IEnumerable<ISpellcastingFeature> spellcastingFeatures, IEnumerable<int> levels, SpellManager spellDB)
        {

            _features = spellcastingFeatures.ToList();
            _spellDB = spellDB;
            setSpellSlots(levels.ToList());
            setSpellbooks();
        }



        internal bool CanCastSpells { get { return _features.Count != 0; } }

        internal IEnumerable<int> AvailableSpellSlots
        {
            get { return _availableSpellSlots; }
        }

        internal string PreparedSpellUtilization(AbilityAggregate abilities, IDictionary<string, int> levels)
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
                _spellBooks[f.Name] = new SpellBook(_spellDB, f.Name, f.IsPreparedCaster);
            }
        }

        private void setSpellSlots(IList<int> levels)
        {
            if (_features.Count == 0) { _totalSpellSlots = Enumerable.Repeat(0, 9).ToList(); }
            if (_features.Count == 1)
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
    }
}