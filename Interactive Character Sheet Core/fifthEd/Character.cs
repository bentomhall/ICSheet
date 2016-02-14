using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Character_Sheet_Core
{
    using CharacterClasses = Dictionary<string, int>;
    public class Character: CharacterBase
    {
        private int _proficiencyBonus = 2;
        public int Proficiency
        {
            get { return _proficiencyBonus; }
        }

        public Character()
        {
            skills = new SkillList(Edition.Fifth);
        }

        public Character(string characterName, CharacterClasses classLevels, string race, Dictionary<AbilityType, Ability> abilitySet)
        {
            CharacterName = characterName;
            Race = race;
            this.abilities = abilitySet;
            CharacterClassLevels = classLevels;
            int totalLevel = classLevels.Sum(x => x.Value);
            _proficiencyBonus = calculateProficiency(totalLevel);
            skills = new SkillList(Edition.Fifth);
        }

        private int calculateProficiency(int level)
        {
            return level / 4 + 2; //integer division
        }

        void setSkills(List<string> taggedSkills)
        {
            Dictionary<string, int> skillBonuses = new Dictionary<string, int>();
          
            foreach (string skillName in skills.getSkillNames())
            {
                AbilityType associatedAbility = skills.abilityFor(skillName);
                if (taggedSkills.Contains(skillName)) 
                {
                    skillBonuses[skillName] = _proficiencyBonus + abilities[associatedAbility].modifier;
                }
                else
                {
                    skillBonuses[skillName] = abilities[associatedAbility].modifier;
                }
            }

            skills.setAllSkillBonuses(skillBonuses);
        }
    }
}
