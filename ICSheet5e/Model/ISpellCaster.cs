using System;
using System.Collections.Generic;
namespace ICSheet5e.Model
{
    interface ISpellCaster
    {
        void AddSpell(Spell spell);
        void AdjustLevel(IEnumerable<CharacterClassItem> newLevels);
        IEnumerable<Spell> AllSpellsForLevel(int level);
        bool CanCastSpell(int ofLevel);
        void CastSpell(int ofLevel);
        string Description { get; }
        bool HasSpellPrepared(Spell spell);
        bool IsSpellKnown(Spell spell);
        string Name { get; }
        IEnumerable<Spell> PreparedSpells { get; }
        bool PrepareSpell(Spell spell);
        void PrepareSpells(ICollection<Spell> spells);
        void RecoverAllSpellSlots();
        void RecoverSpellSlots(int ofLevel);
        void ResetUses();
        Tuple<List<int>, List<int>> Slots { get; }
        bool TryUseFeature();
        string Uses { get; }
    }
}
