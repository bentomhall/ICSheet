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
        private List<Spell> _knownSpells;
        private SpellManager _spellDB;
        
        internal SpellCastingAggregate(IEnumerable<ISpellcastingFeature> spellcastingFeatures, IEnumerable<int> levels, SpellManager spellDB)
        {

            _features = spellcastingFeatures.ToList();
            _spellDB = spellDB;
            setSpellSlots(levels.ToList());
        }

        private void setSpellSlots(IList<int> levels)
        {
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
        }
    }
}