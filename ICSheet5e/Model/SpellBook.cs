using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ICSheet5e.Model
{
    [DataContract]
    public class SpellBook
    {
        [DataMember]
        private Model.SpellManager dB;
        [DataMember]
        private List<Spell> _allSpells = new List<Spell>();
        [DataMember]
        private List<Spell> _knownSpells = new List<Spell>();
        [DataMember]
        private CharacterClassType classType;
        public SpellBook(Model.SpellManager spellDB, CharacterClassType className)
        {
            dB = spellDB;
            classType = className;
            loadSpells(className);

        }

        private void loadSpells(CharacterClassType className)
        {
            var spellNames = dB.SpellNamesFor(className);
            foreach (var name in spellNames)
            {
                var spell = dB.SpellDetailsFor(name);
                _allSpells.Add(spell);
            }
            switch(className)
            {
                case CharacterClassType.Cleric:
                case CharacterClassType.Druid:
                case CharacterClassType.Paladin:
                    _knownSpells = new List<Spell>(_allSpells); //these classes know all their spells
                    break;
                default:
                    break;
            }
        }

        public List<Spell> AllSpellsFor(int spellLevel)
        {
            return _allSpells.Where(x => x.Level == spellLevel).ToList<Spell>();
        }

        public List<Spell> AllPreparedSpells
        {
            get { return _knownSpells.Where(x => x.IsPrepared).ToList<Spell>(); }
        }


        public void AddKnownSpell(Spell spell)
        {
            if (!_knownSpells.Contains(spell)) { _knownSpells.Add(spell); }
        }

        public void ToggleSpellPreparation(Spell spell)
        {
            if (_knownSpells.Contains(spell)) { spell.IsPrepared = (!spell.IsPrepared); }
        }

        public void UnprepareAllSpells()
        {
            foreach (var spell in _knownSpells)
            {
                spell.IsPrepared = false;
            }
        }

        public bool IsSpellKnown(Spell spell)
        {
            return ((_knownSpells.SingleOrDefault(x => x.Name == spell.Name) != null) ? true : false);
        }
    }
}
