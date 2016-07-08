using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSheetCore;

namespace ICCoreTest
{
    class SpellManagerStub : ISpellManager
    {
        public void ReloadSpellDetails(string spellList, string spellDetails)
        {
        }

        public Spell SpellDetailsFor(string spellName)
        {
            return new Spell();
        }

        public IEnumerable<string> SpellNamesFor(string className)
        {
            return new List<string>();
        }
    }
}
