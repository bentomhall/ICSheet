using System.Collections.Generic;
using System.Linq;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace ICSheetCore
{
    /// <summary>
    /// Data source for spell information from XML
    /// </summary>
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
                spell.CastTime = (string)element["CastingTime"];
                spell.IsPrepared = false;
                spell.Duration = (string)element["Duration"];
                _spellDetails.Add(spell);
            }
        }

        private string getElementValueByElementName(XElement e, string name)
        {
            return e.Element(name).Value;
        }

        private void loadXMLSpellDetails(string details)
        {
            var root = XDocument.Parse(details).Root;
            _spellDetails = new List<Spell>();
            foreach (var s in root.Elements("Spell"))
            {
                var spell = new Spell();
                spell.Name = getElementValueByElementName(s, "Name");
                spell.Level = int.Parse(getElementValueByElementName(s, "Level"));
                spell.Range = getElementValueByElementName(s, "Range");
                spell.School = getElementValueByElementName(s, "School");
                spell.Description = getElementValueByElementName(s, "Text");
                spell.Components = getElementValueByElementName(s, "Components");
                spell.CastTime = getElementValueByElementName(s, "Castingtime");
                spell.IsPrepared = false;
                spell.Duration = getElementValueByElementName(s, "Duration");
                _spellDetails.Add(spell);
            }
        }

        [DataMember]
        private List<Spell> _spellDetails;
        [DataMember]
        private List<string> _spellNames;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellNames"></param>
        /// <param name="spellDetails"></param>
        /// <param name="isXML"></param>
        public SpellManager(string spellNames, string spellDetails, bool isXML=false)
        {
            if (isXML)
            {
                loadXMLSpellDetails(spellDetails);
            }
            else
            {
                loadSpellDetails(spellDetails);
            }
            loadSpellNames(spellNames);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="className"></param>
        /// <returns></returns>
        public IEnumerable<string> SpellNamesFor(string className)
        {
            if (!classNameSpellsMap.ContainsKey(className.ToLower())) { return new List<string>(); }
            return classNameSpellsMap[className.ToLower()];
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellName"></param>
        /// <returns></returns>
        public Spell SpellDetailsFor(string spellName)
        {
            if (hasSpellDetails(spellName))
            {
                return _spellDetails.Single(x => x.Name == spellName).DeepCopyInvariants();
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

        /// <summary>
        /// Reloads the details cache after changes to backing store (eg custom spell added).
        /// </summary>
        public void ReloadSpellDetails(string spellList, string spellDetails)
        {
            loadSpellDetails(spellDetails);
            loadSpellNames(spellList);
        }
    }
}
