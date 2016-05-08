using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ICSheetCore.Data
{
    /// <summary>
    /// Required information for serialization. Pure DTO.
    /// </summary>
    [DataContract, KnownType(typeof(ArmorItem)), KnownType(typeof(WeaponItem)), KnownType(typeof(Item)), KnownType(typeof(ClassFeature))]
    public class CharacterData
    {
        /// <summary></summary>
        [DataMember]
        public string Name { get; set; }

        /// <summary></summary>
        [DataMember]
        public string Alignment { get; set; }

        /// <summary></summary>
        [DataMember]
        public string Background { get; set; }

        /// <summary></summary>
        [DataMember]
        public IDictionary<AbilityType, int> AbilityScores { get; set; }

        /// <summary></summary>
        [DataMember]
        public Tuple<string, string> RaceInformation { get; set; }

        /// <summary></summary>
        [DataMember]
        public IDictionary<string, int> ClassLevelInformation { get; set; }

        /// <summary></summary>
        [DataMember]
        public IEnumerable<IItem> Items { get; set; }

        /// <summary></summary>
        [DataMember]
        public IDictionary<ItemSlot, IItem> EquippedItems { get; set; }

        /// <summary></summary>
        [DataMember]
        public Money Cash { get; set; }

        /// <summary></summary>
        [DataMember]
        public IEnumerable<IFeature> CustomFeatures { get; set; }

        /// <summary></summary>
        [DataMember]
        public string Notes { get; set; }

        /// <summary></summary>
        [DataMember]
        public int Experience { get; set; }

        /// <summary>(Max, Current, Temporary)</summary>
        [DataMember]
        public Tuple<int, int, int> HealthInformation { get; set; }

        /// <summary></summary>
        [DataMember]
        public IDictionary<DefenseType, int> DefenseOverrides { get; set; }

        /// <summary></summary>
        [DataMember]
        public IEnumerable<Spell> KnownSpells { get; set; }

        /// <summary></summary>
        [DataMember]
        public IDictionary<string, ProficiencyType> Skills{ get; set;}

        /// <summary></summary>
        [DataMember]
        public IEnumerable<int> CurrentSpellSlots { get; set; }
    }
}
