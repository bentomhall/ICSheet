using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.IO;

namespace ICSheet5e.ResourceModifiers
{
    public class CustomSpellSerializer
    {
        private XDocument _spellLists;
        private List<SpellSerializationData> _spellDetails;
        private string _spellListPath;
        private string _spellDetailPath;

        private List<SpellSerializationData> parseDetails(string path)
        {
            var text = File.ReadAllText(path);
            var output = JsonConvert.DeserializeObject<List<SpellSerializationData>>(text);
            return output;
        }

        public  CustomSpellSerializer(string spellListPath, string spellDetailPath)
        {
            _spellLists = XDocument.Load(spellListPath);
            _spellDetails = parseDetails(spellDetailPath);
        }

        public void Add(SpellSerializationData item, IEnumerable<string> toClasses)
        {
            if (_spellDetails.Count(x => x.name == item.name) == 0)
            {
                _spellDetails.Add(item);
            }
            addToSpellList(item.name, toClasses);
        }

        private void addToSpellList(string name, IEnumerable<string> toClasses)
        {

            var newElement = new XElement("Spell");
            newElement.SetAttributeValue("Name", name);
            foreach (var cls in toClasses)
            {
                var e = findSpellListFor(cls, _spellLists);
                if (e.Elements("Spell").Count(x => x.Attribute("Name").Value == name) == 0)
                {
                    // only add if it doesn't already exist
                    e.Add(newElement);
                }
                
            }
        }

        private XElement findSpellListFor(string className, XDocument inDocument)
        {
            var element = inDocument.Descendants("CastingClass").SingleOrDefault(x => x.Attribute("Name").Value == className.ToLower());
            return element;
        }

        public void Save()
        {
            var fileManager = new ResourceFileManager();
            _spellListPath = fileManager.CreatePathForResource("spell_list.xml");
            _spellDetailPath = fileManager.CreatePathForResource("SpellList5e.json");
        
            _spellLists.Save(_spellListPath);
            var output = JsonConvert.SerializeObject(_spellDetails);
            File.WriteAllText(_spellDetailPath, output);
        }

        public string RawSpellList()
        {
            return File.ReadAllText(_spellListPath);
        }

        public string RawSpellDetails()
        {
            return File.ReadAllText(_spellDetailPath);
        }
    }
}
