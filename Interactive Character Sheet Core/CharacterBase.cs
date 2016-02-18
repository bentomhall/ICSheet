using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace Interactive_Character_Sheet_Core
{
    [DataContract]
    public abstract class CharacterBase
    {

        protected DiceBag dice = new DiceBag();
        [DataMember] protected string Race { get; set; }
        [DataMember] protected string CharacterName { get; set; }
        #region Initiative
        [DataMember] protected int initiativeModifier = 0;
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

        public int AbilityScoreFor(AbilityType ability)
        {
            return abilities[ability].score;
        }

        #endregion

        #region Skills
        protected abstract void SetSkills<T>(List<T> taggedSkills) where T : ISkill;
        #endregion

        #region Health
        [DataMember] public int MaxHealth { get; protected set; }
        [DataMember] protected int _currentHealth;
        public int CurrentHealth { get { return _currentHealth; } }
        public void TakeDamage(IDamage damage)
        {
            _currentHealth -= damage.Amount;
        }

        public void HealDamage(int amount)
        {
            _currentHealth = Math.Min(MaxHealth, _currentHealth + amount);
        }
        #endregion

        #region Defenses
        [DataMember] protected List<DamageType> Resistances { get; set; }
        [DataMember] protected List<DamageType> Immunities { get; set; }
        [DataMember] private List<Defense> _defenses = new List<Defense>();
        protected List<Defense> Defenses { get { return _defenses; } }
        public bool willHit(DefenseType type, int withAttack)
        {
            var d = _defenses.Single(s => s.type == type);
            return d.willHit(withAttack);
        }
        #endregion

        [DataMember] protected VisionType _vision;
        public VisionType Vision { get { return _vision; } }

        [DataMember] public int Movement { get; set; }

    }
}
