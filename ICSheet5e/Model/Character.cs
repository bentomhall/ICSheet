using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Interactive_Character_Sheet_Core;
using System.Runtime.CompilerServices;
using System.ComponentModel;
using System;

namespace ICSheet5e.Model
{
    using CharacterClasses = List<System.Tuple<CharacterClassType, int>>;
    [DataContract]
    public class Character: CharacterBase
    {
        public Model.SpellManager SpellDB { get; set; }
        public Model.ItemDataBase ItemDB { get; set; }
        private static List<CharacterClassType> castingClasses = new List<CharacterClassType>()
        {
            CharacterClassType.Bard,
            CharacterClassType.Cleric,
            CharacterClassType.Druid,
            CharacterClassType.Paladin,
            CharacterClassType.Ranger,
            CharacterClassType.Sorcerer,
            CharacterClassType.Warlock,
            CharacterClassType.Wizard,
            CharacterClassType.EldritchKnight
        };

        private static Dictionary<CharacterClassType, AbilityType> castingAbilities = new Dictionary<CharacterClassType,AbilityType>()
        {
            {CharacterClassType.ArcaneTrickster, AbilityType.Intelligence},
            {CharacterClassType.Bard, AbilityType.Charisma},
            {CharacterClassType.Cleric, AbilityType.Wisdom},
            {CharacterClassType.Druid, AbilityType.Wisdom},
            {CharacterClassType.EldritchKnight, AbilityType.Intelligence},
            {CharacterClassType.Paladin, AbilityType.Charisma},
            {CharacterClassType.Ranger, AbilityType.Wisdom},
            {CharacterClassType.Sorcerer, AbilityType.Charisma},
            {CharacterClassType.Warlock, AbilityType.Charisma},
            {CharacterClassType.Wizard, AbilityType.Intelligence}
        };

        [DataMember] private CharacterClasses CharacterClassLevels;
        [DataMember] private int _proficiencyBonus = 2;
        [DataMember] private SkillList<Skill5e> skills;
        [DataMember]
        public int ArmorClass 
        {
            get { return Defenses.Single(x => x.type == DefenseType.Armor).value; }
            set
            {
                var ac = Defenses.SingleOrDefault(x => x.type==DefenseType.Armor);
                if (ac != null) { Defenses.Remove(ac); }
                Defenses.Add(new Defense(value, DefenseType.Armor));
            }
        }
        public SkillList<Skill5e> Skills { get { return skills; } }

        public double Gold { get { return inventory.CurrentGold; } }

        public Character()
        {
            skills = new SkillList<Skill5e>(Edition.Fifth);
            CharacterClassLevels = new CharacterClasses();
            inventory = new Inventory<Item>(10);
            inventory.EquipmentChanged += EquipmentChangeHandler;
            CharacterName = "";
            Race = "";
            MaxHealth = 1;
            _currentHealth = 1;
            Resistances = new List<DamageType>();
            Immunities = new List<DamageType>();
            _proficiencyBonus = 2;
            SetSkills<Skill5e>(new List<Skill5e>());
            setSpellCasting();

            inventory = new Inventory<Item>(AbilityScoreFor(AbilityType.Strength));
        }

        public Character(string characterName, CharacterClasses classLevels, string race, Dictionary<AbilityType, Ability> abilitySet, int health, List<Skill5e> taggedSkills )
        {
            CharacterName = characterName;
            Race = race;
            this.abilities = abilitySet; //set in base
            CharacterClassLevels = classLevels;
            int totalLevel = classLevels.Sum(x => x.Item2);
            _proficiencyBonus = calculateProficiency(totalLevel);
            skills = new SkillList<Skill5e>(Edition.Fifth);
            SetSkills<Skill5e>(taggedSkills);
            MaxHealth = health;
            _currentHealth = health;
            initiativeModifier = calculateInitiative();
            Resistances = new List<DamageType>();
            Immunities = new List<DamageType>();

            inventory = new Inventory<Item>(AbilityScoreFor(AbilityType.Strength));
            setSpellCasting();
            InitializeDefenses();
        }

        public Character(string characterName, CharacterClasses levels, string race) : this()
        {
            CharacterClassLevels = levels;
            CharacterName = characterName;
            Race = race;
            InitializeDefenses();
            int totalLevel = levels.Sum(x => x.Item2);
            _proficiencyBonus = calculateProficiency(totalLevel);
            initiativeModifier = calculateInitiative();
        }

