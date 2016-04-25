using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace ICSheetCore.Data
{
    [DataContract, KnownType(typeof(ArmorItem)), KnownType(typeof(WeaponItem)), KnownType(typeof(Item))]
    public class CharacterData
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Alignment { get; set; }

        [DataMember]
        public string Background { get; set; }

        [DataMember]
        public IDictionary<AbilityType, int> AbilityScores { get; set; }

        [DataMember]
        public Tuple<string, string> RaceInformation { get; set; }

        [DataMember]
        public IDictionary<string, int> ClassLevelInformation { get; set; }

        [DataMember]
        public IEnumerable<IItem> Items { get; set; }

        [DataMember]
        public IDictionary<ItemSlot, IItem> EquippedItems { get; set; }

        [DataMember]
        public Money Cash { get; set; }

        [DataMember]
        public IEnumerable<IFeature> CustomFeatures { get; set; }

        [DataMember]
        public string Notes { get; set; }

        [DataMember]
        public int Experience { get; set; }

        [DataMember]
        public Tuple<int, int, int> HealthInformation { get; set; }

        [DataMember]
        public IDictionary<DefenseType, int> DefenseOverrides { get; set; }

        [DataMember]
        public IEnumerable<Spell> KnownSpells { get; set; }

        [DataMember]
        public IDictionary<string, ProficiencyType> Skills{ get; set;}

        [DataMember]
        public IEnumerable<int> CurrentSpellSlots { get; set; }
    }
}
