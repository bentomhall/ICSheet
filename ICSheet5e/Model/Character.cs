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
            skills = new SkillList<Skill5e>(Edition.Fifth);
        }

        public Character(string characterName, CharacterClasses classLevels, string race, Dictionary<AbilityType, Ability> abilitySet, int Health)
        {
            CharacterName = characterName;
            Race = race;
            this.abilities = abilitySet;
            CharacterClassLevels = classLevels;
            int totalLevel = classLevels.Sum(x => x.Value);
            _proficiencyBonus = calculateProficiency(totalLevel);
            skills = new SkillList<Skill5e>(Edition.Fifth);
            MaxHealth = Health;
            inventory = new Inventory<Item>(abilitySet[AbilityType.Strength].score);

        }

        private Inventory<Item> inventory;
        private List<IClassFeature> features = new List<IClassFeature>();
        private List<SpellCaster> spellBooks = new List<SpellCaster>();
        private int calculateProficiency(int level)
        {
            return (level - 1) / 4 + 2; //integer division
        }

        void setSkills(List<string> taggedSkills)
        {
            List<Skill5e> skillBonuses = new List<Skill5e>();
          
            foreach (string skillName in skills.getSkillNames())
            {
                AbilityType associatedAbility = skills.abilityFor(skillName);
                if (taggedSkills.Contains(skillName)) 
                {
                    skillBonuses.Add( new Skill5e(skillName, _proficiencyBonus + abilities[associatedAbility].modifier));
                }
                else
                {
                    skillBonuses.Add(new Skill5e(skillName, abilities[associatedAbility].modifier));
                }
            }
            this.skills.setAllSkillBonuses(skillBonuses);
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
