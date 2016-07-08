using System.Collections.Generic;

namespace ICSheetCore
{
    public interface IFeatureFactory
    {
        IEnumerable<string> ExtractClassNames();
        IEnumerable<IFeature> ExtractFeaturesFor(string pcClassName);
        IEnumerable<string> ExtractRaceNames();
        IEnumerable<IFeature> ExtractRacialFeatures(string race, string subrace);
        IEnumerable<string> ExtractSubclassesFor(string _selectedClass);
        IEnumerable<IFeature> ExtractSubclassFeaturesFor(string _selectedSubclass);
        IEnumerable<string> ExtractSubraceNames(string raceName);
    }
}