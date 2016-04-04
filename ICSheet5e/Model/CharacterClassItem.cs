using InteractiveCharacterSheetCore;
using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ICSheet5e.Model
{
    [DataContract]
    public class CharacterClassItem
    {
        [DataMember]
        private CharacterClassType _classType;
        [DataMember]
        private int level;
        [DataMember]
        static private Dictionary<CharacterClassType, List<DefenseType>> _proficientDefenses = new Dictionary<CharacterClassType, List<DefenseType>>()
        {
            {CharacterClassType.ArcaneTrickster, new List<DefenseType>() {DefenseType.Dexterity, DefenseType.Intelligence}},
            {CharacterClassType.Barbarian, new List<DefenseType>() {DefenseType.Strength, DefenseType.Constitution}},
            {CharacterClassType.Bard, new List<DefenseType>() {DefenseType.Dexterity, DefenseType.Charisma}},
            {CharacterClassType.Cleric, new List<DefenseType>() {DefenseType.Wisdom, DefenseType.Charisma}},
            {CharacterClassType.Druid, new List<DefenseType>() {DefenseType.Intelligence, DefenseType.Wisdom}},
            {CharacterClassType.Fighter, new List<DefenseType>() {DefenseType.Strength, DefenseType.Constitution}},
            {CharacterClassType.EldritchKnight, new List<DefenseType>() {DefenseType.Strength, DefenseType.Constitution}},
            {CharacterClassType.Rogue, new List<DefenseType>() {DefenseType.Dexterity, DefenseType.Intelligence}},
            {CharacterClassType.Paladin, new List<DefenseType>() {DefenseType.Wisdom, DefenseType.Charisma}},
            {CharacterClassType.Monk, new List<DefenseType>() {DefenseType.Strength, DefenseType.Dexterity}},
            {CharacterClassType.Ranger, new List<DefenseType>() {DefenseType.Strength, DefenseType.Dexterity}},
            {CharacterClassType.Sorcerer, new List<DefenseType>() {DefenseType.Charisma, DefenseType.Constitution}},
            {CharacterClassType.Warlock, new List<DefenseType>() {DefenseType.Wisdom, DefenseType.Charisma}},
            {CharacterClassType.Wizard, new List<DefenseType>() {DefenseType.Intelligence, DefenseType.Wisdom}}
        };

        public CharacterClassItem(CharacterClassType type, int numberOfLevels)
        {
            _classType = type;
            level = numberOfLevels;
        }

        public void LevelUp()
        {
            level += 1;
        }

        public CharacterClassType ClassType { get { return _classType; } }
        public int Level { get { return level; } }
        public bool Matches(CharacterClassType otherType)
        {
            return _classType == otherType;
        }

        public IEnumerable<DefenseType> ProficientDefenses { get { return _proficientDefenses[_classType]; } }

        public override string ToString()
        {
            return string.Format("{0} {1}", _classType, level);
        }

        public Tuple<CharacterClassType, int> MovementBonus
        {
            get
            {
                if (_classType == CharacterClassType.Monk && level >= 2)
                {
                    return new Tuple<CharacterClassType, int>(_classType, 10 + 5 * (level - 2) / 4);
                }
                else if (_classType == CharacterClassType.Barbarian && level >= 5) { return new Tuple<CharacterClassType, int>(_classType, 10); }
                return new Tuple<CharacterClassType, int>(_classType, 0);
            }
        }
    }
}
