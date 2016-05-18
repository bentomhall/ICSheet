using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using ICSheetCore;

namespace ICSheet5e.ResourceModifiers
{
    class CustomPlayerClassSerializer
    {
        private XDocument _classDocument;
        private List<string> _classNames = new List<string>();

        private XElement _constructFeatureNode(IFeature feature)
        {
            var element = new XElement("Feature");
            element.SetAttributeValue("Name", feature.Name);
            element.SetAttributeValue("StartLevel", feature.StartsFromLevel);
            element.Value = feature.Description;
            return element;
        }

        private XElement _constructSubclassNode(SubclassData data)
        {
            var element = new XElement("Subclass");
            element.SetAttributeValue("Name", data.Name);
            foreach (var f in data.Features)
            {
                element.Add(_constructFeatureNode(f));
            }
            return element;
        }


        internal CustomPlayerClassSerializer(string classData)
        {
            _classDocument = XDocument.Parse(classData);
            _classNames = _classDocument.Root.Elements().Select(x => x.Attribute("Name").Value).ToList();
        }

        internal IEnumerable<string> ClassNames { get { return _classNames; } }

        internal void ConstructBaseClass(string className, IEnumerable<IFeature> features, IEnumerable<SubclassData> subclasses)
        {

        }
    }
}
