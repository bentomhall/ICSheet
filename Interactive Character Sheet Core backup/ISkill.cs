using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace InteractiveCharacterSheetCore
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
            skills[skill.Name] = skill;
        }

        public void SetAllSkillBonuses(List<T> skillList)
        {
            if (skillList == null) { return; }
            foreach (T item in skillList)
            {
                skills[item.Name] = item;
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
        private Dictionary<string, AbilityType> skillAbilityMap5e = new Dictionary<string, AbilityType>()
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

        public AbilityType AbilityFor(string skillName)
        {
                switch (edition)
                {
                    case Edition.Fifth:
                        return skillAbilityMap5e[skillName];
                    default:
                        throw new NotImplementedException("Only 5th editions allowed");
                }
        }

        [DataMember] private Dictionary<string, T> skills = new Dictionary<string, T>();
        public int SkillBonusFor(string skillName)
        {
            return skills[skillName].Bonus;
        }

        public bool IsSkillTagged(string skillName)
        {
            return skills[skillName].IsTagged;
        }

        public List<string> SkillNames 
        { 
            get
            {
                switch (edition)
                {
                    case Edition.Fifth:
                        return skillAbilityMap5e.Keys.ToList<string>();
                    default:
                        return new List<string>();
                }
            }
        }

    }

    public interface ISkill
    {
        string Name { get; }
        int Bonus { get; set; }
        bool IsTagged { get; set; }
    }
}
