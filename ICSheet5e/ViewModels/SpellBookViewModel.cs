using ICSheetCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ICSheet5e.ViewModels
{
    //TODO: needs significant surgery
    public class SpellBookViewModel : BaseViewModel
    {
        public SpellBookViewModel(PlayerCharacter caster, SpellManager dB, XMLFeatureFactory classNamesSource)
        {
            _caster = caster;
            _dB = dB;
            _classNamesSource = classNamesSource;
            _spellcastingClasses = _caster.SpellcastingClasses;

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
            get { return new Views.DelegateCommand<object>(LearnNewSpellCommandExecuted); }
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

        private void LearnNewSpellCommandExecuted(object obj)
        {
            var type = Views.WindowManager.DialogType.AddNewSpellsDialog;
            var model = new AddNewSpellViewModel(_dB, _classNamesSource.ExtractClassNames(), _spellcastingClasses);
            Views.WindowManager.DisplayDialog(type, model, AddNewSpellDelegate);
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

        //private void LoadAllSpells()
        //{
        //    _allSpells.AddRange(_caster.KnownSpells);
        //}


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