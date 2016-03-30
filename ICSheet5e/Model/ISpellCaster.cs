﻿using System;
using System.Collections.Generic;
namespace ICSheet5e.Model
{
    interface ISpellCaster
    {
        void AddSpell(Spell spell);
        void AdjustLevel(List<CharacterClassItem> newLevels);
        System.Collections.Generic.List<Spell> AllSpellsForLevel(int level);
        bool CanCastSpell(int ofLevel);
        void CastSpell(int ofLevel);
        string Description { get; }
        bool HasSpellPrepared(Spell spell);
        bool IsSpellKnown(Spell spell);
        string Name { get; }
        System.Collections.Generic.List<Spell> PreparedSpells { get; }
        bool PrepareSpell(Spell spell);
        void PrepareSpells(System.Collections.Generic.List<Spell> spells);
        void RecoverAllSpellSlots();
        void RecoverSpellSlots(int ofLevel);
        void ResetUses();
        Tuple<System.Collections.Generic.List<int>, System.Collections.Generic.List<int>> Slots { get; }
        bool TryUseFeature();
        string Uses { get; }
    }
}
