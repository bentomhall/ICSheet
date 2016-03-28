using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace InteractiveCharacterSheetCore
{
    [DataContract]
    public class Defense
    {
        public Defense(int value, DefenseType type)
        {
            _value = value;
            _type = type;
        }
        [DataMember]
        private int _value = 10;
        [DataMember]
        private DefenseType _type;

        public int value { get { return _value; } }
        public DefenseType type { get { return _type; } }
        public bool willHit(int attackValue)
        {
            return attackValue >= _value;
        }
    }

    public enum ResistanceType
    {
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
