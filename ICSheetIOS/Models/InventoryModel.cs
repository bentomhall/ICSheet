using ICSheetCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICSheetIOS.Models
{
    public class InventoryModel
    {
        public InventoryModel(PlayerCharacter c)
        {
            _character = c;
        }

        private PlayerCharacter _character;
    }
}
