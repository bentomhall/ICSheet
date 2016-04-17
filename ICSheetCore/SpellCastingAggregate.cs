using System.Collections.Generic;
using System.Linq;

namespace ICSheetCore
{
    internal class SpellCastingAggregate
    {
        private List<SpellcastingFeature> _features;
        private List<int> _totalSpellSlots;
        private List<int> _availableSpellSlots;
        private List<Spell> _knownSpells;
        private SpellManager _spellDB;
        
        internal SpellCastingAggregate(IEnumerable<SpellcastingFeature> spellcastingFeatures, SpellManager spellDB)
        {
            _features = spellcastingFeatures.ToList();
            _spellDB = spellDB;
            if (_features.Count == 1)
            {
                _totalSpellSlots = _features[0].SpellSlots(1).ToList();
            }
            else if (_features.All(x => x.ParticipatesInMulticlassSpellcasting))
            {
                _totalSpellSlots = SpellcastingLookup.SpellSlotsFor(SpellcastingLookup.CastingType.Full, 2).ToList(); //wrong level
            }
            else
            {
                _totalSpellSlots = new List<int>(_features[0].SpellSlots(1));
                for (var ii = 0; ii < _features.Count; ii++)
                {
                    _totalSpellSlots = _totalSpellSlots.Zip(_features[ii].SpellSlots(1), (x, y) => x + y).ToList();
                }
            }
        } 
    }
}