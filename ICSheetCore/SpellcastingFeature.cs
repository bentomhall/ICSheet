using System;
using System.Collections.Generic;

namespace ICSheetCore
{
    /// <summary>
    /// A common feature for spellcasters
    /// </summary>
    public class SpellcastingFeature : ISpellcastingFeature
    {
        private SpellcastingLookup.CastingType _type;
        private AbilityType _castingAbility;
        private bool _isPreparedCaster;
        private string _spellBookName;
        private List<int> _bonusSpells;
        private List<int> _spellsKnown;
        private List<int> _cantripsKnown;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <param name="castingType"></param>
        /// <param name="bonusSpells"></param>
        /// <param name="castingAbility"></param>
        /// <param name="isPreparedCaster"></param>
        /// <param name="cantripsKnown"></param>
        /// <param name="spellsKnown"></param>
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

        /// <summary>
        /// 
        /// </summary>
        public SpellcastingLookup.CastingType CasterType
        {
            get
            {
                return _type;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public AbilityType CastingAbility
        {
            get
            {
                return _castingAbility;
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return "Spellcasting";
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public bool ParticipatesInMulticlassSpellcasting
        {
            get
            {
                return !(_spellBookName == "Warlock");
            }
        }

        /// <summary>
        /// 
        /// </summary>
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

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public IEnumerable<int> SpellSlots(int level)
        {
            return SpellcastingLookup.SpellSlotsFor(_type, level);
        }

        /// <summary>
        /// 
        /// </summary>
        public int StartsFromLevel
        {
            get
            {
                if (_type == SpellcastingLookup.CastingType.Half) { return 2; }
                else if (_type == SpellcastingLookup.CastingType.Martial) { return 3; }
                return 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int BonusSpells(int level)
        {
            return _bonusSpells[level - 1]; //levels are 1 indexed, 
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int CantripsKnown(int level)
        {
            return _cantripsKnown[level - 1];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <returns></returns>
        public int SpellsKnown(int level)
        {
            return _spellsKnown[level - 1];
        }

        /// <summary>
        /// 
        /// </summary>
        public bool IsPreparedCaster { get { return _isPreparedCaster; } }

        /// <summary>
        /// 
        /// </summary>
        public string FullDescription
        {
            get
            {
                return Description;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="level"></param>
        /// <param name="abilityBonus"></param>
        /// <returns></returns>
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
