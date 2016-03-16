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

        public Spell SpellToLearn { get; set; }

        public string SearchString { get; set; }

        public ObservableCollection<Spell> MatchingSpells
        {
            get {
                if (SearchString == null) { return new ObservableCollection<Spell>(_allSpells); }
                return new ObservableCollection<Spell>(_allSpells.Where(x => x.Name.Contains(SearchString))); 
            
            }
        }

        public ICommand FindMatchingSpellsCommand
        {
            get { return new Views.DelegateCommand<object>(x => NotifyPropertyChanged("MatchingSpells")); }
        }


    }
}
