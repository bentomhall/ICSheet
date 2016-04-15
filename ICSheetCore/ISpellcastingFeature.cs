using System.Collections.Generic;

namespace ICSheetCore
{
    interface ISpellcastingFeature: IFeature
    {
        string SpellBookName { get; }
        AbilityType CastingAbility { get; }
        bool ParticipatesInMulticlassSpellcasting { get; }
        IEnumerable<int> SpellSlots { get; }
        SpellcastingLookup.CastingType CasterType { get; }

        int CantripsKnown(int level);
        int BonusSpells(int level);
        int SpellsKnown(int level);
        int SpellsPrepared(int level);

    }
}
