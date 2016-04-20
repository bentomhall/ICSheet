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
        private bool canCast(CharacterClassType classType, string spellName)
        {
            return classSpellsMap[classType].Contains(spellName);
        }
        private bool hasSpellDetails(string spellName)
        {
            return _spellDetails.Any(x => x.Name == spellName);
        }
        [DataMember]
        private Dictionary<CharacterClassType, List<string>> classSpellsMap;

        [DataMember]
        private Dictionary<string, List<string>> classNameSpellsMap;

        private void loadSpellNames(string spellNames) 
        {
            _spellNames = new List<string>();
            var doc = XDocument.Parse(spellNames);
            classSpellsMap = new Dictionary<CharacterClassType, List<string>>();
            classNameSpellsMap = new Dictionary<string, List<string>>();
            foreach (var cls in doc.Descendants("CastingClass"))
            {
                var clsName = cls.Attribute("Name").Value;
                var clsType = classNameClassTypeMap[clsName];
                classSpellsMap[clsType] = new List<string>();
                classNameSpellsMap[clsName] = new List<string>();
                foreach (var spell in cls.Elements())
                {
                    var name = spell.Attribute("Name").Value;
                    if (!_spellNames.Contains(name)) { _spellNames.Add(name); }
                    classSpellsMap[clsType].Add(name);
                    classNameSpellsMap[clsName].Add(name);
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
                foreach (var caster in castingClasses)
                {
                    var type = classNameClassTypeMap[caster];
                    if (canCast(type, spell.Name))
                    {
                        spell.AddCastingClass(type);
                    }
                }
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
        [DataMember]
        private List<string> castingClasses = new List<String>() { "bard", "cleric", "druid", "paladin", "ranger", "sorcerer", "warlock", "wizard" };

        public IEnumerable<string> SpellNamesFor(CharacterClassType caster)
        {
            if (caster == CharacterClassType.Barbarian || caster == CharacterClassType.Fighter || caster == CharacterClassType.Monk || caster == CharacterClassType.Rogue) { return new List<string>(); }
            if (caster == CharacterClassType.ArcaneTrickster || caster == CharacterClassType.EldritchKnight) { return classSpellsMap[CharacterClassType.Wizard]; }
            if (caster == CharacterClassType.MulticlassCaster) { return new List<string>(); } 
            return classSpellsMap[caster];
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
