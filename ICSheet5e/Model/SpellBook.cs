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
        public SpellBook(Model.SpellManager spellDB, CharacterClassType className, List<CharacterClassType> subClasses = null)
        {
            dB = spellDB;
            if (subClasses != null)
            {
                loadSpells(subClasses);
            }
            else { loadSpells(className); }
            classType = className;
        }

        private void loadSpells(List<CharacterClassType> types)
        {
            foreach (var type in types)
            {
                loadSpells(type);
            }
        }

        private void loadSpells(CharacterClassType className)
        {
            var spellNames = dB.SpellNamesFor(className);
            foreach (var name in spellNames)
            {
                var spell = dB.SpellDetailsFor(name);
                if (!IsSpellAlreadyAddedToList(spell)) { _allSpells.Add(spell); };
            }
            switch(className)
            {
                case CharacterClassType.Cleric:
                case CharacterClassType.Druid:
                case CharacterClassType.Paladin:
                    setKnownSpells(_allSpells); //these classes know all their spells
                    break;
                default:
                    break;
            }
        }

        private void setKnownSpells(List<Spell> spells)
        {
            if (_knownSpells == null) { _knownSpells = new List<Spell>(); }
            foreach (var spell in spells)
            {
                if (!IsSpellKnown(spell)) { _knownSpells.Add(spell); }
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

        public List<Spell> AllKnownSpells
        {
            get { return _knownSpells; }
        }


        public void AddKnownSpell(Spell spell)
        {
            if (!IsSpellKnown(spell)) { _knownSpells.Add(spell); }
        }

        public void ToggleSpellPreparation(Spell spell)
        {
            if (IsSpellKnown(spell)) { spell.IsPrepared = (!spell.IsPrepared); }
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
            return (_knownSpells.SingleOrDefault(x => x.Name == spell.Name) != null);
        }

        private bool IsSpellAlreadyAddedToList(Spell spell)
        {
            return _allSpells.SingleOrDefault(x => x.Name == spell.Name) != null;
        }
    }
}
