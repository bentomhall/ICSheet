using System;
using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace ICSheetCore
{
    [DataContract]
    public class SpellManager
    {
        private bool hasSpellDetails(string spellName)
        {
            return _spellDetails.Any(x => x.Name == spellName);
        }

        [DataMember]
        private Dictionary<string, List<string>> classNameSpellsMap;

        private void loadSpellNames(string spellNames) 
        {
            _spellNames = new List<string>();
            var doc = XDocument.Parse(spellNames);
            classNameSpellsMap = new Dictionary<string, List<string>>();
            foreach (var cls in doc.Descendants("CastingClass"))
            {
                var clsName = cls.Attribute("Name").Value;
                classNameSpellsMap[clsName] = new List<string>();
                foreach (var spell in cls.Elements())
                {
                    var name = spell.Attribute("Name").Value;
                    if (!_spellNames.Contains(name)) { _spellNames.Add(name); }
                    classNameSpellsMap[clsName].Add(name);
                }
            }
        }

        private void loadSpellDetails(string details)
        {
            var spellDetailsJ = Newtonsoft.Json.Linq.JArray.Parse(details);
            _spellDetails = new List<Spell>();
            foreach (var element in spellDetailsJ)
            {
                var spell = new Spell();
                spell.Name = (string)element["name"];
                spell.Level = int.Parse((string)element["level"]);
                spell.Range = (string)element["Range"];
                spell.School = (string)element["school"];
                spell.Description = (string)element["text"];
                spell.Components = (string)element["Components"];
                spell.CastTime = (string)element["Casting Time"];
                spell.IsPrepared = false;
                spell.Duration = (string)element["Duration"];
                _spellDetails.Add(spell);
            }
        }

        [DataMember]
        private List<Spell> _spellDetails;
        [DataMember]
        private List<string> _spellNames;

        public SpellManager(string spellNames, string spellDetails)
        {
            loadSpellNames(spellNames);
            loadSpellDetails(spellDetails);
        }

        public IEnumerable<string> SpellNamesFor(string className)
        {
            return classNameSpellsMap[className];
        }

        public Spell SpellDetailsFor(string spellName)
        {
            if (hasSpellDetails(spellName))
            {
                return _spellDetails.Single(x => x.Name == spellName);
            }
            else
            {
                var spell = new Spell();
                spell.Name = spellName;
                spell.Description = "This spell is not part of the SRD. Look in the appropriate source book (Player's Handbook, etc.)";
                spell.Level = -1;
                spell.Range = "?";
                spell.Components = "?";
                spell.CastTime = "?";
                spell.School = "?";
                return spell;
            }
        }
    }
}
