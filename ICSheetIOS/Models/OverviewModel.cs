using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSheetCore;

namespace ICSheetIOS.Models
{
    public class OverviewModel
    {
        private PlayerCharacter _character;
        #region Static Maps
        static private Dictionary<string, AbilityType> _abilityNames = new Dictionary<string, AbilityType>()
        {
            {"STR", AbilityType.Strength },
            {"DEX", AbilityType.Dexterity },
            {"CON", AbilityType.Constitution },
            {"INT", AbilityType.Intelligence },
            {"WIS", AbilityType.Wisdom },
            {"CHA", AbilityType.Charisma }
        };

        static private Dictionary<string, DefenseType> _defenseNames = new Dictionary<string, DefenseType>()
        {
            {"STR", DefenseType.Strength },
            {"DEX", DefenseType.Dexterity },
            {"CON", DefenseType.Constitution },
            {"INT", DefenseType.Intelligence },
            {"WIS", DefenseType.Wisdom },
            {"CHA", DefenseType.Charisma }
        };
        #endregion
        private string _formatAbilities(int score, int modifier)
        {
            return $"{score} ({modifier})";
        }
        private string _formatHP(int currentHP, int maxHP, int temporaryHP)
        {
            return $"{currentHP} ({temporaryHP}) of {maxHP}";
        }
        private string _formatDefence(int value, bool isProficient)
        {
            if (isProficient)
            {
                return $"\u2713 {value}";
            }
            else
            {
                return $"{value}";
            }

        }
        private string _formatDictionary(IReadOnlyDictionary<string, int> dict)
        {
            return string.Join(Environment.NewLine, dict.Select(x => $"{x.Key} : {x.Value}"));
        }

        public OverviewModel(PlayerCharacter character)
        {
            _character = character;
        }

        public string Name { get { return _character.Name; } }
        public string AbilityStringFor(string ability)
        {
            if (_abilityNames.ContainsKey(ability))
            {
                var type = _abilityNames[ability];
                return _formatAbilities(_character.AbilityScoreFor(type), _character.AbilityModifierFor(type));
            }
            throw new ArgumentException($"Invalid string for ability: {ability}");

        }
        public string Health
        {
            get { return _formatHP(_character.CurrentHealth, _character.MaxHealth, _character.TemporaryHP); }
        }
        public string DefenseStringFor(string defenseType)
        {
            if (defenseType == "AC") { return _formatDefence(_character.ArmorClassBonus, false); }
            else if (_defenseNames.ContainsKey(defenseType))
            {
                var type = _defenseNames[defenseType];
                var defenseValue = _character.DefenseBonusFor(type);
                return _formatDefence(defenseValue.Item1, defenseValue.Item2);
            }
            throw new ArgumentException($"Invalid defense type received: {defenseType}");
        }
        public string SpellSlots
        {
            get { return string.Join(" / ", _character.SpellSlots); }
        }
        public string SpellAttackBonuses
        {
            get { return _formatDictionary(_character.SpellAttackBonuses); }
        }
        public string SpellDCs
        {
            get { return _formatDictionary(_character.SpellDCs); }
        }
        public string AttackInformationFor(WeaponItem item)
        {
            var bonus = _character.AttackBonusWith(item) + item.EnhancementBonus;
            int damageBonus = item.AssociatedAbilities.Max(x => _character.AbilityModifierFor(x)) + item.EnhancementBonus;
            if (damageBonus >= 0)
            {
                return $"{item.Name}: {bonus}, {item.BaseEffect} + {damageBonus}";
            }
            else
            {
                return $"{item.Name}: {bonus}, {item.BaseEffect} - {damageBonus}";
            }

        }
    }
}
