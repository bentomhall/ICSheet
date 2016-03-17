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
        private string _classFeaturesXMLPath;

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
            }
            var racialElement = doc.Elements("Race").SingleOrDefault(x => x.Attribute("Name").Value == raceName);
            return ExtractRacialFeatures(racialElement, subRaceName);
        }

        public List<IClassFeature> ClassFeatures(CharacterClassType classType)
        {
            throw new NotImplementedException();
        }

        private List<IClassFeature> ExtractRacialFeatures(XElement e, string subRaceName)
        {
            List<IClassFeature> features = new List<IClassFeature>();
            foreach (var node in e.Descendants("Feature"))
            {
                string featureName = node.Attribute("Name").Value;
                string text = node.Value;
                string uses = "";
                if (node.Attribute("HasLimitedUse").Value == "True")
                {
                    uses = "1/day";
                }
                var feature = new MartialFeature(featureName, text, uses);
                features.Add(feature);
            }
            if (subRaceName != null)
            {
                var subRace = e.Descendants("SubRace").SingleOrDefault(x => x.Attribute("Name").Value == subRaceName);
                
                var subFeatures = ExtractRacialFeatures(subRace, null);
                var subBonus = subFeatures.SingleOrDefault(x => x.Name == "Ability Bonus");
                var abilityScoreFeature = features.SingleOrDefault(x => x.Name == "Ability Bonus");
                if (subRace.LastAttribute.Value == "Replaces") //variant human
                {
                    features.Remove(features.SingleOrDefault(x => x.Name == "Ability Bonus"));
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
    }
}
