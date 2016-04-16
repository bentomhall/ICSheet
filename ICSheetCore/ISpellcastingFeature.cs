using System.Collections.Generic;

namespace ICSheetCore
{
    interface ISpellcastingFeature: IFeature
    {
        string SpellBookName { get; }
        AbilityType CastingAbility { get; }
        bool ParticipatesInMulticlassSpellcasting { get; }
        
        SpellcastingLookup.CastingType CasterType { get; }

        IEnumerable<int> SpellSlots(int level);
        int CantripsKnown(int level);
        int BonusSpells(int level);
        int SpellsKnown(int level);
        int SpellsPrepared(int level, int abilityBonus);

    }
}
