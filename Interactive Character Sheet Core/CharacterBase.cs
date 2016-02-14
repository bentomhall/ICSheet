using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Character_Sheet_Core
{
    

    public abstract class CharacterBase
    {
        //skills -- abstract
        //feats -- abstract
        //vision -- abstract
        //movement -- abstract
        //inventory (equipped) -- abstract
        //constructors: (Factory?)
        //-->New
        //-->From XML

        protected DiceBag dice = new DiceBag();
        protected string Race { get; set; }
        protected Dictionary<string, int> CharacterClassLevels { get; set; }
        protected string CharacterName { get; set; }
        #region Initiative
        protected int initiativeModifier = 0;
        public int initiative
        {
            get { return initiativeModifier; }
        }

        public int RollInitiative()
        {
            return dice.rollOne(DiceSize.d20) + initiativeModifier;
        }
        #endregion

        #region Abilities
        protected Dictionary<AbilityType, Ability> abilities = new Dictionary<AbilityType, Ability>()
        {
            { AbilityType.Strength, new Ability(AbilityType.Strength, 10) },
            { AbilityType.Dexterity, new Ability(AbilityType.Dexterity, 10) },
            { AbilityType.Constitution, new Ability(AbilityType.Constitution, 10) },
            { AbilityType.Intelligence, new Ability(AbilityType.Intelligence, 10) },
            { AbilityType.Wisdom, new Ability(AbilityType.Wisdom, 10) },
            { AbilityType.Charisma, new Ability(AbilityType.Charisma, 10) }
        };

        public void mutateAbilityScore(AbilityType ability, int newScore)
        {
            abilities[ability] = new Ability(ability, newScore);
        }

        public int abilityModifierFor(AbilityType ability)
        {
            return abilities[ability].modifier;
        }
        #endregion

        #region Skills
        protected SkillList skills;
        abstract void setSkills(List<ISkill> taggedSkills);
        #endregion
    }
}
