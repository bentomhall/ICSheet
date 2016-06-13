using ICSheetCore;
using System;
using System.Collections.Generic;
using System.Text;

namespace ICSheetIOS.Models
{
    public class InformationModel
    {
        public InformationModel(PlayerCharacter c)
        {
            _character = c;
        }

        private PlayerCharacter _character;
    }
}
