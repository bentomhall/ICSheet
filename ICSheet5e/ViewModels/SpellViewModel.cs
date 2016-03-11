using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheet5e.ViewModels
{
    public class SpellViewModel : BaseViewModel
    {
        private bool isSpellKnown = false;
        private Model.Spell _spell;

        public bool SpellKnown
        {
            get { return isSpellKnown; }
            set
            {
                isSpellKnown = value;
                NotifyPropertyChanged();
            }
        }

        public SpellViewModel(Model.Spell spell)
        {
            _spell = spell;
        }

        public Model.Spell Spell { get { return _spell; } }

        public string Name { get { return _spell.Name; } }
        public bool IsPrepared { get { return _spell.IsPrepared; } }
        public int Level { get { return _spell.Level; } }

        
    }
}
