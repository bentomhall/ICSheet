using System.Collections.Generic;
using System.Runtime.Serialization;

namespace InteractiveCharacterSheetCore
{
    public enum AbilityType
    {
        Strength,
        Dexterity,
        Constitution,
        Intelligence,
        Wisdom,
        Charisma,
        None
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

    [DataContract]
    public class Ability
    {
        [DataMember] private int value;

        public int Modifier
        {
            get { return (value - 10)/2; }
        }

        public Ability(int score)
        {
            value = score;
        }

        public int Score
        {
            get { return value; }
        }
    }
}
