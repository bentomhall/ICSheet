using System;
using System.Collections.Generic;
using System.Linq;
using System.Collections.ObjectModel;
using System.Windows.Input;
using ICSheetCore;

namespace ICSheet5e.ViewModels
{
    public class AddNewSpellViewModel : BaseViewModel
    {
        private SpellManager spellDB;
        private List<Spell> _allSpells = new List<Spell>();

        public AddNewSpellViewModel(SpellManager dB, IEnumerable<string> classNames, IEnumerable<string> castingClasses, Action<string, string, bool> callback)
        {
            spellDB = dB;
            loadSpells(classNames);
            CastingClasses = castingClasses;
            SelectedClass = castingClasses.First();
            _callback = callback;
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

        public ICommand LearnSpellCommand
        {
            get { return new Views.DelegateCommand<object>(x => _callback(SpellToLearn.Name, SelectedClass, IsBonusSpell)); }
        }

        private Action<string, string, bool> _callback;


    }

    public class AddCustomSpellViewModel: BaseViewModel
    {
        public string Name { get; set; }
        public string School { get; set; }
        public bool IsConcentration { get; set; }
        public IEnumerable<string> ClassNames { get; set; }
        public string SelectedClass { get; set; }
        public string ClassToRemove { get; set; }
        public List<string> ClassesWhichCanCast { get; set; }
        public int Level { get; set; }
        public string SpellRange { get; set; }
        public string SpellDuration { get; set; }
        public string Text { get; set; }
        public string SpellComponents { get; set; }
        public string SpellCastingTime { get; set; }

        public ICommand AddCastingClass
        {
            get { return new Views.DelegateCommand<object>(x => { ClassesWhichCanCast.Add(SelectedClass); NotifyPropertyChanged("ClassesWhichCanCast"); }); }
        }

        public ICommand RemoveCastingClass
        {
            get
            {
                return new Views.DelegateCommand<object>(x =>
                {
                    ClassesWhichCanCast.Remove(ClassToRemove);
                    NotifyPropertyChanged("ClassesWhichCanCast");
                });
            }
        }

        public AddCustomSpellViewModel(IEnumerable<string> classes, Action<Spell> callback, ResourceModifiers.CustomSpellSerializer serializer)
        {
            ClassNames = classes;
            _callback = callback;
            _serializer = serializer;
        }

        private void _createCustomSpell(object obj)
        {
            var details = new ResourceModifiers.SpellSerializationData()
            {
                name = Name,
                text = Text,
                Range = SpellRange,
                level = Level.ToString(),
                school = School,
                Duration = $"{(IsConcentration ? "Concentration": "")}, {SpellDuration}",
                Components = SpellComponents,
                CastingTime = SpellCastingTime
            };
            _serializer.Add(details, ClassesWhichCanCast);
            _serializer.Save();
            _callback(_spellFromData(details));
        }

        private Spell _spellFromData(ResourceModifiers.SpellSerializationData data)
        {
            var sp = new Spell();
            sp.CastTime = data.CastingTime;
            sp.Components = data.Components;
            sp.Duration = data.Duration;
            sp.Description = data.text;
            sp.InSpellbook = ClassesWhichCanCast[0];
            sp.IsBonusSpell = false;
            sp.IsPrepared = false;
            sp.Level = Level;
            sp.Name = Name;
            sp.Range = SpellRange;
            sp.School = School;
            return sp;
        }

        private ResourceModifiers.CustomSpellSerializer _serializer;
        private Action<Spell> _callback;
    }
}
