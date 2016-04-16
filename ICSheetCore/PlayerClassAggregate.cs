using System;
using System.Linq;
using System.Collections.Generic;

namespace ICSheetCore
{
    internal class PlayerClassAggregate
    {
        private List<CharacterClassItem> _playerClasses;
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

        internal PlayerClassAggregate(IEnumerable<CharacterClassItem> classesAndLevels)
        {
            _playerClasses = new List<CharacterClassItem>(classesAndLevels);
            _totalLevel = _playerClasses.Sum(x => x.Level);
            _proficiencyBonus = calculateProficiencyBonus();
            foreach (var defense in _playerClasses[0].ProficientDefenses)
            {
                _proficienctDefenses[defense] = _proficiencyBonus;
            }
            
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