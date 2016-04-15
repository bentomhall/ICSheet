using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ICSheetCore
{
    public class CharacterFactory
    {
        private string _characterName;
        private Race _characterRace;
        private List<CharacterClassItem> _characterLevels;
        private ItemDataBase _itemDB;
        private SpellManager _spellDB;
        private XMLFeatureFactory _featureFactory;

        public CharacterFactory(string name, Race race, IEnumerable<CharacterClassItem> levels, ItemDataBase items, SpellManager spells, XMLFeatureFactory features)
        {
            _featureFactory = features;
            _characterLevels = new List<CharacterClassItem>(levels);
            _characterName = name;
            _characterRace = race;
            _itemDB = items;
            _spellDB = spells;
        }

        public Character Build()
        {
            var c = new Character(_characterName, _characterLevels, _characterRace);
            c.ItemDB = _itemDB;
            c.SpellDB = _spellDB;
            var raceFeatures = _featureFactory.RacialFeatures(_characterRace);
            foreach (var f in raceFeatures)
            {
                c.AddFeature(f);
            }

            foreach (var clsLvl in _characterLevels)
            {
                var classFeatures = _featureFactory.ClassFeatures(clsLvl.ClassType);
                foreach (var f in classFeatures) { c.AddFeature(f); }
            }
            return c;
        }

    }
}
