using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using System.Resources;
using ICSheetCore;

namespace ICSheet5e.ResourceModifiers
{
    public class CustomPlayerClassSerializer
    {
        private XDocument _classDocument;
        private List<string> _classNames = new List<string>();
        private ResourceFileManager _fileManager = new ResourceFileManager();

        private XElement _constructFeatureNode(IFeature feature)
        {
            var element = new XElement("Feature");
            element.SetAttributeValue("Name", feature.Name);
            element.SetAttributeValue("StartLevel", feature.StartsFromLevel);
            element.SetAttributeValue(XNamespace.Xml + "space", "preserve");
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

        private void save()
        {
            var path = _fileManager.CreatePathForResource("ClassFeatures.xml");
            _classDocument.Save(path);
        }


        internal CustomPlayerClassSerializer(string classData)
        {
            _classDocument = XDocument.Parse(classData);
            _classNames = _classDocument.Root.Elements().Select(x => x.Attribute("Name").Value).ToList();
        }

        internal IEnumerable<string> ClassNames { get { return _classNames; } }

        internal void ConstructBaseClass(string className, IEnumerable<IFeature> features, IEnumerable<SubclassData> subclasses)
        {
            var root = _classDocument.Root;
            var element = new XElement("PCClass");
            element.SetAttributeValue("Name", className);
            foreach (var f in features) { element.Add(_constructFeatureNode(f)); }
            foreach (var sc in subclasses) { element.Add(_constructSubclassNode(sc)); }
            root.Add(element);
            save();
        }

        internal void ConstructSubclassForExistingClass(string className, SubclassData subclassInformation)
        {
            var classElement = _classDocument.Root.Elements().SingleOrDefault(x => x.Attribute("Name").Value == className);
            if (classElement == null) { throw new ArgumentException("Invalid class name supplied."); }
            classElement.Add(_constructSubclassNode(subclassInformation));
            save();
        }
    }
}
