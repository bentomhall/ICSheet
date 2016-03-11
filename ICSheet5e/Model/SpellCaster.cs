using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ICSheet5e.Model
{
    [DataContract]
    public class SpellCaster: IClassFeature
    {
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

        public SpellCaster(CharacterClassType type, int level, Model.SpellManager spellDB)
        {
            className = type;
            SetSpellSlots(level);
            spellBook = new SpellBook(spellDB, type);
            RecoverAllSpellSlots();
        }

        [DataMember] private CharacterClassType className;
        public string Name { get { return String.Format("Spell Caster: {0}", nameForClass[className]); } }
        public string Uses { get { return "Special (Spell Slots)"; } }
        public string Description
        {
            get { return String.Format("Casts spells as a {0}",className); }
        }

        #region slots
        [DataMember]
        private List<int> totalSpellSlots = new List<int>();

        [DataMember]
        private List<int> availableSpellSlots = new List<int>();
        #endregion

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
            { CharacterClassType.ArcaneTrickster, SpellSlotsByLevel.Martial }
        };

        private void SetSpellSlots(int level)
        {
            totalSpellSlots = spellSlotsByClass[className][level];
            availableSpellSlots = new List<int>(totalSpellSlots);
        }

        public bool CanCastSpell(int ofLevel)
        {
            return availableSpellSlots[ofLevel] > 0;
        }

        public Tuple<List<int>, List<int>> Slots
        {
            get { return new Tuple<List<int>, List<int>>(totalSpellSlots, availableSpellSlots); }
        }

        public void CastSpell(int ofLevel)
        {
            if (availableSpellSlots[ofLevel] == 0) return;
            availableSpellSlots[ofLevel] -= 1;
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
        private int maxPreparedSpells = 10;
        [DataMember]
        private int currentPrepared = 0;

        public void AddSpell(Spell spell)
        {
            spellBook.AddKnownSpell(spell);
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
            if (currentPrepared == maxPreparedSpells) { return false; }
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
            return true; //can always cast cantrips
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
    }
}
