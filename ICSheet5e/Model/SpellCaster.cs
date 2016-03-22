using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using Interactive_Character_Sheet_Core;

namespace ICSheet5e.Model
{
    [DataContract]
    public class SpellCaster: IClassFeature, ICSheet5e.Model.ISpellCaster
    {
        public int MinimumLevel { get { return 1; } }
        public void Clear() { spellBook.UnprepareAllSpells(); }
        public void AddDescriptionText(string newText) { return; }
        static public Tuple<List<int>,List<int>> Empty
        {
            get
            {
                return new Tuple<List<int>, List<int>>(new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 }, new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 });
            }
        }

        static private Dictionary<CharacterClassType, string> nameForClass = new Dictionary<CharacterClassType,string>()
        {
            { CharacterClassType.Barbarian, "Barbarian" },
            { CharacterClassType.Bard, "Bard" },
            { CharacterClassType.Cleric, "Cleric" },
            { CharacterClassType.Druid, "Druid" },
            { CharacterClassType.Fighter, "Fighter" },
            { CharacterClassType.Monk, "Monk"},
            { CharacterClassType.Paladin, "Paladin" },
            { CharacterClassType.Ranger, "Ranger" },
            { CharacterClassType.Rogue, "Rogue" },
            { CharacterClassType.Sorcerer, "Sorcerer" },
            { CharacterClassType.Warlock, "Warlock" },
            { CharacterClassType.Wizard, "Wizard" },
            { CharacterClassType.EldritchKnight, "Eldtritch Knight"},
            { CharacterClassType.ArcaneTrickster, "Arcane Trickster"}
        };

        [DataMember]
        private List<CharacterClassType> subTypes;

        public SpellCaster(CharacterClassType type, int level, Model.SpellManager spellDB, IEnumerable<CharacterClassType> subTypes = null)
        {
            className = type;
            SetSpellSlots(level);
            if (subTypes != null) { this.subTypes = subTypes.ToList<CharacterClassType>(); }
            spellBook = new SpellBook(spellDB, type, this.subTypes);
            RecoverAllSpellSlots();
        }

        public static SpellCaster Construct(List<Tuple<CharacterClassType, int>> levels, Character source, Model.SpellManager spellDB)
        {
            var numberOfCastingClasses = levels.Count(x => SpellSlotsByLevel.CastingTypeForClassType[x.Item1] != SpellSlotsByLevel.CastingType.None);
            if (numberOfCastingClasses == 0)
            {
                var sp = new SpellCaster(levels[0].Item1, levels[0].Item2, spellDB);
                sp.SpellAttackModifier = 0;
                sp.SpellDC = 0;
                sp.maxPreparedSpells = 0;
                return sp;
            }
            
            else if (numberOfCastingClasses == 1)
            {
                var type = levels.Where(x => SpellSlotsByLevel.CastingTypeForClassType[x.Item1] != SpellSlotsByLevel.CastingType.None).First();
                var sp = new SpellCaster(type.Item1, type.Item2, spellDB);
                var abilityMod = source.abilityModifierFor(SpellSlotsByLevel.CastingAbilityFor(levels[0].Item1));
                sp.SpellAttackModifier = source.Proficiency + abilityMod;
                sp.SpellDC = 8 + source.Proficiency + abilityMod;
                sp.maxPreparedSpells = SpellSlotsByLevel.MaximumPreparedSpells(type.Item1, type.Item2, abilityMod);
                return sp;
            }
            else
            {
                var type = CharacterClassType.MultiClassCaster;
                var subTypes = levels.Where(x => SpellSlotsByLevel.CastingTypeForClassType[x.Item1] != SpellSlotsByLevel.CastingType.None);
                var typeWithHighestLevel = levels.OrderByDescending(x =>x.Item2).First();
                var level = castingLevel(levels);
                var sp = new SpellCaster(type, level, spellDB, subTypes.Select(x => x.Item1));
                var abilityMod = source.abilityModifierFor(SpellSlotsByLevel.CastingAbilityFor(typeWithHighestLevel.Item1));
                sp.SpellDC = 8 + source.Proficiency + abilityMod;
                sp.SpellAttackModifier = source.Proficiency + abilityMod;
                sp.maxPreparedSpells = SpellSlotsByLevel.MaximumPreparedSpells(typeWithHighestLevel.Item1, typeWithHighestLevel.Item2, abilityMod);
                return sp;
            }
        }

        static public SpellCaster ConstructFromExisting(SpellCaster caster, Character source, List<Tuple<CharacterClassType, int>> levels, SpellManager spellDB)
        {
            var castingClasses = levels.Where(x => SpellSlotsByLevel.CastingTypeForClassType[x.Item1] != SpellSlotsByLevel.CastingType.None);
            if (castingClasses.Count() == 1 && SpellSlotsByLevel.CastingTypeForClassType[caster.className] != SpellSlotsByLevel.CastingType.None) { return caster; } //no change
            else if (castingClasses.Count() == 1 && SpellSlotsByLevel.CastingTypeForClassType[caster.className] == SpellSlotsByLevel.CastingType.None)
            {
                return SpellCaster.Construct(levels, source, spellDB); //no known spells to worry about
            }
            else
            {
                var sc = SpellCaster.Construct(levels, source, spellDB);
                foreach (var spell in caster.spellBook.AllKnownSpells)
                {
                    sc.AddSpell(spell);
                }
                return sc;
            }
        }

        static private int castingLevel(List<Tuple<CharacterClassType, int>> levels)
        {
            var output = 0;
            foreach (var item in levels)
            {
                switch (SpellSlotsByLevel.CastingTypeForClassType[item.Item1])
                {
                    case SpellSlotsByLevel.CastingType.Full:
                        output += item.Item2;
                        break;
                    case SpellSlotsByLevel.CastingType.Half:
                        output += item.Item2 / 2;
                        break;
                    case SpellSlotsByLevel.CastingType.Martial:
                        output += item.Item2 / 3;
                        break;
                    default:
                        break; //warlock levels don't advance caster level
                }
            }
            return output;
        }

        [DataMember] private CharacterClassType className;
        public string Name { get { return String.Format("Spell Caster: {0}", nameForClass[className]); } }
        public string Uses { get { return "Special (Spell Slots)"; } }
        public string Description
        {
            get { return String.Format("Casts spells as a {0}", className); }
        }

        public void AdjustLevel(List<Tuple<CharacterClassType,int>> newLevels)
        {
            var newLevel = castingLevel(newLevels);
            SetSpellSlots(newLevel);
            maxPreparedSpells += 1;
        }

        public int MaxPreparedSpells 
        { 
            get { return maxPreparedSpells; }
            set { maxPreparedSpells = value; }
        }

        #region slots
        [DataMember]
        private List<int> totalSpellSlots = new List<int>();

        [DataMember]
        private List<int> availableSpellSlots = new List<int>();
        #endregion

        private void SetSpellSlots(int level)
        {
            totalSpellSlots = SpellSlotsByLevel.SlotsForClassAndLevel(className, level);
            availableSpellSlots = new List<int>(totalSpellSlots);
        }

        public bool CanCastSpell(int ofLevel)
        {
            return availableSpellSlots[ofLevel - 1] > 0;
        }

        public Tuple<List<int>, List<int>> Slots
        {
            get { return new Tuple<List<int>, List<int>>(totalSpellSlots, availableSpellSlots); }
        }

        public void CastSpell(int ofLevel)
        {
            if (availableSpellSlots[ofLevel - 1 ] == 0) return;
            availableSpellSlots[ofLevel - 1] -= 1;
            return;
        }

        public bool HasSpellPrepared(Spell spell)
        {
            return spell.IsPrepared;
        }

        public void RecoverSpellSlots(int ofLevel)
        {
            availableSpellSlots[ofLevel] = totalSpellSlots[ofLevel]; //levels are 1 indexed
        }

        public void RecoverAllSpellSlots()
        {
            for (int i=1; i < 10; i++)
            {
                RecoverSpellSlots(i - 1);
            }
        }

        public void ResetUses()
        {
            RecoverAllSpellSlots();
        }
        [DataMember]
        private SpellBook spellBook;
        [DataMember]
        private int maxPreparedSpells = 99; //fake, not currently working
        [DataMember]
        private int currentPrepared = 0;

        public void AddSpell(Spell spell)
        {
            spellBook.AddKnownSpell(spell);
        }

        public List<Spell> AllSpellsForLevel(int level)
        {
            return spellBook.AllSpellsFor(level);
        }

        public bool IsSpellKnown(Spell spell)
        {
            return spellBook.IsSpellKnown(spell);
        }

        public void PrepareSpells(List<Spell> spells)
        {
            if (spells.Count > maxPreparedSpells) { throw new System.ArgumentException("Too many spells to prepare!"); }
            spellBook.UnprepareAllSpells();
            foreach (var spell in spells)
            {
                spellBook.ToggleSpellPreparation(spell);
                currentPrepared += 1;
            }
        }

        public bool PrepareSpell(Spell spell)
        {
            if (currentPrepared == maxPreparedSpells && spell.Level != 0) { return false; } //cantrips don't count
            spellBook.ToggleSpellPreparation(spell);
            return true;
        }

        private void AddSlot(int ofLevel, int number=1)
        {
            totalSpellSlots[ofLevel + 1] += number; //spells are 1 indexed
            return;
        }

        [DataMember]
        public int SpellAttackModifier { get; set; }
        [DataMember]
        public int SpellDC { get; set; }

        public bool TryUseFeature()
        {
            return SpellSlotsByLevel.CastingTypeForClassType[className] != SpellSlotsByLevel.CastingType.None;
        }

        public List<Spell> PreparedSpells
        {
            get { return spellBook.AllPreparedSpells; }
        }

        public void AdjustMaxPreparedSpells(Character source)
        {
            var highestCaster = source.Levels.Where(x => SpellSlotsByLevel.CastingTypeForClassType[x.Item1] != SpellSlotsByLevel.CastingType.None ).OrderByDescending(y => y.Item2).First();
            MaxPreparedSpells = SpellSlotsByLevel.MaximumPreparedSpells(highestCaster.Item1, highestCaster.Item2, source.abilityModifierFor(SpellSlotsByLevel.CastingAbilityFor(highestCaster.Item1)));
        }

        public void SetSpellAttackDetails(Character source)
        {
            var highestCaster = source.Levels.Where(x => SpellSlotsByLevel.CastingTypeForClassType[x.Item1] != SpellSlotsByLevel.CastingType.None ).OrderByDescending(y => y.Item2).First();
            var mod = source.abilityModifierFor(SpellSlotsByLevel.CastingAbilityFor(highestCaster.Item1));
            SpellAttackModifier = source.Proficiency + mod;
            SpellDC = 8 + SpellAttackModifier;
        }
    }

    static class SpellSlotsByLevel
    {
        public static Dictionary<int, List<int>> FullCaster = new Dictionary<int,List<int>>()
        {
            { 1, new List<int>() { 2, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 2, new List<int>() { 3, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 3, new List<int>() { 4, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 4, new List<int>() { 4, 3, 0, 0, 0, 0, 0, 0, 0 }},
            { 5, new List<int>() { 4, 3, 2, 0, 0, 0, 0, 0, 0 }},
            { 6, new List<int>() { 4, 3, 3, 0, 0, 0, 0, 0, 0 }},
            { 7, new List<int>() { 4, 3, 3, 1, 0, 0, 0, 0, 0 }},
            { 8, new List<int>() { 4, 3, 3, 2, 0, 0, 0, 0, 0 }},
            { 9, new List<int>() { 4, 3, 3, 3, 1, 0, 0, 0, 0 }},
            { 10, new List<int>() { 4, 3, 3, 3, 2, 0, 0, 0, 0 }},
            { 11, new List<int>() { 4, 3, 3, 3, 2, 1, 0, 0, 0 }},
            { 12, new List<int>() { 4, 3, 3, 3, 2, 1, 0, 0, 0 }},
            { 13, new List<int>() { 4, 3, 3, 3, 2, 1, 1, 0, 0 }},
            { 14, new List<int>() { 4, 3, 3, 3, 2, 1, 1, 0, 0 }},
            { 15, new List<int>() { 4, 3, 3, 3, 2, 1, 1, 1, 0 }},
            { 16, new List<int>() { 4, 3, 3, 3, 2, 1, 1, 1, 0 }},
            { 17, new List<int>() { 4, 3, 3, 3, 2, 1, 1, 1, 1 }},
            { 18, new List<int>() { 4, 3, 3, 3, 3, 1, 1, 1, 1 }},
            { 19, new List<int>() { 4, 3, 3, 3, 3, 2, 1, 1, 1 }},
            { 20, new List<int>() { 4, 3, 3, 3, 3, 2, 2, 1, 1 }}
        };

        public static Dictionary<int, List<int>> HalfCaster = new Dictionary<int,List<int>>()
        {
            { 1, new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 2, new List<int>() { 2, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 3, new List<int>() { 3, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 4, new List<int>() { 3, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 5, new List<int>() { 4, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 6, new List<int>() { 4, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 7, new List<int>() { 4, 3, 0, 0, 0, 0, 0, 0, 0 }},
            { 8, new List<int>() { 4, 3, 0, 0, 0, 0, 0, 0, 0 }},
            { 9, new List<int>() { 4, 3, 2, 0, 0, 0, 0, 0, 0 }},
            { 10, new List<int>() { 4, 3, 2, 0, 0, 0, 0, 0, 0 }},
            { 11, new List<int>() { 4, 3, 3, 0, 0, 0, 0, 0, 0 }},
            { 12, new List<int>() { 4, 3, 3, 0, 0, 0, 0, 0, 0 }},
            { 13, new List<int>() { 4, 3, 3, 1, 0, 0, 0, 0, 0 }},
            { 14, new List<int>() { 4, 3, 3, 1, 0, 0, 0, 0, 0 }},
            { 15, new List<int>() { 4, 3, 3, 2, 0, 0, 0, 0, 0 }},
            { 16, new List<int>() { 4, 3, 3, 2, 0, 0, 0, 0, 0 }},
            { 17, new List<int>() { 4, 3, 3, 3, 1, 0, 0, 0, 0 }},
            { 18, new List<int>() { 4, 3, 3, 3, 1, 0, 0, 0, 0 }},
            { 19, new List<int>() { 4, 3, 3, 3, 2, 0, 0, 0, 0 }},
            { 20, new List<int>() { 4, 3, 3, 3, 2, 0, 0, 0, 0 }}
        };

        public static Dictionary<int, List<int>> Warlock = new Dictionary<int,List<int>>()
        {
            { 1, new List<int>() { 1, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 2, new List<int>() { 2, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 3, new List<int>() { 0, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 4, new List<int>() { 0, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 5, new List<int>() { 0, 0, 2, 0, 0, 0, 0, 0, 0 }},
            { 6, new List<int>() { 0, 0, 2, 0, 0, 0, 0, 0, 0 }},
            { 7, new List<int>() { 0, 0, 0, 2, 0, 0, 0, 0, 0 }},
            { 8, new List<int>() { 0, 0, 0, 2, 0, 0, 0, 0, 0 }},
            { 9, new List<int>() { 0, 0, 0, 0, 2, 0, 0, 0, 0 }},
            { 10, new List<int>() { 0, 0, 0, 0, 2, 0, 0, 0, 0 }},
            { 11, new List<int>() { 0, 0, 0, 0, 3, 0, 0, 0, 0 }},
            { 12, new List<int>() { 0, 0, 0, 0, 3, 0, 0, 0, 0 }},
            { 13, new List<int>() { 0, 0, 0, 0, 3, 0, 0, 0, 0 }},
            { 14, new List<int>() { 0, 0, 0, 0, 3, 0, 0, 0, 0 }},
            { 15, new List<int>() { 0, 0, 0, 0, 3, 0, 0, 0, 0 }},
            { 16, new List<int>() { 0, 0, 0, 0, 3, 0, 0, 0, 0 }},
            { 17, new List<int>() { 0, 0, 0, 0, 4, 0, 0, 0, 0 }},
            { 18, new List<int>() { 0, 0, 0, 0, 4, 0, 0, 0, 0 }},
            { 19, new List<int>() { 0, 0, 0, 0, 4, 0, 0, 0, 0 }},
            { 20, new List<int>() { 0, 0, 0, 0, 4, 0, 0, 0, 0 }}
        };

        public static Dictionary<int, List<int>> Martial = new Dictionary<int,List<int>>()
        {
            { 1, new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 2, new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 3, new List<int>() { 2, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 4, new List<int>() { 3, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 5, new List<int>() { 3, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 6, new List<int>() { 3, 0, 0, 0, 0, 0, 0, 0, 0 }},
            { 7, new List<int>() { 4, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 8, new List<int>() { 4, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 9, new List<int>() { 4, 2, 0, 0, 0, 0, 0, 0, 0 }},
            { 10, new List<int>() { 4, 3, 0, 0, 0, 0, 0, 0, 0 }},
            { 11, new List<int>() { 4, 3, 0, 0, 0, 0, 0, 0, 0 }},
            { 12, new List<int>() { 4, 3, 2, 0, 0, 0, 0, 0, 0 }},
            { 13, new List<int>() { 4, 3, 2, 0, 0, 0, 0, 0, 0 }},
            { 14, new List<int>() { 4, 3, 2, 0, 0, 0, 0, 0, 0 }},
            { 15, new List<int>() { 4, 3, 3, 0, 0, 0, 0, 0, 0 }},
            { 16, new List<int>() { 4, 3, 3, 0, 0, 0, 0, 0, 0 }},
            { 17, new List<int>() { 4, 3, 3, 0, 0, 0, 0, 0, 0 }},
            { 18, new List<int>() { 4, 3, 3, 0, 0, 0, 0, 0, 0 }},
            { 19, new List<int>() { 4, 3, 3, 1, 0, 0, 0, 0, 0 }},
            { 20, new List<int>() { 4, 3, 3, 1, 0, 0, 0, 0, 0 }}
        };

        static private Dictionary<CharacterClassType, Dictionary<int, List<int>>> spellSlotsByClass = new Dictionary<CharacterClassType, Dictionary<int, List<int>>>()
        {
            { CharacterClassType.Bard, SpellSlotsByLevel.FullCaster },
            { CharacterClassType.Cleric, SpellSlotsByLevel.FullCaster },
            { CharacterClassType.Druid, SpellSlotsByLevel.FullCaster },
            { CharacterClassType.EldritchKnight, SpellSlotsByLevel.Martial },
            { CharacterClassType.Paladin, SpellSlotsByLevel.HalfCaster },
            { CharacterClassType.Sorcerer, SpellSlotsByLevel.FullCaster },
            { CharacterClassType.Warlock, SpellSlotsByLevel.Warlock },
            { CharacterClassType.Wizard, SpellSlotsByLevel.FullCaster },
            { CharacterClassType.ArcaneTrickster, SpellSlotsByLevel.Martial },
            { CharacterClassType.MultiClassCaster, FullCaster}
        };

        static public Dictionary<CharacterClassType, CastingType> CastingTypeForClassType = new Dictionary<CharacterClassType, CastingType>()
        {
            { CharacterClassType.ArcaneTrickster, CastingType.Martial },
            { CharacterClassType.Bard, CastingType.Full },
            { CharacterClassType.Cleric, CastingType.Full },
            { CharacterClassType.Druid, CastingType.Full },
            { CharacterClassType.EldritchKnight, CastingType.Martial },
            { CharacterClassType.Paladin, CastingType.Half },
            { CharacterClassType.Ranger, CastingType.Half },
            { CharacterClassType.Rogue, CastingType.None },
            { CharacterClassType.Sorcerer, CastingType.Full },
            { CharacterClassType.Warlock, CastingType.Warlock },
            { CharacterClassType.Wizard, CastingType.Full },
            { CharacterClassType.Barbarian, CastingType.None },
            { CharacterClassType.Fighter, CastingType.None },
            { CharacterClassType.MultiClassCaster, CastingType.Full}
        };

        static public List<int> SlotsForClassAndLevel(CharacterClassType type, int level)
        {
            if (CastingTypeForClassType[type] != CastingType.None)
            {
                return spellSlotsByClass[type][level];
            }
            else
            {
                return new List<int>() { 0, 0, 0, 0, 0, 0, 0, 0, 0 };
            }
        }

        public enum CastingType
        {
            Full,
            Half,
            Martial,
            Warlock,
            None
        }

        private static Dictionary<CharacterClassType, AbilityType> CastingAbilities = new Dictionary<CharacterClassType, AbilityType>()
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

        public static AbilityType CastingAbilityFor(CharacterClassType type)
        {
            if (CastingAbilities.Keys.Contains(type))
            {
                return CastingAbilities[type];
            }
            else
            {
                return AbilityType.None;
            }
        }

        private static Dictionary<CharacterClassType, List<int>> extraSpellsByClassAndLevel = new Dictionary<CharacterClassType, List<int>>()
        {
            {CharacterClassType.Cleric, new List<int>() {2, 2, 4, 4, 6, 6, 8, 8, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10, 10}},
            {CharacterClassType.Paladin, new List<int>() {0, 0, 2, 2, 4, 4, 4, 4, 6, 6, 6, 6, 8, 8, 8, 8, 10, 10, 10, 10 }},
            {CharacterClassType.Druid, new List<int>(){0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}},
            {CharacterClassType.Wizard, new List<int>(){0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0, 0}}

        };

        public static int MaximumPreparedSpells(CharacterClassType classType, int level, int abilityModifier)
        {
            int maxSpells = 0;
            switch (classType)
            {
                case CharacterClassType.Bard:
                case CharacterClassType.Sorcerer:
                case CharacterClassType.EldritchKnight:
                case CharacterClassType.ArcaneTrickster:
                case CharacterClassType.Ranger:
                case CharacterClassType.Warlock:
                    maxSpells = 99; //all known spells are prepared
                    break;
                default:
                    maxSpells = Math.Max(level + abilityModifier, 1) + extraSpellsByClassAndLevel[classType][level];
                    break;
            }

            return maxSpells;
        }
    }
}
