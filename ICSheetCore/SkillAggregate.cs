using System.Collections.Generic;
namespace ICSheetCore
{
    internal class SkillAggregate
    {
        static private Dictionary<string, AbilityType> _skillAbilityMap5e = new Dictionary<string, AbilityType>()
        {
            { "Acrobatics", AbilityType.Dexterity },
            { "Animal Handling", AbilityType.Wisdom },
            { "Arcana", AbilityType.Intelligence },
            { "Athletics", AbilityType.Strength },
            { "Deception", AbilityType.Charisma },
            { "History", AbilityType.Intelligence },
            { "Insight", AbilityType.Wisdom },
            { "Intimidation", AbilityType.Charisma },
            { "Medicine", AbilityType.Wisdom },
            { "Nature", AbilityType.Intelligence },
            { "Perception", AbilityType.Wisdom },
            { "Performance", AbilityType.Charisma },
            { "Persuasion", AbilityType.Charisma },
            { "Religion", AbilityType.Intelligence },
            { "Sleight of Hand", AbilityType.Dexterity },
            { "Stealth", AbilityType.Dexterity },
            { "Survival", AbilityType.Wisdom }
        };

        internal int SkillBonusFor(string skillName, IAbilityDataSource abilities, int proficiency)
        {
            try
            {
                return abilities.AbilityModifierFor(_skillAbilityMap5e[skillName]) + proficiency; 
            }
            catch (KeyNotFoundException) { return 0; }
        }


    }
}