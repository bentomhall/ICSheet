using System.Collections.Generic;

namespace ICSheetCore
{
    public interface ISpellManager
    {
        void ReloadSpellDetails(string spellList, string spellDetails);
        Spell SpellDetailsFor(string spellName);
        IEnumerable<string> SpellNamesFor(string className);
    }
}