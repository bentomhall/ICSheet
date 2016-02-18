using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using Interactive_Character_Sheet_Core;

namespace ICSheet5e.Model
{
    using CharacterClasses = Dictionary<CharacterClassType, int>;
    [DataContract]
    public class Character: CharacterBase
    {
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

        public int Proficiency
        {
            get { return _proficiencyBonus; }
        }

        public Character()
        {
            skills = new SkillList<Skill5e>(Edition.Fifth);
            CharacterClassLevels = new CharacterClasses();
            inventory = new Inventory<Item>(10);
            CharacterName = "";
            Race = "";
            MaxHealth = 1;
            SetSkills<Skill5e>(new List<Skill5e>());
            setSpellCasting();
        }

        public Character(string characterName, CharacterClasses classLevels, string race, Dictionary<AbilityType, Ability> abilitySet, int health, List<Skill5e> taggedSkills )
        {
            CharacterName = characterName;
            Race = race;
            this.abilities = abilitySet; //set in base
            CharacterClassLevels = classLevels;
            int totalLevel = classLevels.Sum(x => x.Value);
            _proficiencyBonus = calculateProficiency(totalLevel);
            skills = new SkillList<Skill5e>(Edition.Fifth);
            SetSkills<Skill5e>(taggedSkills);
            MaxHealth = health;
            inventory = new Inventory<Item>(abilitySet[AbilityType.Strength].score);
            setSpellCasting();

        }

        [DataMember] private Inventory<Item> inventory;
        [DataMember] private List<IClassFeature> features = new List<IClassFeature>();
        [DataMember] private List<SpellCaster> spellBooks = new List<SpellCaster>();
        private int calculateProficiency(int level)
        {
            return (level - 1) / 4 + 2; //integer division
        }

        protected override void SetSkills<T>(List<T> taggedSkills) 
        {
            List<Skill5e> skillBonuses = new List<Skill5e>();
          
            foreach (string skillName in skills.getSkillNames())
            {
                AbilityType associatedAbility = skills.abilityFor(skillName);
                if (taggedSkills.Count(x => x.name == skillName) != 0) 
                {
                    skillBonuses.Add( new Skill5e(skillName, _proficiencyBonus + abilities[associatedAbility].modifier));
                }
                else
                {
                    skillBonuses.Add(new Skill5e(skillName, abilities[associatedAbility].modifier));
                }
            }
            this.skills.setAllSkillBonuses(skillBonuses);
        }

        void setSpellCasting()
        {
            foreach (KeyValuePair<CharacterClassType, int> entry in CharacterClassLevels)
            {
                if (castingClasses.Contains(entry.Key))
                {
                    var castingModifier = abilityModifierFor(castingAbilities[entry.Key]);
                    SpellCaster book = new SpellCaster(entry.Key, entry.Value);
                    book.SpellAttackModifier = castingModifier + _proficiencyBonus;
                    book.SpellDC = 8 + castingModifier + _proficiencyBonus;
                    spellBooks.Add(book);
                }
            }
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

        public void DoGoldTransaction(int amount)
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
            inventory.EquippedItems[item.Slot] = item;
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

        public void AddFeature(IClassFeature feature)
        {
            features.Add(feature);
        }
        
        //hooks for serialization

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
