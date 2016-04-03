using InteractiveCharacterSheetCore;
using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ICSheet5e.Model
{
    using CharacterClasses = List<System.Tuple<CharacterClassType, int>>;

    public enum CharacterClassType
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
        MulticlassCaster
    }

    [DataContract]
    public class Character : CharacterBase
    {
        public Character()
        {
            skills = new SkillList<Skill5e>(Edition.Fifth);
            CharacterClassLevels = new List<CharacterClassItem>();
            inventory = new Inventory<Item>(10);
            inventory.EquipmentChanged += EquipmentChangeHandler;
            CharacterName = "";
            CharacterRace = new Race(Model.Race.RaceType.Human);
            MaxHealth = 1;
            _currentHealth = 1;
            Resistances = new List<DamageType>();
            Immunities = new List<DamageType>();
            _proficiencyBonus = 2;
            SetSkills<Skill5e>(new List<Skill5e>());

            inventory = new Inventory<Item>(AbilityScoreFor(AbilityType.Strength));
        }

        public Character(string characterName, IEnumerable<CharacterClassItem> classLevels, Race race, Dictionary<AbilityType, Ability> abilitySet, int health, List<Skill5e> taggedSkills)
        {
            CharacterName = characterName;
            CharacterRace = race;
            this._abilities = abilitySet; //set in base
            CharacterClassLevels = classLevels.ToList<CharacterClassItem>();
            int totalLevel = classLevels.Sum(x => x.Level);
            _proficiencyBonus = calculateProficiency(totalLevel);
            skills = new SkillList<Skill5e>(Edition.Fifth);
            SetSkills<Skill5e>(taggedSkills);
            MaxHealth = health;
            _currentHealth = health;
            initiativeModifier = calculateInitiative();
            Resistances = new List<DamageType>();
            Immunities = new List<DamageType>();

            inventory = new Inventory<Item>(AbilityScoreFor(AbilityType.Strength));
            InitializeDefenses();
        }

        public Character(string characterName, IEnumerable<CharacterClassItem> levels, Race race)
            : this()
        {
            CharacterClassLevels = levels.ToList<CharacterClassItem>();
            CharacterName = characterName;
            CharacterRace = race;
            int totalLevel = levels.Sum(x => x.Level);
            _proficiencyBonus = calculateProficiency(totalLevel);
            InitializeDefenses();
            InitializeMovement();

            initiativeModifier = calculateInitiative();
        }

        public List<Item> AllCarriedItems
        {
            get { return inventory.FilterContentsBy(x => true); }
        }

        [DataMember]
        public int ArmorClass
        {
            get { return Defenses.Single(x => x.TypeOfDefense == DefenseType.Armor).Value; }
            set
            {
                var ac = Defenses.SingleOrDefault(x => x.TypeOfDefense == DefenseType.Armor);
                if (ac != null) { Defenses.Remove(ac); }
                Defenses.Add(new Defense(value, DefenseType.Armor));
            }
        }

        [DataMember]
        public Race CharacterRace { get; set; }

        [DataMember]
        public int Experience { get; set; }

        public List<MartialFeature> Features { get { return features; } }

        public double Gold { get { return inventory.CurrentGold; } }

        public bool IsSpellCaster
        {
            get { return _isSpellCaster; }
        }

        public Model.ItemDataBase ItemDB { get; set; }

        public List<CharacterClassItem> Levels
        {
            get { return CharacterClassLevels; }
        }

        [DataMember]
        public string Notes { get; set; }

        public List<Spell> PreparedSpells { get { return spellBooks[0].PreparedSpells; } }

        public int Proficiency
        {
            get { return _proficiencyBonus; }
        }

        public List<bool> ProficientDefenses
        {
            get
            {
                return proficientDefensesForCharacter;
            }
        }

        public SkillList<Skill5e> Skills { get { return skills; } }

        public List<SpellCaster> Spellcasting { get { return spellBooks; } }

        public Model.SpellManager SpellDB { get { return _spellDB; } set { _spellDB = value; setSpellCasting(); } }

        public void AddFeature(MartialFeature feature)
        {
            features.Add(feature);
            NotifyPropertyChanged("Features");
        }

        public EncumbranceType AddItemToInventory(Item item)
        {
            inventory.AddItem(item);
            if (inventory.IsEncumbered()) { return EncumbranceType.Light; }
            else if (inventory.IsHeavyEncumbered()) { return EncumbranceType.Heavy; }
            else { return EncumbranceType.None; }
        }

        public int AttackBonusWith(Item weapon)
        {
            var abilities = weapon.AssociatedAbilities;
            var modifier = abilities.Max(x => AbilityModifierFor(x));

            if (weapon.IsProficient)
            {
                return modifier + _proficiencyBonus;
            }
            else
            {
                return modifier;
            }
        }

        public int DamageBonusWith(Item weapon)
        {
            var abilities = weapon.AssociatedAbilities;
            if (weapon.Slot == ItemSlot.Mainhand)
            {
                return abilities.Max(x => AbilityModifierFor(x)) + weapon.EnhancementBonus;
            }
            else
            {
                return weapon.EnhancementBonus;
            }
        }

        public void DoGoldTransaction(double amount)
        {
            if (amount > 0) { inventory.Earn(amount); }
            else
            {
                var success = inventory.Pay(amount);
                if (!success) { throw new System.ArgumentException("Not enough gold for that transaction!"); }
            }
        }

        public void DoLevelUp(List<CharacterClassItem> newLevels, IEnumerable<MartialFeature> newFeatures)
        {
            if (CharacterClassLevels.Count < newLevels.Count) { updateFeatures(newFeatures); }
            CharacterClassLevels = newLevels;
            RecalculateDependentBonuses();
            setSpellCasting();
            NotifyPropertyChanged("Levels");
        }

        private void updateFeatures(IEnumerable<MartialFeature> newFeatures)
        {
            foreach (var feature in newFeatures)
            {
                if (feature.Name == "Hit Dice")
                {
                    features.SingleOrDefault(x => x.Name == "Hit Dice").AddDescriptionText("+" + feature.Description.Remove(0, 9));
                }
                else if (feature.Name.Contains("Proficiency")) { continue; }
                var existingFeature = Features.SingleOrDefault(x => x.Name == feature.Name);
                if (existingFeature != null)
                {
                    existingFeature.AddDescriptionText(feature.Description.Replace(feature.Name + ":", ""));
                }
                else
                {
                    features.Add(feature);
                }
            }
        }

        public void DropItem(Item item)
        {
            inventory.DropItemStack(item);
        }

        public void Equip(Item item)
        {
            inventory.EquipItem(item);
            if (item.Slot == ItemSlot.Armor || (item.Name.Contains("Shield") && item.Slot == ItemSlot.Offhand)) { RecalculateArmorClass(); }
            if (item.IsEquipped) { item.IsEquipped = false; }
            else { item.IsEquipped = true; }
        }

        public void EquipmentChangeHandler(object sender, EquipmentChangedEventArgs e)
        {
            System.Console.WriteLine(e.Items.ToString());
        }

        public Item EquippedItemForSlot(ItemSlot slot)
        {
            if (inventory.EquippedItems.Keys.Contains(slot))
            {
                return inventory.EquippedItems[slot];
            }
            return null;
        }

        public List<Item> ItemsMatching(System.Func<Item, bool> predicate)
        {
            return inventory.FilterContentsBy(predicate);
        }

        //Public API starts here
        //from base class:
        //public void mutateAbilityScore(AbilityType ability, int newScore)
        //public int abilityModifierFor(AbilityType ability)
        //public int AbilityScoreFor(AbilityType ability)
        //
        //public int MaxHealth { get; protected set; }
        //public int CurrentHealth { get { return _currentHealth; } }
        //public void TakeDamage(IDamage damage)
        //public void HealDamage(int amount)
        //
        //public VisionType Vision { get { return _vision; } }
        //public int Movement { get; set; }
        public void RecalculateArmorClass()
        {
            var armor = EquippedItemForSlot(ItemSlot.Armor) as ArmorItem;
            var shield = EquippedItemForSlot(ItemSlot.Offhand) as ArmorItem;
            if (armor != null)
            {
                var armorBonus = armor.ArmorBonus + Math.Max(Math.Min(AbilityModifierFor(AbilityType.Dexterity), armor.MaxDexBonus), 0);
                ArmorClass = armorBonus;
            }
            else if (Levels.SingleOrDefault(x => x.Matches(CharacterClassType.Barbarian)) != null) //unarmored defense
            {
                ArmorClass = 10 + AbilityModifierFor(AbilityType.Dexterity) + AbilityModifierFor(AbilityType.Constitution);
            }
            else if (Levels.SingleOrDefault(x => x.Matches(CharacterClassType.Monk)) != null)
            {
                ArmorClass = 10 + AbilityModifierFor(AbilityType.Dexterity) + AbilityModifierFor(AbilityType.Wisdom);
            }
            else if (Levels.SingleOrDefault(x => x.Matches(CharacterClassType.Sorcerer)) != null && features.SingleOrDefault(x => x.Name =="Draconic Resilience") != null)
            {
                ArmorClass = 13 + AbilityModifierFor(AbilityType.Dexterity);
            }
            else
            {
                ArmorClass = 10 + AbilityModifierFor(AbilityType.Dexterity);
            }
            if (shield != null)
            {
                ArmorClass += 2 + shield.EnhancementBonus;
            }
        }

        public void RecalculateDependentBonuses()
        {
            InitializeDefenses(true);
            initiativeModifier = calculateInitiative();
            spellBooks[0].AdjustMaxPreparedSpells(this);
            spellBooks[0].SetSpellAttackDetails(this);
        }

        public void RecalculateSkillsAfterAbilityScoreChange(List<Skill5e> skills)
        {
            SetSkills<Skill5e>(skills);
        }

        public void RemoveItemFromInventory(Item item)
        {
            inventory.RemoveItem(item);
        }

        public void setSpellCasting() //update to support multiclassing
        {
            if (spellBooks.Count > 0)
            {
                var current = spellBooks[0];
                spellBooks.Clear();
                spellBooks.Add(SpellCaster.ConstructFromExisting(current, this, CharacterClassLevels, SpellDB));
            } //update existing model
            else
            {
                spellBooks.Add(SpellCaster.Construct(CharacterClassLevels, this, SpellDB));
            }
            _isSpellCaster = (spellBooks[0].SpellAttackModifier != 0);
            NotifyPropertyChanged("AvailableSpellSlots");
            NotifyPropertyChanged("PreparedSpellsCount");
        }

        public void TakeLongRest()
        {
            _currentHealth = MaxHealth;
            if (spellBooks.Count > 0)
            {
                foreach (var book in spellBooks)
                {
                    book.RecoverAllSpellSlots();
                }
            }
            NotifyPropertyChanged("CurrentHealth");
            NotifyPropertyChanged("AvailableSpellSlots");
        }

        public bool TryUseFeature(IClassFeature feature)
        {
            if (!features.Contains(feature)) { throw new System.ArgumentException(string.Format("This character does not have the {0} feature", feature.Name)); }
            return feature.TryUseFeature(); //sideEffects
        }

        protected override void SetSkills<T>(List<T> taggedSkills)
        {
            List<Skill5e> skillBonuses = new List<Skill5e>();

            foreach (var name in Skills.SkillNames)
            {
                if (skills.SkillForName(name) == null)
                {
                    Skills.SetSkillBonusFor(new Skill5e(name, 0, false));
                }
            }

            foreach (var s in taggedSkills)
            {
                AbilityType associatedAbility = skills.AbilityFor(s.Name);
                if (s.IsTagged)
                {
                    s.Bonus = AbilityModifierFor(associatedAbility) + Proficiency;
                }
                else
                {
                    s.Bonus = AbilityModifierFor(associatedAbility);
                }
            }
            this.skills.SetAllSkillBonuses(taggedSkills as List<Skill5e>);
        }

        [DataMember]
        private bool _isSpellCaster = false;

        [DataMember]
        private int _proficiencyBonus = 2;

        [DataMember]
        private Dictionary<CharacterClassType, List<DefenseType>> _proficientDefenses = new Dictionary<CharacterClassType, List<DefenseType>>()
        {
            {CharacterClassType.ArcaneTrickster, new List<DefenseType>() {DefenseType.Dexterity, DefenseType.Intelligence}},
            {CharacterClassType.Barbarian, new List<DefenseType>() {DefenseType.Strength, DefenseType.Constitution}},
            {CharacterClassType.Bard, new List<DefenseType>() {DefenseType.Dexterity, DefenseType.Charisma}},
            {CharacterClassType.Cleric, new List<DefenseType>() {DefenseType.Wisdom, DefenseType.Charisma}},
            {CharacterClassType.Druid, new List<DefenseType>() {DefenseType.Intelligence, DefenseType.Wisdom}},
            {CharacterClassType.Fighter, new List<DefenseType>() {DefenseType.Strength, DefenseType.Constitution}},
            {CharacterClassType.EldritchKnight, new List<DefenseType>() {DefenseType.Strength, DefenseType.Constitution}},
            {CharacterClassType.Rogue, new List<DefenseType>() {DefenseType.Dexterity, DefenseType.Intelligence}},
            {CharacterClassType.Paladin, new List<DefenseType>() {DefenseType.Wisdom, DefenseType.Charisma}},
            {CharacterClassType.Monk, new List<DefenseType>() {DefenseType.Strength, DefenseType.Dexterity}},
            {CharacterClassType.Ranger, new List<DefenseType>() {DefenseType.Strength, DefenseType.Dexterity}},
            {CharacterClassType.Sorcerer, new List<DefenseType>() {DefenseType.Charisma, DefenseType.Constitution}},
            {CharacterClassType.Warlock, new List<DefenseType>() {DefenseType.Wisdom, DefenseType.Charisma}},
            {CharacterClassType.Wizard, new List<DefenseType>() {DefenseType.Intelligence, DefenseType.Wisdom}}
        };

        private Model.SpellManager _spellDB;

        [DataMember]
        private List<CharacterClassItem> CharacterClassLevels;

        [DataMember]
        private List<MartialFeature> features = new List<MartialFeature>();

        [DataMember]
        private Inventory<Item> inventory;

        [DataMember]
        private List<bool> proficientDefensesForCharacter = new List<bool>();

        [DataMember]
        private SkillList<Skill5e> skills;

        [DataMember]
        private List<SpellCaster> spellBooks = new List<SpellCaster>();

        #region Spells

        public Tuple<List<int>, List<int>> SpellSlots
        {
            get
            {
                if (spellBooks.Count == 0) { return SpellCaster.Empty; }
                return spellBooks[0].Slots;
            }
        }

        public void CastSpell(Spell spell, int asLevel)
        {
            var book = spellBooks.FirstOrDefault(x => x.HasSpellPrepared(spell));
            if (book != null && book.CanCastSpell(asLevel))
            {
                book.CastSpell(asLevel);
                NotifyPropertyChanged("SpellSlots");
            }
            else { throw new System.ArgumentException("Can't cast that spell as that level."); }
        }

        #endregion Spells

        private int calculateInitiative()
        {
            var bonus = AbilityModifierFor(AbilityType.Dexterity);
            if (CharacterClassLevels.Where(x => (x.Matches(CharacterClassType.Bard) && x.Level >= 2)).Count() > 0)
            {
                bonus += _proficiencyBonus / 2;
            }
            return bonus;
        }

        private int calculateProficiency(int level)
        {
            return (level - 1) / 4 + 2; //integer division
        }

        private bool HasProficiencyIn(DefenseType d)
        {
            try
            {
                return _proficientDefenses[CharacterClassLevels[0].ClassType].Contains(d);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return false; //no classes have been set yet
            }
        }

        private void InitializeDefenses(bool resetAll = false)
        {
            if (resetAll)
            {
                _defenses.Clear();
                proficientDefensesForCharacter.Clear();
            }

            int str = (AbilityModifierFor(AbilityType.Strength)) + (HasProficiencyIn(DefenseType.Strength) ? _proficiencyBonus : 0);
            Defenses.Add(new Defense(str, DefenseType.Strength));
            proficientDefensesForCharacter.Add(HasProficiencyIn(DefenseType.Strength));

            int dex = (AbilityModifierFor(AbilityType.Dexterity)) + (HasProficiencyIn(DefenseType.Dexterity) ? _proficiencyBonus : 0);
            Defenses.Add(new Defense(dex, DefenseType.Dexterity));
            proficientDefensesForCharacter.Add(HasProficiencyIn(DefenseType.Dexterity));

            int con = (AbilityModifierFor(AbilityType.Constitution)) + (HasProficiencyIn(DefenseType.Constitution) ? _proficiencyBonus : 0);
            Defenses.Add(new Defense(con, DefenseType.Constitution));
            proficientDefensesForCharacter.Add(HasProficiencyIn(DefenseType.Constitution));

            int intel = (AbilityModifierFor(AbilityType.Intelligence)) + (HasProficiencyIn(DefenseType.Intelligence) ? _proficiencyBonus : 0);
            Defenses.Add(new Defense(intel, DefenseType.Intelligence));
            proficientDefensesForCharacter.Add(HasProficiencyIn(DefenseType.Intelligence));

            int wis = (AbilityModifierFor(AbilityType.Wisdom)) + (HasProficiencyIn(DefenseType.Wisdom) ? _proficiencyBonus : 0);
            Defenses.Add(new Defense(wis, DefenseType.Wisdom));
            proficientDefensesForCharacter.Add(HasProficiencyIn(DefenseType.Wisdom));

            int cha = (AbilityModifierFor(AbilityType.Charisma)) + (HasProficiencyIn(DefenseType.Charisma) ? _proficiencyBonus : 0);
            Defenses.Add(new Defense(cha, DefenseType.Charisma));
            proficientDefensesForCharacter.Add(HasProficiencyIn(DefenseType.Charisma));

            RecalculateArmorClass();
        }

        private void InitializeMovement()
        {
            if (CharacterRace.Value == Race.RaceType.Dwarf || CharacterRace.SuperType == Race.RaceType.Dwarf ||
                CharacterRace.SuperType == Race.RaceType.Halfling || CharacterRace.Value == Race.RaceType.Halfling ||
                CharacterRace.Value == Race.RaceType.Gnome || CharacterRace.SuperType == Race.RaceType.Gnome)
            {
                Movement = 25;
            }
            else if (CharacterRace.Value == Race.RaceType.WoodElf)
            {
                Movement = 35;
            }
            else
            {
                Movement = 30;
            }
        }

        #region INotifyPropertyChanged Implementation

        public event PropertyChangedEventHandler PropertyChanged;

        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }

        #endregion INotifyPropertyChanged Implementation
    }
}