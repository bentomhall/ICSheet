using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheet5e.ViewModels
{
    class NoCharacterViewModel: BaseViewModel
    {
        private bool isCharacterInitialized = false;
        private bool canCastSpells = false;

        public bool IsCharacterInitialized
        {
            get { return isCharacterInitialized; }
            set
            {
                isCharacterInitialized = value;
                NotifyPropertyChanged();
            }
        }

        public bool CanCastSpells
        {
            get { return canCastSpells; }
            set
            {
                canCastSpells = value;
                NotifyPropertyChanged();
            }
        }
    }
}
