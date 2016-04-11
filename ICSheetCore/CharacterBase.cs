using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Runtime.Serialization.Json;
using System.IO;

namespace ICSheetCore
{
    [DataContract]
    public abstract class CharacterBase
    {

        [DataMember] public string CharacterName { get; set; }
        #region Initiative
        [DataMember] protected int initiativeModifier = 0;
        public int Initiative
        {
            get { return initiativeModifier; }
        }

        #endregion

        #region Abilities
        [DataMember]
        protected Dictionary<AbilityType, Ability> _abilities = new Dictionary<AbilityType, Ability>()
        {
            { AbilityType.Strength, new Ability(10) },
            { AbilityType.Dexterity, new Ability(10) },
            { AbilityType.Constitution, new Ability(10) },
            { AbilityType.Intelligence, new Ability(10) },
            { AbilityType.Wisdom, new Ability(10) },
            { AbilityType.Charisma, new Ability(10) }
        };

        public Dictionary<AbilityType, Ability> Abilities { get { return _abilities; } }

        public void MutateAbilityScore(AbilityType ability, int newScore)
        {
            _abilities[ability] = new Ability(newScore);
        }

        public int AbilityModifierFor(AbilityType ability)
        {
            if (ability == AbilityType.None) { return 0; }
            return _abilities[ability].Modifier;
        }

        public int AbilityScoreFor(AbilityType ability)
        {
            if (ability == AbilityType.None) { return 10; }
            return _abilities[ability].Score;
        }

        #endregion

        #region Health
        [DataMember] public int MaxHealth { get; set; }
        [DataMember] protected int _currentHealth;
        public int CurrentHealth { get { return _currentHealth; } }
        public virtual void TakeDamage(int damage)
        {
            if (_temporaryHP > damage)
            {
                _temporaryHP -= damage;
            }
            else if (_temporaryHP > 0)
            {
                _currentHealth -= damage - _temporaryHP;
                _temporaryHP = 0;
            }
            else
            {
                _currentHealth -= damage;
            }
            _currentHealth = Math.Max(_currentHealth, 0); //no negative health
        }

        public virtual void HealDamage(int amount)
        {
            if (_currentHealth < 0) { _currentHealth = 0; }
            _currentHealth = Math.Min(MaxHealth, _currentHealth + amount);
        }

        [DataMember]
        private  int _temporaryHP = 0;
        public int TemporaryHP { get { return _temporaryHP; } }
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Naming", "CA1709:IdentifiersShouldBeCasedCorrectly", MessageId = "THP")]
        public void AddTHP(int amount)
        {
            if (amount > _temporaryHP) { _temporaryHP = amount; } //does not stack
        }
        #endregion

        #region Defenses
        [DataMember] protected List<Defense> _defenses = new List<Defense>();
        public List<Defense> Defenses { get { return _defenses; } }
        public bool WillHit(DefenseType type, int withAttack)
        {
            var d = _defenses.Single(s => s.TypeOfDefense == type);
            return d.WillHit(withAttack);
        }
        #endregion

        [DataMember] protected VisionType _vision;
        public VisionType Vision { get { return _vision; } }

        [DataMember] public int Movement { get; set; }

    }
}
