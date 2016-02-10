using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Character_Sheet_Core
{
    public enum AbilityType
    {
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma
    };

    public static class AbilityTypeExtension
    {
        private static Dictionary<AbilityType, string> abilityNameMap = new Dictionary<AbilityType, string>()
        {
            { AbilityType.Strength, "STR" },
            { AbilityType.Dexterity, "DEX" },
            { AbilityType.Constitution, "CON"},
            { AbilityType.Intelligence, "INT"},
            { AbilityType.Wisdom, "WIS"},
            { AbilityType.Charisma, "CHA"}
        };

        public static string NameForAbility(this AbilityType ability)
        {
            return abilityNameMap[ability];
        } 
    }

    public class Ability
    {
        private AbilityType name;
        private int value;

        public int modifier
        {
            get { return (int)Math.Truncate((value - 10.0) / 2.0); }
        }

        public Ability(AbilityType type, int score)
        {
            name = type;
            value = score;
        }
    }
}
