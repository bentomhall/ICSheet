using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;

namespace ICSheetCore
{
    [DataContract]
    public class SpellBook
    {
        [DataMember]
        private SpellManager dB;
        [DataMember]
        private List<Spell> _allSpells = new List<Spell>();
        [DataMember]
        private List<Spell> _knownSpells = new List<Spell>();
        [DataMember]
        private string _name;

        public SpellBook(SpellManager spellDB, string className, bool isPreparedCaster)
        {
            dB = spellDB;
            _name = className;
            loadSpells(className, isPreparedCaster);
        }

        public int PreparedSpellCount { get { return _knownSpells.Count(x => x.IsPrepared); } }

        private void loadSpells(string className, bool isPrepared)
        {
            var spellNames = dB.SpellNamesFor(className);
            foreach (var name in spellNames)
            {
                var spell = dB.SpellDetailsFor(name);
                spell.InSpellbook = className;
                _allSpells.Add(spell);
            }
            if (isPrepared) { setKnownSpells(_allSpells); }
        }

        private void setKnownSpells(List<Spell> spells)
        {
            if (_knownSpells == null) { _knownSpells = new List<Spell>(); }
            foreach (var spell in spells)
            {
                if (!IsSpellKnown(spell)) { _knownSpells.Add(spell); }
            }
        }

        public IEnumerable<Spell> AllSpellsFor(int spellLevel)
        {
            return _allSpells.Where(x => x.Level == spellLevel);
        }

        public IEnumerable<Spell> AllPreparedSpells
        {
            get 
            {
                return _knownSpells.Where(x => x.IsPrepared); 
            }
        }

        public IEnumerable<Spell> AllKnownSpells
        {
            get { return _knownSpells; }
        }


        public void AddKnownSpell(Spell spell)
        {
            if (!IsSpellKnown(spell)) { _knownSpells.Add(spell); }
        }

        public void AddKnownSpell(string withName, bool isBonus)
        {
            if (!IsSpellKnown(withName))
            {
                var spell = dB.SpellDetailsFor(withName);
                spell.InSpellbook = _name;
                spell.IsBonusSpell = isBonus;
                if (isBonus) { spell.IsPrepared = true; }
                _knownSpells.Add(spell);
            }
        }

        public void ToggleSpellPreparation(Spell spell)
        {
            
            if (IsSpellKnown(spell)) 
            {
                spell.IsPrepared = (!spell.IsPrepared);
            }
        }

        public void UnprepareAllSpells()
        {
            foreach (var spell in _knownSpells)
            {
                spell.IsPrepared = false;
            }
        }

        public void UnlearnSpell(string spellName)
        {
            var s = _knownSpells.SingleOrDefault(x => x.Name == spellName);
            if (s != null) { _knownSpells.Remove(s); }
        }

        public void PrepareSpell(string spellName)
        {
            var s = _knownSpells.SingleOrDefault(x => x.Name == spellName);
            if (s != null) { s.IsPrepared = true; }
        }

        public void UnprepareSpell(string spellName)
        {
            var s = _knownSpells.SingleOrDefault(x => x.Name == spellName);
            if (s != null) { s.IsPrepared = false; }
        }

        public bool IsSpellKnown(Spell spell)
        {
            return (_knownSpells.SingleOrDefault(x => x.Name == spell.Name) != null);
        }

        public bool IsSpellKnown(string spellName)
        {
            return _knownSpells.Count(x => x.Name == spellName) > 0;
        }

        private bool IsSpellAlreadyAddedToList(Spell spell)
        {
            return _allSpells.SingleOrDefault(x => x.Name == spell.Name) != null;
        }


    }
}
