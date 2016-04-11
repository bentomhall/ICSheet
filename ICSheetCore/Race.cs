using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ICSheetCore
{
    [DataContract]
    public class Race
    {
        public enum RaceType
        {
            Dwarf,
            HillDwarf,
            MountainDwarf,
            Elf,
            HighElf,
            WoodElf,
            DarkElf,
            Halfling,
            Lightheart,
            Stout,
            Human,
            VariantHuman,
            Dragonborn,
            Gnome,
            ForestGnome,
            RockGnome,
            HalfElf,
            HalfOrc,
            Tiefling
        }

        [DataMember]
        private Dictionary<RaceType, string> _raceNameMap = new Dictionary<RaceType, string>()
        {
            { RaceType.DarkElf, "Dark Elf"},
            { RaceType.Dragonborn, "Dragonborn"},
            { RaceType.Dwarf, "Dwarf"},
            { RaceType.Elf, "Elf"},
            { RaceType.ForestGnome, "Forest Gnome"},
            { RaceType.Gnome, "Gnome"},
            { RaceType.HalfElf, "Half-Elf"},
            { RaceType.Halfling, "Halfling"},
            { RaceType.HalfOrc, "Half-Orc"},
            { RaceType.HighElf, "High Elf"},
            { RaceType.HillDwarf, "Hill Dwarf"},
            { RaceType.Human, "Human"},
            { RaceType.Lightheart, "Lightheart"},
            { RaceType.MountainDwarf, "Mountain Dwarf"},
            { RaceType.RockGnome, "Rock Gnome"},
            { RaceType.Stout, "Stout"},
            { RaceType.Tiefling, "Tiefling"},
            { RaceType.VariantHuman, "Variant Human"},
            { RaceType.WoodElf, "Wood Elf"},
        };

        public RaceType? SuperType
        {
            get
            {
                switch (Value)
                {
                    case RaceType.DarkElf:
                    case RaceType.HighElf:
                    case RaceType.WoodElf:
                        return RaceType.Elf;
                    case RaceType.ForestGnome:
                    case RaceType.RockGnome:
                        return RaceType.Gnome;
                    case RaceType.MountainDwarf:
                    case RaceType.HillDwarf:
                        return RaceType.Dwarf;
                    case RaceType.Lightheart:
                    case RaceType.Stout:
                        return RaceType.Halfling;
                    case RaceType.VariantHuman:
                        return RaceType.Human;
                    default:
                        return null;
                    
                }
            }
        }

        [DataMember]
        public RaceType Value { get; private set; }

        public override string ToString()
        {
            if (_raceNameMap.Keys.Contains(this.Value)) { return _raceNameMap[this.Value]; }
            else { return "Unknown Race"; }
        }

        public Race(RaceType race) { Value = race; }

        public int BaseMovement
        {
            get
            {
                if (Value == RaceType.Dwarf || SuperType == RaceType.Dwarf ||
    SuperType == RaceType.Halfling || Value == RaceType.Halfling ||
    Value == RaceType.Gnome || SuperType == RaceType.Gnome)
                {
                    return 25;
                }
                else if (Value == RaceType.WoodElf)
                {
                    return 35;
                }
                else
                {
                    return 30;
                }
            }
        }
    }
}
