using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ICSheetCore
{
    /// <summary>
    /// A set of spells and associated information for a single spellcasting class.
    /// </summary>
    [DataContract]
    public class SpellBook
    {
        [DataMember]
        private ISpellManager dB;
        [DataMember]
        private List<Spell> _allSpells = new List<Spell>();
        [DataMember]
        private List<Spell> _knownSpells = new List<Spell>();
        [DataMember]
        private string _name;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellDB"></param>
        /// <param name="className"></param>
        /// <param name="isPreparedCaster"></param>
        public SpellBook(ISpellManager spellDB, string className, bool isPreparedCaster)
        {
            dB = spellDB;
            _name = className;
            loadSpells(className, isPreparedCaster);
        }

        /// <summary>
        /// 
        /// </summary>
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
                if (!IsSpellKnown(spell.Name)) { _knownSpells.Add(spell); }
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellLevel"></param>
        /// <returns></returns>
        public IEnumerable<Spell> AllSpellsFor(int spellLevel)
        {
            return _allSpells.Where(x => x.Level == spellLevel);
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Spell> AllPreparedSpells
        {
            get 
            {
                return _knownSpells.Where(x => x.IsPrepared); 
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<Spell> AllKnownSpells
        {
            get { return _knownSpells; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="withName"></param>
        /// <param name="isBonus"></param>
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

        /// <summary>
        /// 
        /// </summary>
        public void UnprepareAllSpells()
        {
            foreach (var spell in _knownSpells)
            {
                spell.IsPrepared = false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellName"></param>
        public void UnlearnSpell(string spellName)
        {
            var s = _knownSpells.SingleOrDefault(x => x.Name == spellName);
            if (s != null) { _knownSpells.Remove(s); }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellName"></param>
        /// <param name="asBonus"></param>
        public void PrepareSpell(string spellName, bool asBonus)
        {
            var s = _knownSpells.SingleOrDefault(x => x.Name == spellName);
            if (s != null) { s.IsPrepared = true; s.IsBonusSpell = asBonus; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellName"></param>
        public void UnprepareSpell(string spellName)
        {
            var s = _knownSpells.SingleOrDefault(x => x.Name == spellName);
            if (s != null) { s.IsPrepared = false; s.IsBonusSpell = false; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="spellName"></param>
        /// <returns></returns>
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
