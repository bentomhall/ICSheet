using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Newtonsoft.Json;
using System.Xml.Linq;
using System.Runtime.Serialization;

namespace ICSheet5e.Model
{
    [DataContract]
    public class SpellManager
    {
        private bool canCast(CharacterClassType classType, string spellName)
        {
            return classSpellsMap[classType].Count > 0;
        }
        private bool hasSpellDetails(string spellName)
        {
            return _spellDetails.Any(x => x.Name == spellName);
        }
        [DataMember]
        private Dictionary<CharacterClassType, List<string>> classSpellsMap;

        private void loadSpellNames() 
        {
            _spellNames = new List<string>();
            var resource = ICSheet5e.Properties.Resources.spell_list;
            var doc = XDocument.Parse(resource);
            classSpellsMap = new Dictionary<CharacterClassType, List<string>>();
            foreach (var cls in doc.Descendants("CastingClass"))
            {
                var clsName = cls.Attribute("Name").Value;
                var clsType = classNameClassTypeMap[clsName];
                classSpellsMap[clsType] = new List<string>();
                foreach (var spell in cls.Elements())
                {
                    var name = spell.Attribute("Name").Value;
                    if (!_spellNames.Contains(name)) { _spellNames.Add(name); }
                    classSpellsMap[clsType].Add(name);
                }
            }
        }

        [DataMember]
        private Dictionary<string, CharacterClassType> classNameClassTypeMap = new Dictionary<string, CharacterClassType>()
        {
            {"bard", CharacterClassType.Bard},
            {"cleric", CharacterClassType.Cleric},
            {"druid", CharacterClassType.Druid},
            {"paladin", CharacterClassType.Paladin},
            {"ranger", CharacterClassType.Ranger},
            {"sorcerer", CharacterClassType.Sorcerer},
            {"warlock", CharacterClassType.Warlock},
            {"wizard", CharacterClassType.Wizard}
        };

        private void loadSpellDetails()
        {
            var resource = ICSheet5e.Properties.Resources.SpellList5e;
            var spellDetailsJ = Newtonsoft.Json.Linq.JArray.Parse(resource);
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
                foreach (var caster in castingClasses)
                {
                    var type = classNameClassTypeMap[caster];
                    if (canCast(type, spell.Name))
                    {
                        if (spell.CastableBy == null) { spell.CastableBy = new List<CharacterClassType>(); }
                        spell.CastableBy.Add(type);
                    }
                }
                _spellDetails.Add(spell);
            }
        }

        [DataMember]
        private List<Spell> _spellDetails;
        [DataMember]
        private List<string> _spellNames;

        public SpellManager()
        {
            loadSpellNames();
            loadSpellDetails();
        }
        [DataMember]
        private List<string> castingClasses = new List<String>() { "bard", "cleric", "druid", "paladin", "ranger", "sorcerer", "warlock", "wizard" };

        public List<string> SpellNamesFor(CharacterClassType caster)
        {
            if (caster == CharacterClassType.Barbarian || caster == CharacterClassType.Fighter || caster == CharacterClassType.Monk || caster == CharacterClassType.Rogue) { return new List<string>(); }
            if (caster == CharacterClassType.ArcaneTrickster || caster == CharacterClassType.EldritchKnight) { return classSpellsMap[CharacterClassType.Wizard]; }
            if (caster == CharacterClassType.MulticlassCaster) { return new List<string>(); } 
            return classSpellsMap[caster];
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
                spell.Description = "No data for this spell. Look in the PHB.";
                spell.Level = -1;
                spell.Range = "?";
                spell.Components = "?";
                spell.CastableBy = new List<CharacterClassType>();
                spell.CastTime = "?";
                spell.School = "?";
                return spell;
            }
        }
    }
}
