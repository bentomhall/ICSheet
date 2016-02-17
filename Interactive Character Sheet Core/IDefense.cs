using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Character_Sheet_Core
{
    public class Defense
    {
        public Defense(int value, DefenseType type)
        {
            _value = value;
            _type = type;
        }
        private int _value = 10;
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
