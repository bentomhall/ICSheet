﻿using System;
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

        public AddNewSpellViewModel(SpellManager dB, IEnumerable<string> classNames, IEnumerable<string> castingClasses)
        {
            spellDB = dB;
            loadSpells(classNames);
            CastingClasses = castingClasses;
            SelectedClass = castingClasses.First();
            NotifyPropertyChanged("SelectedClass");
        }

        private void loadSpells(IEnumerable<string> names)
        {
            foreach (var typeName in names)
            {
                foreach (var spellName in spellDB.SpellNamesFor(typeName))
                {
                    if (_allSpells.SingleOrDefault(x => x.Name.Equals(spellName, StringComparison.CurrentCultureIgnoreCase)) == null)
                    {
                        _allSpells.Add(spellDB.SpellDetailsFor(spellName));
                    }
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")] //set in XAML
        public Spell SpellToLearn { get; set; }

        private string _searchString;

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public string SearchString
        {
            get { return _searchString; }
            set
            {
                if (value != _searchString)
                {
                    _searchString = value;
                    NotifyPropertyChanged();
                    NotifyPropertyChanged("MatchingSpells");
                }
            }
        }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ObservableCollection<Spell> MatchingSpells
        {
            get {
                if (SearchString == null) { return new ObservableCollection<Spell>(_allSpells); }
                return new ObservableCollection<Spell>(_allSpells.Where(x => x.Name.ToLower().Contains(SearchString.ToLower()))); 
            
            }
        }

        public bool IsBonusSpell { get; set; }

        public IEnumerable<string> CastingClasses { get; set; }
        public string SelectedClass { get; set; }

        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Performance", "CA1811:AvoidUncalledPrivateCode")]
        public ICommand FindMatchingSpellsCommand
        {
            get { return new Views.DelegateCommand<object>(x => NotifyPropertyChanged("MatchingSpells")); }
        }


    }
}
