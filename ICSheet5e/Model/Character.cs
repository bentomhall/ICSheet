using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactive_Character_Sheet_Core;

namespace ICSheet5e.Model
{
    using CharacterClasses = Dictionary<CharacterClassType, int>;
    public class Character: CharacterBase
    {
        private static List<CharacterClassType> castingClasses = new List<CharacterClassType>()
        {
            CharacterClassType.Bard,
            CharacterClassType.Cleric,
            CharacterClassType.Druid,
            CharacterClassType.Paladin,
            CharacterClassType.Ranger,
            CharacterClassType.Sorcerer,
            CharacterClassType.Warlock,
            CharacterClassType.Wizard,
            CharacterClassType.EldritchKnight
        };

        private CharacterClasses CharacterClassLevels;
        private int _proficiencyBonus = 2;
        public int Proficiency
        {
            get { return _proficiencyBonus; }
        }

        public Character()
        {
            skills = new SkillList(Edition.Fifth);
        }

        public Character(string characterName, CharacterClasses classLevels, string race, Dictionary<AbilityType, Ability> abilitySet, int Health)
        {
            CharacterName = characterName;
            Race = race;
            this.abilities = abilitySet;
            CharacterClassLevels = classLevels;
            int totalLevel = classLevels.Sum(x => x.Value);
            _proficiencyBonus = calculateProficiency(totalLevel);
            skills = new SkillList(Edition.Fifth);
            MaxHealth = Health;
            MaxCarryWeight = 5 * abilitySet[AbilityType.Strength].modifier;

        }

        private List<IClassFeature> features = new List<IClassFeature>();
        private List<SpellCaster> spellBooks = new List<SpellCaster>();
        private int calculateProficiency(int level)
        {
            return (level - 1) / 4 + 2; //integer division
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

        void setSpellCasting()
        {
            foreach (KeyValuePair<CharacterClassType, int> entry in CharacterClassLevels)
            {
                if (castingClasses.Contains(entry.Key))
                {
                    SpellCaster book = new SpellCaster(entry.Key, entry.Value);
                    
                }
            }
        }
    }

    enum CharacterClassType
    {
        Barbarian,
        Bard,
        Cleric,
        Druid,
        Fighter,
        Monk,
        Paladin,
        Ranger,
        Rogue,
        Sorcerer,
        Warlock,
        Wizard,
        EldritchKnight,
        ArcaneTrickster,
    }
}
