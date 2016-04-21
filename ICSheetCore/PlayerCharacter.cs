using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheetCore
{
    public class PlayerCharacter
    {
        private string _name;
        private string _alignment;
        private string _notes;
        private string _background;

        private HealthManager _health;
        private AbilityAggregate _abilityAggregate; //can construct
        private DefenseAggregate _defenseAggregate; //can construct
        private IRace _race; //should be passed in
        private PlayerClassAggregate _classAggregate; //should be passed in
        private SkillAggregate _skillAggregate; //can construct
                                                //private Inventory _inventory;

        private ICollection<IFeature> aggregateFeatures()
        {
            throw new NotImplementedException();
        }

        internal PlayerCharacter(string name, IRace race, PlayerClassAggregate classesAndLevels)
        {
            _race = race;
            _name = name;
            _classAggregate = classesAndLevels;
            _abilityAggregate = new AbilityAggregate();
            _defenseAggregate = new DefenseAggregate(_abilityAggregate, _classAggregate.ProficiencyForDefenses);
            _skillAggregate = new SkillAggregate();
        }

        public bool IsSpellcaster { get { return _classAggregate.IsSpellcaster; } }

        public IEnumerable<int> AvailableSpellSlots { get { return _classAggregate.AvailableSpellSlots; } }

        public int ArmorClassBonus
        {
            get { return _defenseAggregate.ArmorClass; }
        }

        public int CurrentHealth { get { return _health.CurrentHealth; } }

        public int MaxHealth { get { return _health.MaxHealth; } set { _health.MaxHealth = value; } }

        public int Experience { get; set; }

        public string RaceName { get { return _race.RaceName; } }

        public string Notes { get { return _notes; } set { _notes = value; } }

        public ICollection<IFeature> Features { get { return aggregateFeatures(); } }

        public int Proficiency { get { return _classAggregate.ProficiencyBonus; } }

        public void AddFeature(IFeature feature)
        {
            if (feature as ClassFeature != null) { _classAggregate.AddFeature(feature); }
            else if (feature as RaceFeature != null) { _race.AddFeature(feature); }
            else { return; } //ideally will never happen.
        }

        //Skills

        public void AddItemToInventory(IItem item)
        {
            throw new NotImplementedException();
        }

        public int AttackBonusWith(IItem weapon)
        {
            var w = weapon as WeaponItem;
            if (w == null) { return _abilityAggregate.AbilityModifierFor(AbilityType.Strength); } //improvised weapons
            var abilities = w.AssociatedAbilities;
            var modifier = abilities.Max(x => _abilityAggregate.AbilityModifierFor(x));
            if (w.IsProficient)
            {
                return modifier + Proficiency;
            }
            else
            {
                return modifier;
            }
        }

        public int DamageBonusWith(IItem weapon)
        {
            var w = weapon as WeaponItem;
            if (w == null && _classAggregate.HasFeature("Martial Arts"))
            {
                return _abilityAggregate.AbilityModifierFor(AbilityType.Strength);
            }
            else if (w == null) { return 0; }
            var abilities = w.AssociatedAbilities;
            if (w.Slot == ItemSlot.Mainhand)
            {
                return abilities.Max(x => _abilityAggregate.AbilityModifierFor(x)) + w.EnhancementBonus;
            }
            else if (_classAggregate.HasFeature("Two-Weapon Fighting"))
            {
                return abilities.Max(x => _abilityAggregate.AbilityModifierFor(x)) + w.EnhancementBonus;
            }
            else { return w.EnhancementBonus; }
        }

        public void DoGoldTransaction(double amount)
        {
            throw new NotImplementedException();
        }

        public void DoLevelUp(ICollection<PlayerCharacterClassDetail> newLevels, IEnumerable<IFeature> newFeatures)
        {
            throw new NotImplementedException();
        }

        public void DropItem(IItem item)
        {
            throw new NotImplementedException();
        }

        public void Equip(IItem item)
        {
            throw new NotImplementedException();
        }

        public IItem EquippedItemForSlot(ItemSlot slot)
        {
            throw new NotImplementedException();
        }

        public IEnumerable<IItem> ItemsMatching(Func<IItem, bool> predicate)
        {
            throw new NotImplementedException();
        }

        public int ArmorClassOverride { get; set; }

        public TakeLongRest()
        {
            throw new NotImplementedException();
        }

        public IEnumerable<int> SpellSlots { get { return _classAggregate.AvailableSpellSlots; } }


    }

}
