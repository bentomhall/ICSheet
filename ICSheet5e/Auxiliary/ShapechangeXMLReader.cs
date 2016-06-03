using ICSheet5e.ViewModels;
using ICSheetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;

namespace ICSheet5e.Auxiliary
{
    class ShapechangeXMLReader
    {
        private string _xmlPath = @"Resources/WildShapes.xml";

        private ShapechangeData generateData(XElement e)
        {
            return new ShapechangeData();
        }

        private XElement findElement(string formName, XDocument doc)
        {
            var root = doc.Root;
            return root.Elements("Creature").Single(x => x.Attribute("Name").Value.ToLower() == formName.ToLower()); //throws exception on shape not found.
        }

        private IEnumerable<IFeature> extractFeatures(XElement e)
        {
            var output = new List<IFeature>();
            foreach (var d in e.Element("Features").Elements("Feature"))
            {
                output.Add(extractSingleFeature(d));
            }
            return output;
        }

        private IFeature extractSingleFeature(XElement d)
        {
            var name = d.Attribute("Name").Value;
            var text = d.Value;
            return new RaceFeature(name, text);
        }

        private IEnumerable<AttackViewModel> extractAttacks(XElement e)
        {
            var output = new List<AttackViewModel>();
            foreach (var attack in e.Element("Attacks").Elements("Attack"))
            {
                var vm = new AttackViewModel();
                vm.Name = attack.Attribute("Name").Value;
                vm.AttackBonus = int.Parse(attack.Element("AttackBonus").Value);
                var dmg = attack.Element("BaseDamage").Value.Split(' ');
                vm.BaseDamage = dmg[0];
                vm.StaticBonus = int.Parse(dmg[1]);
                output.Add(vm);
            }
            return output;
        }
    }
}
