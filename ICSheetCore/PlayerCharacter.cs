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
    public class PlayerCharacter :ISpellcastingDataSource, ISpellcastingDelegate, IInventoryDataSource, IInventoryDelegate
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
            _abilityAggregate.AbilityModified += _defenseAggregate.ModifyAbilityBonus;
            _skillAggregate = new SkillAggregate();
            _inventory = new InventoryAggregate();
            _health = new HealthManager();
        }

        /// <summary>
        /// 
        /// </summary>
        public string Alignment { get { return _alignment; } set { _alignment = value; } }

        /// <summary>
        /// 
        /// </summary>
        public string Background { get { return _background; } set { _background = value; } }

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
        public IEnumerable<Spell> KnownSpells
        {
            get { return _classAggregate.KnownSpells; }
        }

        public IEnumerable<string> SpellcastingClasses
        {
            get { return _classAggregate.SpellcastingClasses; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string SpellPreparationCount
        {
            get { return _classAggregate.SpellsPreparedOfMax(_abilityAggregate); }
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
        /// <param name="isBonus"></param>
        public void Learn(string spellName, string asClass, bool isBonus)
        {
            _classAggregate.LearnSpell(spellName, asClass, isBonus);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellName"></param>
        /// <param name="asClass"></param>
        public void Unlearn(string spellName, string asClass) { _classAggregate.UnlearnSpell(spellName, asClass); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellName"></param>
        /// <param name="asClass"></param>
        public void Prepare(string spellName, string asClass) { _classAggregate.PrepareSpell(spellName, asClass); }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellName"></param>
        /// <param name="asClass"></param>
        public void Unprepare(string spellName, string asClass) { _classAggregate.UnprepareSpell(spellName, asClass); }
        #endregion

        #region IInventoryDelegate
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
        /// <param name="amount"></param>
        public void DoGoldTransaction(decimal amount)
        {
            _inventory.MoneyTransaction(amount);
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
        #endregion

        #region IInventoryDataSource
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
        /// returns the object representing the cash on hand. Read only. Setters inaccessible outside of the assembly.
        /// </summary>
        public Money Cash { get { return _inventory.CashOnHand; } }
        #endregion


        #region Defenses
        /// <summary>
        /// 
        /// </summary>
        public int ArmorClassBonus
        {
            get { return _defenseAggregate.ArmorClass; }
        }

        /// <summary>
        /// 
        /// </summary>
        public void SetNonACDefenseOverride(DefenseType defense, int value)
        {
            _defenseAggregate.ModifyDefenseAdjustment(defense, value);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="defense"></param>
        public void GetNotACDefenseOverride(DefenseType defense)
        {
            _defenseAggregate.GetDefenseAdjustment(defense);
        }

        /// <summary>
        /// Non-armor, non-attribute bonuses/maluses to armor.
        /// </summary>
        public int ArmorClassOverride
        {
            get { return _defenseAggregate.GetDefenseAdjustment(DefenseType.Armor); }
            set { _defenseAggregate.ModifyDefenseAdjustment(DefenseType.Armor, value); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="type"></param>
        /// <returns></returns>
        public Tuple<int, bool> DefenseBonusFor(DefenseType type)
        {

            return new Tuple<int, bool>(_defenseAggregate.DefenseValueFor(type), _classAggregate.ProficiencyForDefenses[type] > 0);
        }
        #endregion

        #region Health
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
        public int TemporaryHP { get { return _health.TemporaryHP; } }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void TakeDamage(int amount)
        {
            _health.TakeDamage(amount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void HealDamage(int amount)
        {
            _health.HealDamage(amount);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount"></param>
        public void AddTHP(int amount)
        {
            _health.AddTHP(amount);
        }
        #endregion

        
        /// <summary>
        /// 
        /// </summary>
        public int Initiative
        {
            get { return _classAggregate.InitiativeBonus(_abilityAggregate); }
        }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, int> Levels
        {
            get { return _classAggregate.Levels; }
        }

        /// <summary>
        /// 
        /// </summary>
        public int Movement
        {
            get { return _classAggregate.MovementSpeed(this) + _race.BaseMovement; }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name { get { return _name; } }

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

        /// <summary>
        /// Gets the bonus for the given skill.
        /// </summary>
        /// <param name="skillName"></param>
        /// <param name="proficiencyLevel"></param>
        /// <returns></returns>
        public int SkillBonusFor(string skillName, ProficiencyType proficiencyLevel)
        {
            int p = 0;
            switch (proficiencyLevel)
            {
                case ProficiencyType.Expertise:
                    p = 2 * Proficiency;
                    break;
                case ProficiencyType.Full:
                    p = Proficiency;
                    break;
                case ProficiencyType.Half:
                    p = Proficiency / 2;
                    break;
                case ProficiencyType.None:
                    p = 0;
                    break;
            }
            return _skillAggregate.SkillBonusFor(skillName, _abilityAggregate, p);
        }

        /// <summary>
        /// 
        /// </summary>
        public IDictionary<string, ProficiencyType> SkillProficiencies
        {
            get; set;
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
            if (w.IsProficient || (w.Name == "Unarmed Strike" && _classAggregate.HasFeature("Martial Arts")))
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
            if (w.Name == "Unarmed Strike" && _classAggregate.HasFeature("Martial Arts"))
            {
                return abilitiesSource.AbilityModifierFor(AbilityType.Strength);
            }
            else if (w.Name == "Unarmed Strike") { return 0; }
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
        /// <param name="className">The name of the class in which to level up.</param>
        /// <param name="newFeatures">should only contain the features for the new class. Ignored if increasing level of current class</param>
        public void DoLevelUp(string className, IEnumerable<IFeature> newFeatures)
        {
            _classAggregate.LevelUp(className, newFeatures);
        }


        /// <summary>
        /// 
        /// </summary>
        public void TakeLongRest()
        {
            _health.HealDamage(MaxHealth);
            _classAggregate.ResetSpellSlots(new Dictionary<int, int>() { { -1, -1 } });
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ability"></param>
        /// <param name="value"></param>
        public void ModifyAbilityScore(AbilityType ability, int value)
        {
            _abilityAggregate.Modify(ability, value);
            var hasShield = _inventory.ItemEquippedIn(ItemSlot.Offhand) is ArmorItem;
            var armor = _inventory.ItemEquippedIn(ItemSlot.Armor) as ArmorItem;
            var baseAC = _classAggregate.BaseACWith(_abilityAggregate, armor.ArmorClassType, hasShield);
            _defenseAggregate.ChangeACFromArmor(armor, _abilityAggregate, baseAC);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ability"></param>
        /// <returns></returns>
        public int AbilityScoreFor(AbilityType ability)
        {
            var abilities = _abilityAggregate as IAbilityDataSource;
            return abilities.AbilityScoreFor(ability);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="ability"></param>
        /// <returns></returns>
        public int AbilityModifierFor(AbilityType ability)
        {
            var abilities = _abilityAggregate as IAbilityDataSource;
            return abilities.AbilityModifierFor(ability);
        }


        #region Serialization
        /// <summary>
        /// Collects all data to reconstruct character.
        /// </summary>
        /// <returns></returns>
        public Data.CharacterData ToCharacterData()
        {
            var d = new Data.CharacterData();
            d.Name = _name;
            d.Alignment = _alignment;
            d.Background = _background;
            d.Notes = _notes;
            d.Experience = Experience;
            d.AbilityScores = abilityScores();
            d.RaceInformation = _race.GetInformation();
            d.ClassLevelInformation = Levels;
            d.Items = ItemsMatching(x => true);
            d.EquippedItems = _inventory.GetEquippedItems();
            d.Cash = Cash;
            d.CustomFeatures = _classAggregate.CustomFeatures;
            d.HealthInformation = new Tuple<int, int, int>(_health.CurrentHealth, _health.MaxHealth, _health.TemporaryHP);
            d.KnownSpells = KnownSpells;
            d.DefenseOverrides = _defenseAggregate.AllDefenseAdjustments;
            d.Skills = SkillProficiencies;
            d.CurrentSpellSlots = SpellSlots;
            return d;

        }

        private IDictionary<AbilityType, int> abilityScores()
        {
            var abilities = new Dictionary<AbilityType, int>();
            abilities[AbilityType.Strength] = AbilityScoreFor(AbilityType.Strength);
            abilities[AbilityType.Dexterity] = AbilityScoreFor(AbilityType.Dexterity);
            abilities[AbilityType.Constitution] = AbilityScoreFor(AbilityType.Constitution);
            abilities[AbilityType.Intelligence] = AbilityScoreFor(AbilityType.Intelligence);
            abilities[AbilityType.Wisdom] = AbilityScoreFor(AbilityType.Wisdom);
            abilities[AbilityType.Charisma] = AbilityScoreFor(AbilityType.Charisma);
            return abilities;
        }

        #endregion
    }

    /// <summary>
    /// The type of proficiency to add to a skill check.
    /// </summary>
    public enum ProficiencyType
    {
        /// <summary>
        /// Not proficient.
        /// </summary>
        None,
        /// <summary>
        /// Jack of all trades (bard) gets 1/2 proficiency to otherwise non-proficient skills
        /// </summary>
        Half,
        /// <summary>
        /// Regular proficiency
        /// </summary>
        Full,
        /// <summary>
        /// Double proficiency (rogue expertise, Knowledge cleric, a few others).
        /// </summary>
        Expertise
    }
}
