using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ICSheetCore
{
    /// <summary>
    /// Parses XML features into feature objects.
    /// </summary>
    public class XMLFeatureFactory
    {
        private string _raceFeaturesXML;
        private string _classFeaturesXML;
        private XDocument _classFeatures;
        private XDocument _raceFeatures;

        public XMLFeatureFactory(string raceFeatures, string classFeatures)
        {
            if (raceFeatures == null) { throw new System.ArgumentNullException("raceFeatures"); }
            if (classFeatures == null) { throw new System.ArgumentNullException("classFeatures"); }
            _raceFeaturesXML = raceFeatures;
            _classFeaturesXML = classFeatures;
            _classFeatures = XDocument.Parse(_classFeaturesXML);
            _raceFeatures = XDocument.Parse(_raceFeaturesXML);
        }


        static private XElement FindSingleElementByNameAttribute(string XName, string attributeName, XElement inElement)
        {
            return inElement.Elements(XName).First(x => x.Attribute("Name").Value == attributeName);
        }


        /// <summary>
        /// Gets all valid race names as strings from the provided xml
        /// </summary>
        /// <returns>all valid race names</returns>
        public IEnumerable<string> ExtractRaceNames()
        {
            return _raceFeatures.Root.Elements().Select(x => x.Attribute("Name").Value);
        }

        /// <summary>
        /// Gets all subraces defined for a particular race.
        /// 
        /// </summary>
        /// <param name="raceName">Name of base race</param>
        /// <returns>Names of all subraces</returns>
        public IEnumerable<string> ExtractSubraceNames(string raceName)
        {
            var element = FindSingleElementByNameAttribute("Race", raceName, _raceFeatures.Root);
            return element.Elements("Subrace").Select(x => x.Attribute("Name").Value);
        }

        /// <summary>
        /// Gets all features for the given race/subrace combination. 
        /// </summary>
        /// <param name="race">Name of base race.</param>
        /// <param name="subrace">Name of subrace (if any). Pass null or "" for no subrace.</param>
        /// <returns></returns>
        /// <exception cref="InvalidOperationException"/>
        public IEnumerable<IFeature> ExtractRacialFeatures(string race, string subrace)
        {
            var features = new List<RaceFeature>();
            var baseRaceElement = FindSingleElementByNameAttribute("Race", race, _raceFeatures.Root);
            foreach(var node in baseRaceElement.Elements("Feature"))
            {
                features.Add(extractRacialFeaturesFor(node));
            }
            if (!string.IsNullOrWhiteSpace(subrace))
            {
                var subraceElement = FindSingleElementByNameAttribute("Subrace", subrace, baseRaceElement);
                if (subraceElement.Attribute("Replaces") != null)
                {
                    var replacedFeature = features.Single(x => x.Name == subraceElement.Attribute("Replaces").Value);
                    features.Remove(replacedFeature);
                }
                foreach (var node in subraceElement.Elements())
                {
                    var newFeature = extractRacialFeaturesFor(subraceElement);
                    var oldFeature = features.SingleOrDefault(x => x.Name == newFeature.Name);
                    if (oldFeature != null)
                    {
                        oldFeature.CombineDescriptions(newFeature);
                    }
                    else
                    {
                        features.Add(newFeature);
                    }
                }
            }
            return features;
        }

        private RaceFeature extractRacialFeaturesFor(XElement node)
        {
            var featureName = node.Attribute("Name").Value;
            var text = node.Value;
            return new RaceFeature(featureName, text);
        }

        /// <summary>
        /// Gets all valid class names as strings from the provided xml
        /// </summary>
        /// <returns>all valid class names</returns>
        public IEnumerable<string> ExtractClassNames()
        {
            return _classFeatures.Root.Elements().Select(x => x.Attribute("Name").Value);
        }

        /// <summary>
        /// Extracts an enumerable of all the features related to a single provided player class.
        /// 
        /// </summary>
        /// <param name="pcClassName">The name of the class desired. Must match the output of ExtractClassNames</param>
        /// <returns></returns>
        public IEnumerable<IFeature> ExtractFeaturesFor(string pcClassName)
        {
            var features = new List<IFeature>();
            var element = FindSingleElementByNameAttribute("PCClass", pcClassName, _classFeatures.Root);
            if (element == null) { throw new ArgumentException("Invalid class name supplied."); }
            foreach (var node in element.Elements())
            {
                features.Add(featureFactoryFrom(node));
            }
            return features;

        }

        /// <summary>
        /// All valid skill names for 5th edition
        /// </summary>
        static public IEnumerable<string> SkillNames = new List<string>()
        {
            "Acrobatics",
            "Animal Handling",
            "Arcana",
            "Athletics",
            "Deception",
            "History",
            "Insight",
            "Intimidation",
            "Medicine",
            "Nature",
            "Perception",
            "Performance",
            "Persuasion",
            "Religion",
            "Sleight of Hand",
            "Stealth",
            "Survival"
        };

        private IFeature featureFactoryFrom(XElement element)
        {
            var featureName = element.Attribute("Name").Value;
            if (featureName == "Spellcasting")
            {
                return extractSpellcastingFeature(element);
            }
            else { return extractClassFeature(element); }
        }

        private IFeature extractClassFeature(XElement element)
        {
            var featureName = element.Attribute("Name").Value;
            var text = element.Value;
            var isInheritable = (element.Attribute("MulticlassInheritable").Value == "True" ? true : false);
            var startingLevel = int.Parse(element.Attribute("StartLevel").Value);
            return new ClassFeature(featureName, startingLevel, isInheritable, text);
        }

        private IFeature extractSpellcastingFeature(XElement element)
        {
            var name = element.Parent.Attribute("Name").Value;
            var castingType = element.Element("CastingType").Value;
            var castingAbility = (AbilityType)Enum.Parse(typeof(AbilityType), element.Element("CastingAttribute").Value);
            var isPrepared = (element.Element("IsPreparedCaster").Value == "True" ? true: false);
            var cantrips = element.Element("Cantrips").Value.Split(' ').Select(x => int.Parse(x));
            var bonusSpells = element.Element("BonusSpells").Value.Split(' ').Select(x => int.Parse(x));
            var spellsKnown = element.Element("Spells").Value.Split(' ').Select(x => int.Parse(x));
            return new SpellcastingFeature(name, castingType, bonusSpells, castingAbility, isPrepared, cantrips, spellsKnown);
        }
    }
}
