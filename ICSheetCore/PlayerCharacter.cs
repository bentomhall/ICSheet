using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheetCore
{
    /// <summary>
    /// 
    /// </summary>
    public class PlayerCharacter :ISpellcastingDataSource, ISpellcastingDelegate
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
        private InventoryAggregate _inventory;

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
            _inventory = new InventoryAggregate();
        }

        #region ISpellcastingDataSource
        /// <summary>
        /// 
        /// </summary>
        public bool IsSpellcaster { get { return _classAggregate.IsSpellcaster; } }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<int> SpellSlots { get { return _classAggregate.AvailableSpellSlots; } }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Spell> PreparedSpells
        {
            get { return _classAggregate.PreparedSpells; }
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyDictionary<string, int> SpellAttackBonuses
        {
            get { return _classAggregate.SpellAttackBonuses(_abilityAggregate); }
        }

        /// <summary>
        /// 
        /// </summary>
        public IReadOnlyDictionary<string, int> SpellDCs
        {
            get { return _classAggregate.SpellDCs(_abilityAggregate); }
        }
        #endregion

        #region ISpellcastingDelegate
        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        public void UseSpellSlot(int level)
        {
            _classAggregate.UseSpellSlot(level);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellName"></param>
        /// <param name="asClass"></param>
        public void Learn(string spellName, string asClass)
        {
            throw new NotImplementedException();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellName"></param>
        /// <param name="asClass"></param>
        public void Unlearn(string spellName, string asClass) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellName"></param>
        /// <param name="asClass"></param>
        public void Prepare(string spellName, string asClass) { }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellName"></param>
        /// <param name="asClass"></param>
        public void Unprepare(string spellName, string asClass) { }
        #endregion


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
            _inventory.AddItem(item);
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="weapon"></param>
        /// <returns></returns>
        public int AttackBonusWith(IItem weapon)
        {
            var w = weapon as WeaponItem;
            var abilitiesSource = _abilityAggregate as IAbilityDataSource;
            if (w == null) { return abilitiesSource.AbilityModifierFor(AbilityType.Strength); } //improvised weapons
            var abilities = w.AssociatedAbilities;
            var modifier = abilities.Max(x => abilitiesSource.AbilityModifierFor(x));
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
            var abilitiesSource = _abilityAggregate as IAbilityDataSource;
            if (w == null && _classAggregate.HasFeature("Martial Arts"))
            {
                return abilitiesSource.AbilityModifierFor(AbilityType.Strength);
            }
            else if (w == null) { return 0; }
            var abilities = w.AssociatedAbilities;
            if (w.Slot == ItemSlot.Mainhand)
            {
                return abilities.Max(x => abilitiesSource.AbilityModifierFor(x)) + w.EnhancementBonus;
            }
            else if (_classAggregate.HasFeature("Two-Weapon Fighting"))
            {
                return abilities.Max(x => abilitiesSource.AbilityModifierFor(x)) + w.EnhancementBonus;
            }
            else { return w.EnhancementBonus; }
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void DoGoldTransaction(decimal amount)
        {
            _inventory.MoneyTransaction(amount);
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
            _inventory.UnequipItem(item);
            _inventory.RemoveItem(item);
        }

        /// <summary>
        /// Equips item and recalculates AC if the item was armor.
        /// </summary>
        /// <param name="item"></param>
        public void Equip(IItem item)
        {
            _inventory.EquipItem(item);
            var a = item as ArmorItem;
            if (a != null)
            {
                var hasShield = _inventory.ItemEquippedIn(ItemSlot.Offhand) is ArmorItem;
                var baseAC = _classAggregate.BaseACWith(_abilityAggregate, a.ArmorClassType, hasShield);
                _defenseAggregate.ChangeACFromArmor(a, _abilityAggregate, baseAC);
            }
        }

        /// <summary>
        /// Unequips item and recalculates AC (using cloth) if the item was armor.
        /// </summary>
        /// <param name="item"></param>
        public void Unequip(IItem item)
        {
            _inventory.UnequipItem(item);
            var a = item as ArmorItem;
            if (a != null)
            {
                var hasShield = _inventory.ItemEquippedIn(ItemSlot.Offhand) is ArmorItem;
                var armor = _inventory.ItemEquippedIn(ItemSlot.Armor) as ArmorItem;
                var baseAC = _classAggregate.BaseACWith(_abilityAggregate, armor.ArmorClassType, hasShield);
                _defenseAggregate.ChangeACFromArmor(armor, _abilityAggregate, baseAC);
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slot"></param>
        /// <returns></returns>
        public IItem EquippedItemForSlot(ItemSlot slot)
        {
            return _inventory.ItemEquippedIn(slot);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="predicate"></param>
        /// <returns></returns>
        public IEnumerable<IItem> ItemsMatching(Func<IItem, bool> predicate)
        {
            return _inventory.ContentsMatching(predicate);
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
        }

        /// <summary>
        /// returns the object representing the cash on hand. Read only. Setters inaccessible outside of the assembly.
        /// </summary>
        public Money Cash { get { return _inventory.CashOnHand; } }
        
        
        


    }

}
