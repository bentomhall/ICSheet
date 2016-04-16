using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;

namespace ICSheetCore
{
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

        public List<MartialFeature> RacialFeatures(Race forRace)
        {
            var doc = XDocument.Parse(_raceFeaturesXML);
            var subRaceName = forRace.ToString();
            string raceName = null;
            if (forRace.SuperType != null)
            {
                raceName = new Race(forRace.SuperType.Value).ToString(); //ugh
            }
            else
            {
                raceName = subRaceName;
                subRaceName = null;
            }
            var racialElement = doc.Descendants("Race").SingleOrDefault(x => x.Attribute("Name").Value == raceName);
            return ExtractRacialFeatures(racialElement, subRaceName);
        }

        private List<MartialFeature> ExtractRacialFeatures(XElement e, string subRaceName)
        {
            List<MartialFeature> features = new List<MartialFeature>();
            foreach (var node in e.Elements("Feature"))
            {
                string featureName = node.Attribute("Name").Value;
                string text = node.Value;
                string uses = "";
                if (node.Attribute("HasLimitedUse") != null && node.Attribute("HasLimitedUse").Value == "True")
                {
                    uses = "1/day";
                }
                var feature = new MartialFeature(featureName, text, uses);
                features.Add(feature);
            }
            if (subRaceName != null)
            {
                var subRace = e.Elements("Subrace").SingleOrDefault(x => x.Attribute("Name").Value == subRaceName);
                
                var subFeatures = ExtractRacialFeatures(subRace, null);
                var subBonus = subFeatures.SingleOrDefault(x => x.Name == "Ability Bonus");
                var abilityScoreFeature = features.SingleOrDefault(x => x.Name == "Ability Bonus");
                if (subRace.LastAttribute.Name == "Replaces") //variant human
                {
                    var name = subRace.LastAttribute.Value;
                    features.Remove(features.SingleOrDefault(x => x.Name == name));
                }
                else //all other subraces
                {
                    abilityScoreFeature.AddDescriptionText(subBonus.Description);
                    subFeatures.Remove(subBonus);
                }
                
                features.AddRange(subFeatures);
            }
            return features;
        }

        public IEnumerable<MartialFeature> ClassFeatures(CharacterClassType classType)
        {
            var doc = XDocument.Parse(_classFeaturesXML);
            XElement classRoot;
            if (classType == CharacterClassType.ArcaneTrickster) { classRoot = FindSingleElementByNameAttribute("PCClass", "Rogue", doc.Root); }
            else if (classType == CharacterClassType.EldritchKnight) { classRoot = FindSingleElementByNameAttribute("PCClass", "Fighter", doc.Root); }
            else { classRoot = FindSingleElementByNameAttribute("PCClass", classType.ToString(), doc.Root); }
            return ExtractClassFeatures(classRoot);
        }

        static private XElement FindSingleElementByNameAttribute(string XName, string attributeName, XElement inElement)
        {
            return inElement.Elements(XName).First(x => x.Attribute("Name").Value == attributeName);
        }

        static private IEnumerable<MartialFeature> ExtractClassFeatures(XElement e)
        {
            List<MartialFeature> features = new List<MartialFeature>();
            foreach (var node in e.Descendants("Feature"))
            {
                var featureName = node.Attribute("Name").Value;
                var text = node.Value;
                var uses = "";
                var f = new MartialFeature(featureName, text, uses);
                if (node.Attribute("StartLevel") != null) { f.MinimumLevel = int.Parse(node.Attribute("StartLevel").Value); }
                else { f.MinimumLevel = 1; }
                features.Add(f);
            }
            return features;
        }

        public IEnumerable<string> ExtractClassNames()
        {
            return _classFeatures.Root.Elements().Select(x => x.Attribute("Name").Value);
        }

        public IEnumerable<IFeature> ExtractFeaturesFor(string pcClassName)
        {
            var features = new List<IFeature>();
            var element = FindSingleElementByNameAttribute("PCClass", pcClassName, _classFeatures.Root);
            foreach (var node in element.Elements())
            {
                features.Add(FeatureFactoryFrom(node));
            }
            return features;

        }

        private IFeature FeatureFactoryFrom(XElement element)
        {
            var featureName = element.Attribute("Name").Value;
            if (featureName == "Spellcasting")
            {
                return ExtractSpellcastingFeature(element);
            }
            else { return ExtractClassFeature(element); }
        }

        private IFeature ExtractClassFeature(XElement element)
        {
            var featureName = element.Attribute("Name").Value;
            var text = element.Value;
            var isInheritable = (element.Attribute("MulticlassInheritable").Value == "True" ? true : false);
            var startingLevel = int.Parse(element.Attribute("StartLevel").Value);
            return new ClassFeature(featureName, startingLevel, isInheritable, text);
        }

        private IFeature ExtractSpellcastingFeature(XElement element)
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
