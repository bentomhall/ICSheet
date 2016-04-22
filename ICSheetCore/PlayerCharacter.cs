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
            var features = new List<IFeature>();
            features.AddRange(_classAggregate.AllFeatures);
            features.AddRange(_race.Features);
            features.OrderBy(x => x.StartsFromLevel).ThenBy(x => x.Name);
            return features;
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
        /// <summary>
        /// 
        /// </summary>
        public bool IsSpellcaster { get { return _classAggregate.IsSpellcaster; } }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<int> AvailableSpellSlots { get { return _classAggregate.AvailableSpellSlots; } }
        /// <summary>
        /// 
        /// </summary>
        public int ArmorClassBonus
        {
            get { return _defenseAggregate.ArmorClass + ArmorClassOverride; }
        }
        /// <summary>
        /// 
        /// </summary>
        public int CurrentHealth { get { return _health.CurrentHealth; } }
        /// <summary>
        /// 
        /// </summary>
        public int MaxHealth { get { return _health.MaxHealth; } set { _health.MaxHealth = value; } }
        /// <summary>
        /// 
        /// </summary>
        public int Experience { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string RaceName { get { return _race.RaceName; } }
        /// <summary>
        /// 
        /// </summary>
        public string Notes { get { return _notes; } set { _notes = value; } }
        /// <summary>
        /// 
        /// </summary>
        public ICollection<IFeature> Features { get { return aggregateFeatures(); } }
        /// <summary>
        /// 
        /// </summary>
        public int Proficiency { get { return _classAggregate.ProficiencyBonus; } }
        /// <summary>
        /// Feats count as class features.
        /// </summary>
        /// <param name="feature"></param>
        public void AddFeature(IFeature feature)
        {
            if (feature as ClassFeature != null) { _classAggregate.AddFeature(feature); }
            else if (feature as RaceFeature != null) { _race.AddFeature(feature); }
            else { return; } //ideally will never happen.
        }

        //Skills
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void AddItemToInventory(IItem item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
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
        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void DoGoldTransaction(double amount)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="className">The name of the class in which to level up.</param>
        /// <param name="newFeatures">should only contain the features for the new class. Ignored if increasing level of current class</param>
        public void DoLevelUp(string className, IEnumerable<IFeature> newFeatures)
        {
            _classAggregate.LevelUp(className, newFeatures);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void DropItem(IItem item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="item"></param>
        public void Equip(IItem item)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public IItem EquippedItemForSlot(ItemSlot slot)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<IItem> ItemsMatching(Func<IItem, bool> predicate)
        {
            throw new NotImplementedException();
        }
        /// <summary>
        /// Non-armor, non-attribute bonuses/maluses to armor.
        /// </summary>
        public int ArmorClassOverride { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public void TakeLongRest()
        {
            _health.HealDamage(MaxHealth);
            _classAggregate.ResetSpellSlots(new Dictionary<int, int>() { { -1, -1 } });
            throw new NotImplementedException();
        }
        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<int> SpellSlots { get { return _classAggregate.AvailableSpellSlots; } }


    }

}
