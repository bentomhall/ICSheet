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

        private DiceBag dice = new DiceBag();
        public DiceBag Dice
        {
            get
            {
                if (dice == null)
                {
                    dice = new DiceBag();
                }
                return dice;
            }
        }
        [DataMember] public string CharacterName { get; set; }
        #region Initiative
        [DataMember] protected int initiativeModifier = 0;
        public int Initiative
        {
            get { return initiativeModifier; }
        }

        public int RollInitiative()
        {
            return dice.rollOne(DiceSize.d20) + initiativeModifier;
        }
        #endregion

        #region Abilities
        [DataMember]
        protected Dictionary<AbilityType, Ability> abilities = new Dictionary<AbilityType, Ability>()
        {
            { AbilityType.Strength, new Ability(AbilityType.Strength, 10) },
            { AbilityType.Dexterity, new Ability(AbilityType.Dexterity, 10) },
            { AbilityType.Constitution, new Ability(AbilityType.Constitution, 10) },
            { AbilityType.Intelligence, new Ability(AbilityType.Intelligence, 10) },
            { AbilityType.Wisdom, new Ability(AbilityType.Wisdom, 10) },
            { AbilityType.Charisma, new Ability(AbilityType.Charisma, 10) }
        };

        public Dictionary<AbilityType, Ability> Abilities { get { return abilities; } }

        public void mutateAbilityScore(AbilityType ability, int newScore)
        {
            abilities[ability] = new Ability(ability, newScore);
        }

        public int abilityModifierFor(AbilityType ability)
        {
            if (ability == AbilityType.None) { return 0; }
            return abilities[ability].modifier;
        }

        public int AbilityScoreFor(AbilityType ability)
        {
            if (ability == AbilityType.None) { return 10; }
            return abilities[ability].score;
        }

        #endregion

        #region Skills
        protected abstract void SetSkills<T>(List<T> taggedSkills) where T : ISkill;
        #endregion

        #region Health
        [DataMember] public int MaxHealth { get; set; }
        [DataMember] protected int _currentHealth;
        public int CurrentHealth { get { return _currentHealth; } }
        public virtual void TakeDamage(IDamage damage)
        {
            if (_temporaryHP > damage.Amount)
            {
                _temporaryHP -= damage.Amount;
            }
            else if (_temporaryHP > 0)
            {
                _currentHealth -= damage.Amount - _temporaryHP;
                _temporaryHP = 0;
            }
            else
            {
                _currentHealth -= damage.Amount;
            }
            _currentHealth = Math.Max(_currentHealth, 0); //no negative health
        }

        public virtual void HealDamage(int amount)
        {
            if (_currentHealth < 0) { _currentHealth = 0; }
            _currentHealth = Math.Min(MaxHealth, _currentHealth + amount);
        }

        [DataMember]
        public  int _temporaryHP = 0;
        public int TemporaryHP { get { return _temporaryHP; } }
        public void AddTHP(int amount)
        {
            if (amount > _temporaryHP) { _temporaryHP = amount; } //does not stack
        }
        #endregion

        #region Defenses
        [DataMember] protected List<DamageType> Resistances { get; set; }
        [DataMember] protected List<DamageType> Immunities { get; set; }
        [DataMember] protected List<Defense> _defenses = new List<Defense>();
        public List<Defense> Defenses { get { return _defenses; } }
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
