using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheetCore
{
    class SpellcastingFeature : ISpellcastingFeature
    {
        private SpellcastingLookup.CastingType _type;
        private AbilityType _castingAbility;
        private bool _isPreparedCaster;
        private string _spellBookName;
        private List<int> _bonusSpells;
        private List<int> _spellsKnown;
        private List<int> _cantripsKnown;

        public SpellcastingFeature(string className, string castingType, IEnumerable<int> bonusSpells, AbilityType castingAbility, bool isPreparedCaster, IEnumerable<int> cantripsKnown, IEnumerable<int> spellsKnown)
        {
            _type = (SpellcastingLookup.CastingType)Enum.Parse(typeof(SpellcastingLookup.CastingType), castingType);
            _castingAbility = castingAbility;
            _spellBookName = className;
            _isPreparedCaster = isPreparedCaster;
            _bonusSpells = new List<int>(bonusSpells);
            _spellsKnown = new List<int>(spellsKnown);
            _cantripsKnown = new List<int>(cantripsKnown);
        }

        public SpellcastingLookup.CastingType CasterType
        {
            get
            {
                return _type;
            }
        }

        public AbilityType CastingAbility
        {
            get
            {
                return _castingAbility;
            }
        }

        public string Description
        {
            get
            {
                return $"Cast spells as a {SpellBookName} ({_castingAbility}-based {CasterType} caster)";
            }
        }

/// <summary>
        /// This one makes no sense for spellcasting. Use ParticipatesInMulticlassSpellcasting instead.
        /// </summary>
        public bool IsMulticlassInheritable
        {
            get
            {
                return false; //spellcasting follows special multiclass rules.
            }
        }

        public string Name
        {
            get
            {
                return "Spellcasting";
            }
        }

        public bool ParticipatesInMulticlassSpellcasting
        {
            get
            {
                return !(_spellBookName == "Warlock");
            }
        }

        public string SpellBookName
        {
            get
            {
                switch(_spellBookName)
                {
                    case "Eldritch Knight":
                    case "Arcane Trickster":
                        return "Wizard";
                    default:
                        return _spellBookName;
                }
            }
        }

        public IEnumerable<int> SpellSlots(int level)
        {
            return SpellcastingLookup.SpellSlotsFor(_type, level);
        }

        public int StartsFromLevel
        {
            get
            {
                if (_type == SpellcastingLookup.CastingType.Half) { return 2; }
                else if (_type == SpellcastingLookup.CastingType.Martial) { return 3; }
                return 1;
            }
        }

        public int BonusSpells(int level)
        {
            return _bonusSpells[level - 1]; //levels are 1 indexed, 
        }

        public int CantripsKnown(int level)
        {
            return _cantripsKnown[level - 1];
        }

        public int SpellsKnown(int level)
        {
            return _spellsKnown[level - 1];
        }

        public bool IsPreparedCaster { get { return _isPreparedCaster; } }

        public string FullDescription
        {
            get
            {
                return Description;
            }
        }

        public int SpellsPrepared(int level, int abilityBonus)
        {
            if (_isPreparedCaster)
            {
                int total;
                switch (_type)
                {
                    case SpellcastingLookup.CastingType.Full:
                        total = abilityBonus;
                        break;
                    case SpellcastingLookup.CastingType.Half:
                        total = abilityBonus / 2;
                        break;
                    default:
                        total = 0; //won't hit here normally, the only prepared casters are full/half casters.
                        break;
                }
                return Math.Max(level + total, 1);
            }
            return _spellsKnown[level - 1]; //spontaneous casters have all known spells prepared.
        }
    }
}