        [DataMember] private Inventory<Item> inventory;
        [DataMember] private List<IClassFeature> features = new List<IClassFeature>();
        [DataMember] private List<SpellCaster> spellBooks = new List<SpellCaster>();
        private int calculateProficiency(int level)
        {
            return (level - 1) / 4 + 2; //integer division
        }

        private void InitializeDefenses(bool resetAll=false)
        {
            if (resetAll)
            {
                _defenses.Clear();
                proficientDefensesForCharacter.Clear();
            }
            RecalculateArmorClass();

            int str = (10 + abilityModifierFor(AbilityType.Strength)) + (HasProficiencyIn(DefenseType.Strength) ? _proficiencyBonus : 0);
            Defenses.Add(new Defense(str, DefenseType.Strength));
            proficientDefensesForCharacter.Add(HasProficiencyIn(DefenseType.Strength));

            int dex = (10 + abilityModifierFor(AbilityType.Dexterity)) + (HasProficiencyIn(DefenseType.Dexterity) ? _proficiencyBonus : 0);
            Defenses.Add(new Defense(dex, DefenseType.Dexterity));
            proficientDefensesForCharacter.Add(HasProficiencyIn(DefenseType.Dexterity));

            int con = (10 + abilityModifierFor(AbilityType.Constitution)) + (HasProficiencyIn(DefenseType.Constitution) ? _proficiencyBonus : 0);
            Defenses.Add(new Defense(con, DefenseType.Constitution));
            proficientDefensesForCharacter.Add(HasProficiencyIn(DefenseType.Constitution));

            int intel = (10 + abilityModifierFor(AbilityType.Intelligence)) + (HasProficiencyIn(DefenseType.Intelligence) ? _proficiencyBonus : 0);
            Defenses.Add(new Defense(intel, DefenseType.Intelligence));
            proficientDefensesForCharacter.Add(HasProficiencyIn(DefenseType.Intelligence));

            int wis = (10 + abilityModifierFor(AbilityType.Wisdom)) + (HasProficiencyIn(DefenseType.Wisdom) ? _proficiencyBonus : 0);
            Defenses.Add(new Defense(wis, DefenseType.Wisdom));
            proficientDefensesForCharacter.Add(HasProficiencyIn(DefenseType.Wisdom));

            int cha = (10 + abilityModifierFor(AbilityType.Charisma)) +(HasProficiencyIn(DefenseType.Charisma) ? _proficiencyBonus : 0);
            Defenses.Add(new Defense(cha, DefenseType.Charisma));
            proficientDefensesForCharacter.Add(HasProficiencyIn(DefenseType.Charisma));
        }

        public void EquipmentChangeHandler(object sender, EquipmentChangedEventArgs e)
        {
            System.Console.WriteLine(e.Items.ToString());
        }

        protected override void SetSkills<T>(List<T> taggedSkills) 
        {
            List<Skill5e> skillBonuses = new List<Skill5e>();
          
            foreach (var s in taggedSkills)
            {
                AbilityType associatedAbility = skills.abilityFor(s.name);
                if (s.IsTagged)
                {
                    s.bonus = abilityModifierFor(associatedAbility) + Proficiency;
                }
                else
                {
                    s.bonus = abilityModifierFor(associatedAbility);
                }
            }
            this.skills.setAllSkillBonuses(taggedSkills as List<Skill5e>);
        }

        public void setSpellCasting()
        {
            foreach (System.Tuple<CharacterClassType, int> entry in CharacterClassLevels)
            {
                if (castingClasses.Contains(entry.Item1))
                {
                    var castingModifier = abilityModifierFor(castingAbilities[entry.Item1]);
                    SpellCaster book = new SpellCaster(entry.Item1, entry.Item2, SpellDB);
                    book.SpellAttackModifier = castingModifier + _proficiencyBonus;
                    book.SpellDC = 8 + castingModifier + _proficiencyBonus;
                    spellBooks.Add(book);
                }
            }
        }

        public List<SpellCaster> Spellcasting { get { return spellBooks; } }


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

        //TODO:
        //public inventory management
        //hooks for casting/managing spells
        //hooks for basic attacks

