using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheetCore
{
    interface ISpellcastingDataSource
    {
        IEnumerable<int> SpellSlots { get; }
        IEnumerable<Spell> PreparedSpells { get; }
        bool IsSpellcaster { get; }
        IReadOnlyDictionary<string, int> SpellAttackBonuses { get; }
        IReadOnlyDictionary<string, int> SpellDCs { get; }
        IEnumerable<Spell> KnownSpells { get; }
    }

    interface ISpellcastingDelegate
    {
        void UseSpellSlot(int level);
        void Learn(string spellName, string asClass);
        void Prepare(string spellName, string asClass);
        void Unprepare(string spellName, string asClass);
        void Unlearn(string spellName, string asClass);
    }
}
