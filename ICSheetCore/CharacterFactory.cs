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

        private SpellManager _spellDB;
        private XMLFeatureFactory _featureFactory;
        private PCRace _race;
        private PlayerClassAggregate _classes;
        private string _alignment;
        private string _background;

        /// <summary>
        /// Creates an instance of a PlayerCharacterFactory
        /// </summary>
        /// <param name="name"></param>
        /// <param name="spellDB"></param>
        /// <param name="featureFactory"></param>
        public CharacterFactory(string name, SpellManager spellDB, XMLFeatureFactory featureFactory)
        {
            _characterName = name;
            _spellDB = spellDB;
            _featureFactory = featureFactory;
        }

        /// <summary>
        /// Required. Assigns the new character a race with the appropriate racial features.
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="subraceName"></param>
        public void AssignRace(string baseName, string subraceName)
        {
            var features = _featureFactory.ExtractRacialFeatures(baseName, subraceName);
            _race = new PCRace(baseName, subraceName, features);
        }

        /// <summary>
        /// Required. Assigns the classes and levels to the character along with the appropriate class features.
        /// </summary>
        /// <param name="classesAndLevels"></param>
        public void AssignClassLevels(IDictionary<string, int> classesAndLevels)
        {
            var classes = new List<PlayerCharacterClassDetail>();
            foreach (KeyValuePair<string, int> entry in classesAndLevels)
            {
                var f = _featureFactory.ExtractFeaturesFor(entry.Key);
                classes.Add(new PlayerCharacterClassDetail(entry.Key, entry.Value, f));
            }
            _classes = new PlayerClassAggregate(classes, _spellDB);
        }

        /// <summary>
        /// Optional.
        /// </summary>
        /// <param name="value"></param>
        public void AssignBackground(string value)
        {
            _background = value;
        }

        /// <summary>
        /// Optional
        /// </summary>
        /// <param name="value"></param>
        public void AssignAlignment(string value)
        {
            _alignment = value;
        }


        /// <summary>
        /// Creates a new PlayerCharacter instance. Throws InvalidOperationException if AssignRace and AssignClassLevels not called first.
        /// </summary>
        /// <returns></returns>
        public PlayerCharacter ToPlayerCharacter()
        {
            if (_race == null || _classes ==  null)
            {
                throw new InvalidOperationException("Must set race and classes before building");
            }
            var c = new PlayerCharacter(_characterName, _race, _classes);
            c.Alignment = _alignment ?? "";
            c.Background = _background ?? "";
            return c;
        }
    }
}
