using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.IO;
using Interactive_Character_Sheet_Core;

namespace ICSheet5e.Model
{
    class XMLFeatureFactory
    {
        private string _raceFeaturesXML = ICSheet5e.Properties.Resources.RacialFeatures;
        private string _classFeaturesXMLPath = ICSheet5e.Properties.Resources.ClassFeatures;

        public List<IClassFeature> RacialFeatures(Race forRace)
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

        private List<IClassFeature> ExtractRacialFeatures(XElement e, string subRaceName)
        {
            List<IClassFeature> features = new List<IClassFeature>();
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

        public List<IClassFeature> ClassFeatures(CharacterClassType classType)
        {
            var doc = XDocument.Parse(_classFeaturesXMLPath);
            XElement classRoot;
            if (classType == CharacterClassType.ArcaneTrickster) { classRoot = doc.Descendants("Rogue").First(); }
            else if (classType == CharacterClassType.EldritchKnight) { classRoot = doc.Descendants("Fighter").First(); }
            else { classRoot = doc.Descendants(classType.ToString()).First(); }
            return ExtractClassFeatures(classRoot);
        }

        private List<IClassFeature> ExtractClassFeatures(XElement e)
        {
            List<IClassFeature> features = new List<IClassFeature>();
            foreach (var node in e.Descendants("Feature"))
            {
                var featureName = node.Attribute("Name").Value;
                var text = node.Value;
                var uses = "";
                var minLevel = int.Parse(node.Attribute("StartLevel").Value);
                var f = new MartialFeature(featureName, text, uses);
                f.MinimumLevel = minLevel;
                features.Add(f);
            }
            return features;
        }
    }
}
