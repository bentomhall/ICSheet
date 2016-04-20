using System;
using System.Linq;
using System.Collections.Generic;

namespace ICSheetCore
{
    internal class PlayerClassAggregate
    {
        private List<PlayerCharacterClassDetail> _playerClasses;
        private int _proficiencyBonus;
        private SpellCastingAggregate _spellcastingAggregate;
        private Dictionary<DefenseType, int> _proficienctDefenses = new Dictionary<DefenseType, int>()
        {
            {DefenseType.Strength, 0 },
            {DefenseType.Dexterity, 0 },
            {DefenseType.Constitution, 0 },
            {DefenseType.Intelligence, 0 },
            {DefenseType.Wisdom, 0 },
            {DefenseType.Charisma, 0 }
        };
        private int _totalLevel;

        internal PlayerClassAggregate(IEnumerable<PlayerCharacterClassDetail> classesAndLevels, SpellManager spellDB)
        {
            _playerClasses = classesAndLevels.ToList();
            _totalLevel = _playerClasses.Sum(x => x.Level);
            _proficiencyBonus = calculateProficiencyBonus();
            foreach (var defense in _playerClasses[0].ProficientDefenses)
            {
                _proficienctDefenses[defense] = _proficiencyBonus;
            }
            _spellcastingAggregate = createSpellcastingAggregate(spellDB);
        }

        internal IEnumerable<int> AvailableSpellSlots { get { return _spellcastingAggregate.AvailableSpellSlots; } }

        internal bool IsSpellcaster { get { return _spellcastingAggregate.CanCastSpells; } }

        internal string SpellsPreparedOfMax(AbilityAggregate abilities)
        {
            var spellcastingLevels = _playerClasses.Where(x => x.Spellcasting != null)
                                                   .ToDictionary(x => x.Name, 
                                                                 x => x.Level);
            return _spellcastingAggregate.PreparedSpellUtilization(abilities, spellcastingLevels);
        }

        private SpellCastingAggregate createSpellcastingAggregate(SpellManager db)
        {
            var castingFeatures = new List<ISpellcastingFeature>();
            var levels = new List<int>();
            foreach (var c in _playerClasses)
            {
                var f = c.Spellcasting;
                if (f != null)
                {
                    castingFeatures.Add(f);
                    levels.Add(c.Level);
                }
            }
            return new SpellCastingAggregate(castingFeatures, levels, db);
        }

        private int calculateProficiencyBonus()
        {
            return (_totalLevel - 1) / 4 + 2;
        }

        internal IDictionary<DefenseType, int> ProficiencyForDefenses
        {
            get { return _proficienctDefenses; }
        }

    }

    internal class ClassInformationChangedEventArgs : EventArgs
    {

    }
}