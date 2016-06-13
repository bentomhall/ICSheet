using ICSheetCore;
using ICSheetCore.Data;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICSheetIOS.Utility
{
    public class CharacterCreationManager
    {
        SpellManager _spellDB;
        XMLFeatureFactory _featureFactory;

        public CharacterCreationManager()
        {
            var spellList = FileManager.GetResourceString(ResourceType.SpellList);
            var spellDetails = FileManager.GetResourceString(ResourceType.SpellDetail);
            var classFeatures = FileManager.GetResourceString(ResourceType.ClassFeatures);
            var raceFeatures = FileManager.GetResourceString(ResourceType.RaceFeatures);
            _spellDB = new SpellManager(spellList, spellDetails, isXML:true);
            _featureFactory = new XMLFeatureFactory(raceFeatures, classFeatures);

        }

        public PlayerCharacter CreateCharacter(CharacterData data)
        {
            var factory = new CharacterFactory(data.Name, _spellDB, _featureFactory);
            return factory.BuildFromStoredData(data);
        }

        public PlayerCharacter CreateNewCharacter(string name, string race, string subrace, string startingClass)
        {
            var factory = new CharacterFactory(name, _spellDB, _featureFactory);
            factory.AssignRace(race, subrace);
            factory.AssignClassLevels(new Dictionary<string, int> { { startingClass, 1 } });
            return factory.ToPlayerCharacter(null);
        }

    }
}
