using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ICSheetCore
{
    [DataContract]
    public class Defense
    {
        public Defense(int value, DefenseType type)
        {
            _baseValue = value;
            _type = type;
        }

        public Defense(int baseValue, int proficiency, int adjustment, DefenseType type)
        {
            _baseValue = baseValue;
            _proficiency = proficiency;
            _type = type;
            _defenseAdjustment = adjustment;
        }

        [DataMember]
        private int _value = 10;
        [DataMember]
        private DefenseType _type;
        [DataMember]
        private int _baseValue = 10;
        [DataMember]
        private int _proficiency = 0;
        [DataMember]
        private int _defenseAdjustment = 0;

        public int Value { get { return _baseValue + _proficiency + _defenseAdjustment; } }
        public DefenseType TypeOfDefense { get { return _type; } }
        public int BaseValue { get { return _baseValue; } }
        public int Proficiency { get { return _proficiency; } }
        public int Adjustment { get { return _defenseAdjustment; } }
    }

    public enum ResistanceType
    {
        None
    }

    public enum DefenseType
    {
        Armor,
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma
    }
}
