using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSheetCore;

namespace ICCoreTest
{
    class FeatureFactoryStub : IFeatureFactory
    {
        public IEnumerable<string> ExtractClassNames()
        {
            return new List<string> { "Test" };
        }

        public IEnumerable<IFeature> ExtractFeaturesFor(string pcClassName)
        {
            if (pcClassName != "Test")
            {
                throw new NotImplementedException("Only class Test is implemented for testing");
            }
            var features = new List<IFeature>();
            features.Add(new ClassFeature("Saving Throw Proficiencies", 1, false, "Dexterity, Constitution"));
            return features;
        }

        public IEnumerable<string> ExtractRaceNames()
        {
            return new List<string> { "TestRace" };
        }

        public IEnumerable<IFeature> ExtractRacialFeatures(string race, string subrace)
        {
            return new List<IFeature> { new RaceFeature("test race feature", "test") };
        }

        public IEnumerable<string> ExtractSubclassesFor(string _selectedClass)
        {
            return new List<string> { "Subclass Test" };
        }

        public IEnumerable<IFeature> ExtractSubclassFeaturesFor(string _selectedSubclass)
        {
            var features = new List<IFeature>();
            features.Add(new ClassFeature("Test Feature", 1, false, "Test"));
            return features;
        }

        public IEnumerable<string> ExtractSubraceNames(string raceName)
        {
            return new List<string> { "Subrace TestRace" };
        }
    }
}
