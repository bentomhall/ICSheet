using System;
using System.Collections.Generic;

namespace ICSheetCore
{
    internal class PlayerClassAggregate
    {
        private List<CharacterClassItem> _playerClasses;
        private int _proficiencyBonus;
        private SpellCastingAggregate _spellcastingAggregate;

        internal PlayerClassAggregate()
        {

        }
    }

    internal class ClassInformationChangedEventArgs : EventArgs
    {

    }
}