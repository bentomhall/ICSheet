using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ICSheetCore;

namespace ICSheet5e.ViewModels
{
    class AddNewSpellViewModel : BaseViewModel
    {
        private SpellManager spellDB;
        private List<Spell> _allSpells = new List<Spell>();

        public AddNewSpellViewModel(SpellManager dB, IEnumerable<string> classNames)
        {
            spellDB = dB;
            loadSpells(classNames);
        }

        private void loadSpells(IEnumerable<string> names)
        {
            foreach (var typeName in names)
            {
                foreach (var spellName in spellDB.SpellNamesFor(typeName))
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
