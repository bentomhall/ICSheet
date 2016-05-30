using ICSheetCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ICSheet5e.ResourceModifiers;

namespace ICSheet5e.ViewModels
{
    //TODO: needs significant surgery
    public class SpellBookViewModel : BaseViewModel
    {
        public SpellBookViewModel(PlayerCharacter caster, SpellManager dB, XMLFeatureFactory classNamesSource, CustomSpellSerializer serializer)
        {
            _caster = caster;
            _dB = dB;
            _classNamesSource = classNamesSource;
            _spellcastingClasses = _caster.SpellcastingClasses;
            _serializer = serializer;
        }

        private IEnumerable<string> _spellcastingClasses;
        private XMLFeatureFactory _classNamesSource;

        public ObservableCollection<Spell> AllSpells
        {
            get
            {
                return new ObservableCollection<Spell>(_caster.KnownSpells.Where(x => x.Level == _selectedLevel));
            }
        }

        public string AvailableSpellSlots
        {
            get { return formatSpellSlots(); }
        }

        public ICommand CastSpellCommand
        {
            get { return new Views.DelegateCommand<object>(CastSpellCommandExecuted); }
        }

        public ICommand LearnNewSpellCommand
        {
            get { return new Views.DelegateCommand<string>(LearnNewSpellCommandExecuted); }
        }

        public ICommand LearnSpellCommand
        {
            get { return new Views.DelegateCommand<object>(LearnSpellCommandExecuted); }
        }

        public int SelectedLevel
        {
            get { return _selectedLevel; }
            set
            {
                _selectedLevel = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("AllSpells");
            }
        }

        public Spell SelectedSpell { 
            get { return _selectedSpell; }
            set
            {
                _selectedSpell = value;
                NotifyPropertyChanged();
                SpellLevel = _selectedLevel;
            }
        }

        public int SpellLevel { get; set; }

        public List<string> SpellLevels
        {
            get { return _spellLevelLabels; }
        }

        public ICommand ToggleSpellPreparation
        {
            get { return new Views.DelegateCommand<object>(ToggleSpellPreparationExecuted); }
        }

        public ICommand SetDomainSpell
        {
            get { return new Views.DelegateCommand<object>(SetSpellAsDomainSpellExecuted); }
        }

        public void AddNewSpellDelegate(IViewModel model)
        {
            if (model is AddNewSpellViewModel)
            {
                var vm = model as AddNewSpellViewModel;
                var spell = vm.SpellToLearn;
                _caster.Learn(spell.Name, vm.SelectedClass, vm.IsBonusSpell);
                NotifyPropertyChanged("AllSpells");
            }
        }

        private PlayerCharacter _caster;
        private SpellManager _dB;
        private int _selectedLevel = 0;
        private Spell _selectedSpell;

        private List<string> _spellLevelLabels = new List<string>()
        {
            "Cantrips",
            "Level 1",
            "Level 2",
            "Level 3",
            "Level 4",
            "Level 5",
            "Level 6",
            "Level 7",
            "Level 8",
            "Level 9"
        };
        private CustomSpellSerializer _serializer;
        private bool _isOverlayOpen;

        private void CastSpellCommandExecuted(object obj)
        {
            if (_selectedLevel == 0)
            {
                return; //casting these does nothing
            }
            _caster.UseSpellSlot(_selectedLevel);
            NotifyPropertyChanged("AvailableSpellSlots");
            
        }

        private string formatSpellSlots()
        {
            return string.Join(" / ", _caster.SpellSlots);
        }

        private void LearnNewSpellCommandExecuted(string type)
        {
            var classNames = _classNamesSource.ExtractClassNames();
            switch (type)
            {
                case "Known":
                    NewKnownSpellModel = new AddNewSpellViewModel(_dB, classNames, _spellcastingClasses, LearnNewSpell);
                    IsKnownSpellOverlayOpen = true;
                    break;
                case "Custom":
                    NewCustomSpellModel = new AddCustomSpellViewModel(classNames, OnCustomSpellCreated, _serializer);
                    IsCustomSpellOverlayOpen = true;
                    break;
            }
        }

        internal void LearnNewSpell(string name, string forClass, bool isBonus)
        {
            IsOverlayOpen = false;
            _caster.Learn(name, forClass, isBonus);
            NotifyPropertyChanged("AllSpells");
        }

        internal bool IsOverlayOpen
        {
            get { return _isOverlayOpen; }
            set
            {
                _isOverlayOpen = value;
                NotifyPropertyChanged();
            }
        }

        private bool _isKnownSpellOverlayOpen;
        private bool _isCustomSpellOverlayOpen;
        private AddNewSpellViewModel _knownSpellModel;

        public AddNewSpellViewModel NewKnownSpellModel
        {
            get { return _knownSpellModel; }
            set { _knownSpellModel = value; NotifyPropertyChanged(); }
        }
        public AddCustomSpellViewModel NewCustomSpellModel { get; set; }

        public bool IsKnownSpellOverlayOpen
        {
            get
            {
                return _isKnownSpellOverlayOpen;
            }

            set
            {
                _isKnownSpellOverlayOpen = value;
                _isOverlayOpen = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsOverlayOpen");
            }
        }

        public bool IsCustomSpellOverlayOpen
        {
            get
            {
                return _isCustomSpellOverlayOpen;
            }

            set
            {
                _isCustomSpellOverlayOpen = value;
                _isOverlayOpen = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("IsOverlayOpen");
            }
        }

        public void OnCustomSpellCreated(Spell newSpell)
        {
            IsOverlayOpen = false;
            var names = _serializer.RawSpellList();
            var details = _serializer.RawSpellDetails();
            _dB.ReloadSpellDetails(names, details);
            _caster.Learn(newSpell.Name, newSpell.InSpellbook, false);
            NotifyPropertyChanged("AllSpells");
        }

        private void LearnSpellCommandExecuted(object obj)
        {
            var spell = SelectedSpell;
            _caster.Learn(spell.Name, spell.InSpellbook, false);
            NotifyPropertyChanged("SpellKnown");
        }

        private void SetSpellAsDomainSpellExecuted(object obj)
        {
            if (SelectedSpell.IsBonusSpell) { _caster.Unprepare(SelectedSpell.Name, SelectedSpell.InSpellbook); }
            else { _caster.Prepare(SelectedSpell.Name, SelectedSpell.InSpellbook, true); }
            NotifyPropertyChanged("AllSpells");
        }

        private void ToggleSpellPreparationExecuted(object obj)
        {
            var spell = SelectedSpell;
            if (spell.IsBonusSpell) { return; } //bonus spells are always prepared.
            if (spell.IsPrepared)
            {
                _caster.Unprepare(spell.Name, spell.InSpellbook);
            }
            else
            {
                _caster.Prepare(spell.Name, spell.InSpellbook, spell.IsBonusSpell);
            }
            
            NotifyPropertyChanged("AllSpells");
            NotifyPropertyChanged("IsPrepared");
        }
    }
}