        public int AttackBonusWith(Item weapon)
        {
            var abilities = weapon.AssociatedAbilities;
            var modifier = abilities.Max(x => abilityModifierFor(x));
            
            if (weapon.IsProficient)
            {
                return modifier + _proficiencyBonus;
            } else
            {
                return modifier;
            }
        }

        public int DamageBonusWith(Item weapon)
        {
            var abilities = weapon.AssociatedAbilities;
            if (weapon.Slot == ItemSlot.Mainhand)
            {
                return abilities.Max(x => abilityModifierFor(x)) + weapon.EnhancementBonus;
            }
            else
            {
                return weapon.EnhancementBonus;
            }
        }

        public void RecalculateArmorClass()
        {
            var armor = inventory.EquippedItems[ItemSlot.Armor] as ArmorItem;
            var armorBonus = armor.ArmorBonus + Math.Max(Math.Min(abilityModifierFor(AbilityType.Dexterity), armor.MaxDexBonus), 0);
            ArmorClass = armorBonus;
        }

        public Item EquippedItemForSlot(ItemSlot slot)
        {
            try
            {
                return inventory.EquippedItems[slot];
            }
            catch (KeyNotFoundException)
            {
                return null;
            }
        }

        public void CastSpell(Spell spell, int asLevel)
        {
            var book = spellBooks.FirstOrDefault(x => x.HasSpellPrepared(spell));
            if (book != null && book.CanCastSpell(asLevel))
            {
                book.CastSpell(asLevel);
            }
            else { throw new System.ArgumentException("Can't cast that spell as that level."); }
        }

        public bool TryUseFeature(IClassFeature feature)
        {
            if (!features.Contains(feature)) { throw new System.ArgumentException(string.Format("This character does not have the {0} feature", feature.Name));}
            return feature.TryUseFeature(); //sideEffects
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

        public void Equip(Item item)
        {
            inventory.EquipItem(item);
            if (item.Slot == ItemSlot.Armor) { RecalculateArmorClass(); }
            if (item.IsEquipped) { item.IsEquipped = false; }
            else {item.IsEquipped = true;}
        }

        public EncumbranceType AddItemToInventory(Item item)
        {
            inventory.AddItem(item);
            if (inventory.IsEncumbered()) { return EncumbranceType.Light; }
            else if (inventory.IsHeavyEncumbered()) { return EncumbranceType.Heavy; }
            else { return EncumbranceType.None; }
        }

        public void RemoveItemFromInventory(Item item)
        {
            inventory.RemoveItem(item);
        }

        public void DropItem(Item item)
        {
            inventory.DropItemStack(item);
        }

        public List<Item> AllCarriedItems
        {
            get { return inventory.FilterContentsBy(x => true); }
        }

        public List<Item> ItemsMatching(System.Func<Item, bool> predicate)
        {
            return inventory.FilterContentsBy(predicate);
        }

        public void AddFeature(IClassFeature feature)
        {
            features.Add(feature);
        }

        public int Proficiency
        {
            get { return _proficiencyBonus; }
        }

        public void AddLevel(CharacterClasses type)
        {

        }

        public CharacterClasses Levels
        {
            get { return CharacterClassLevels; }
        }

        [DataMember]
        public int Experience { get; set; }
        
        //hooks for serialization
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

        private bool HasProficiencyIn(DefenseType d)
        {
            try
            {
                return _proficientDefenses[CharacterClassLevels[0].Item1].Contains(d);
            }
            catch (System.ArgumentOutOfRangeException)
            {
                return false; //no classes have been set yet
            }
            
        }
        [DataMember]
        private List<bool> proficientDefensesForCharacter = new List<bool>();
        public List<bool> ProficientDefenses 
        { 
            get 
            {
                return proficientDefensesForCharacter;
            } 
        }

        private int calculateInitiative()
        {
            var bonus = abilityModifierFor(AbilityType.Dexterity);
            if (CharacterClassLevels.Where(x => (x.Item1 == CharacterClassType.Bard && x.Item2 > 2)).Count() > 0)
            {
                bonus += _proficiencyBonus / 2;
            }
            return bonus;
        }

        public void RecalculateDependentBonuses()
        {
            InitializeDefenses(true);
            initiativeModifier = calculateInitiative();
        }

        public void RecalculateSkillsAfterAbilityScoreChange(List<Skill5e> skills)
        {
            SetSkills<Skill5e>(skills);
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
        #endregion

    }

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
    }
}
