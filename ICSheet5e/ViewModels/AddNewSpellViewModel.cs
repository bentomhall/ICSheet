using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSheet5e.Model;
using System.Collections.ObjectModel;
using System.Windows.Input;

namespace ICSheet5e.ViewModels
{
    class AddNewSpellViewModel : BaseViewModel
    {
        private SpellManager spellDB;
        private List<Spell> _allSpells = new List<Spell>();

        public AddNewSpellViewModel(SpellManager dB)
        {
            spellDB = dB;
            loadSpells();
        }

        private void loadSpells()
        {
            foreach (var typeName in Enum.GetNames(typeof(CharacterClassType)))
            {
                var type = (CharacterClassType)Enum.Parse(typeof(CharacterClassType), typeName);
                foreach (var spellName in spellDB.SpellNamesFor(type))
                {
                    if (_allSpells.SingleOrDefault(x => x.Name == spellName) == null)
                    {
                        _allSpells.Add(spellDB.SpellDetailsFor(spellName));
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")] //set in XAML
        public Spell SpellToLearn { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public string SearchString { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ObservableCollection<Spell> MatchingSpells
        {
            get {
                if (SearchString == null) { return new ObservableCollection<Spell>(_allSpells); }
                return new ObservableCollection<Spell>(_allSpells.Where(x => x.Name.Contains(SearchString))); 
            
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ICommand FindMatchingSpellsCommand
        {
            get { return new Views.DelegateCommand<object>(x => NotifyPropertyChanged("MatchingSpells")); }
        }


    }
}
