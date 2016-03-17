using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheet5e.Model
{
    class Race
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

        public RaceType Race { get; private set; }

        public override string ToString()
        {
            if (_raceNameMap.Keys.Contains(this.Race)) { return _raceNameMap[this.Race]; }
            else { return "Unknown Race"; }
        }

        public Race(RaceType race) { this.Race = race; }
    }
}
