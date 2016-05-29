using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml.Linq;

namespace ICSheet5e.ResourceModifiers
{
    public class CustomSpellSerializer
    {
        private XDocument _spellLists;
        private List<SpellSerializationData> _spellDetails;

        private List<SpellSerializationData> parseDetails(string path)
        {
            var text = System.IO.File.ReadAllText(path);
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
            _spellDetails.Add(item);
            addToSpellList(item.name, toClasses);
        }

        private void addToSpellList(string name, IEnumerable<string> toClasses)
        {
            throw new NotImplementedException();
        }
    }
}
