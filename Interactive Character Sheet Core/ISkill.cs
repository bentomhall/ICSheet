using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Character_Sheet_Core
{
    public enum Edition
    {
        Fourth,
        Fifth,
    }

    public class SkillList
    {
        private Edition edition;
        public SkillList(Edition version)
        {
            edition = version;

        }

        private void setSkillBonusFor(string skillName, int taggedBonus = 0)
        {
            switch(edition){
                case Edition.Fifth:
                    skills[skillName] = new fifthEd.Skill5e(skillName, taggedBonus);
                    break;
                case Edition.Fourth:
                    throw new NotImplementedException("4th edition not implemented yet");
            }
        }

        public void setAllSkillBonuses(Dictionary<string, int> modifierMap)
        {
            foreach (KeyValuePair<string, int> entry in modifierMap)
            {
                setSkillBonusFor(entry.Key, entry.Value);
            }
        }

        public Dictionary<string, AbilityType> skillAbilityMap4e = new Dictionary<string, AbilityType>()
        {
             { "Acrobatics", AbilityType.Dexterity },
             { "Arcana", AbilityType.Intelligence },
             { "Athletics", AbilityType.Strength },
             { "Bluff", AbilityType.Charisma },
             { "Diplomacy", AbilityType.Charisma },
             { "Dungeoneering", AbilityType.Intelligence },
             { "Endurance", AbilityType.Constitution },
             { "Heal", AbilityType.Wisdom },
             { "History", AbilityType.Intelligence },
             { "Insight", AbilityType.Wisdom },
             { "Intimidate", AbilityType.Charisma },
             { "Nature", AbilityType.Wisdom },
             { "Perception", AbilityType.Wisdom },
             { "Religion", AbilityType.Intelligence },
             { "Stealth", AbilityType.Dexterity },
             { "Streetwise", AbilityType.Charisma },
             { "Thievery", AbilityType.Dexterity }
        };

        public Dictionary<string, AbilityType> skillAbilityMap5e = new Dictionary<string, AbilityType>()
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

        public AbilityType abilityFor(string skillName)
        {
                switch (edition)
                {
                    case Edition.Fourth:
                        return skillAbilityMap4e[skillName];
                    case Edition.Fifth:
                        return skillAbilityMap5e[skillName];
                    default:
                        throw new NotImplementedException("Only 4th and 5th editions allowed");
                }
        }

        private Dictionary<string, ISkill> skills = new Dictionary<string,ISkill>();
        public int skillBonusFor(string skillName)
        {
            return skills[skillName].bonus;
        }

        public List<string> getSkillNames() 
        { 
            switch(edition)
            {
                case Edition.Fourth:
                    return skillAbilityMap4e.Keys.ToList<string>();
                case Edition.Fifth:
                    return skillAbilityMap5e.Keys.ToList<string>();
                default:
                    throw new NotImplementedException("Only 4th and 5th editions allowed");
            }
        }

    }

    public interface ISkill
    {
        public string name { get; }
        public int bonus { get; }
    }
}
