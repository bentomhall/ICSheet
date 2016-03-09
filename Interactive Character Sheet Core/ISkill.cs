using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Interactive_Character_Sheet_Core
{
    public enum Edition
    {
        Fourth,
        Fifth,
    }

    [DataContract]
    public class SkillList<T> where T: ISkill
    {
        [DataMember] private Edition edition;
        public SkillList(Edition version)
        {
            edition = version;

        }

        public void SetSkillBonusFor(T skill)
        {
            skills[skill.name] = skill;
        }

        public void setAllSkillBonuses(List<T> skillList)
        {
            foreach (T item in skillList)
            {
                skills[item.name] = item;
            }
        }

        public T SkillForName(string name)
        {
            if (skills.ContainsKey(name)){
                return skills[name];
            }
            else
            {
                return default(T);
            }
        }

        [DataMember]
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

        [DataMember]
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

        [DataMember] private Dictionary<string, T> skills = new Dictionary<string, T>();
        public int skillBonusFor(string skillName)
        {
            return skills[skillName].bonus;
        }

        public bool IsSkillTagged(string skillName)
        {
            return skills[skillName].IsTagged;
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
        string name { get; }
        int bonus { get; set; }
        bool IsTagged { get; set; }
    }
}
