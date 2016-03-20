using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ICSheet5e.Model
{
    class CharacterFactory
    {
        private string characterName;
        private Race characterRace;
        private List<Tuple<CharacterClassType, int>> characterLevels;
        private ItemDataBase itemDB;
        private SpellManager spellDB;
        private XMLFeatureFactory featureFactory = new XMLFeatureFactory();

        internal CharacterFactory(string name, Race race, IEnumerable<Tuple<CharacterClassType, int>> levels, ItemDataBase items, SpellManager spells)
        {
            characterLevels = new List<Tuple<CharacterClassType,int>>(levels);
            characterName = name;
            characterRace = race;
            itemDB = items;
            spellDB = spells;
        }

        internal Character Build()
        {
            var c = new Character(characterName, characterLevels, characterRace);
            c.ItemDB = itemDB;
            c.SpellDB = spellDB;
            var raceFeatures = featureFactory.RacialFeatures(characterRace);
            foreach (var f in raceFeatures)
            {
                c.AddFeature(f);
            }

            foreach (var clsLvl in characterLevels)
            {
                var classFeatures = featureFactory.ClassFeatures(clsLvl.Item1);
                foreach (var f in classFeatures) { c.AddFeature(f); }
            }
            return c;
        }

    }
}
