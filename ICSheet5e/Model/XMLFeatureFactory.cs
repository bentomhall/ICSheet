using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using InteractiveCharacterSheetCore;

namespace ICSheet5e.Model
{
    class XMLFeatureFactory
    {
        private string _raceFeaturesXML = ICSheet5e.Properties.Resources.RacialFeatures;
        private string _classFeaturesXMLPath = ICSheet5e.Properties.Resources.ClassFeatures;

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

        public List<MartialFeature> ClassFeatures(CharacterClassType classType)
        {
            var doc = XDocument.Parse(_classFeaturesXMLPath);
            XElement classRoot;
            if (classType == CharacterClassType.ArcaneTrickster) { classRoot = FindSingleElementByNameAttribute("PCClass", "Rogue", doc.Root); }
            else if (classType == CharacterClassType.EldritchKnight) { classRoot = FindSingleElementByNameAttribute("PCClass", "Fighter", doc.Root); }
            else { classRoot = FindSingleElementByNameAttribute("PCClass", classType.ToString(), doc.Root); }
            return ExtractClassFeatures(classRoot);
        }

        private XElement FindSingleElementByNameAttribute(string XName, string attributeName, XElement inElement)
        {
            return inElement.Elements(XName).First(x => x.Attribute("Name").Value == attributeName);
        }

        private List<MartialFeature> ExtractClassFeatures(XElement e)
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
    }
}
