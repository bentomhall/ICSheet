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
            LoadAllSpells();
            _spellsForSelectedLevel = spellViewModelsForLevel(0);
            reconcilePreparedSpells();
        }

        private XMLFeatureFactory _classNamesSource;
        private void reconcilePreparedSpells()
        {
            var master = _caster.PreparedSpells;
            foreach (var s in _allSpells)
            {
                if (master.Count(x => x.Name == s.Name) != 0)
                {
                    s.IsPrepared = true;
                }
            }
        }

        public ObservableCollection<Spell> AllSpells
        {
            get
            {
                return new ObservableCollection<Spell>(_allSpells.Where(x => x.Level == _selectedLevel));
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
                _spellsForSelectedLevel = spellViewModelsForLevel(value);
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

        public void AddNewSpellDelegate(IViewModel model)
        {
            if (model is AddNewSpellViewModel)
            {
                var spell = (model as AddNewSpellViewModel).SpellToLearn;
                _caster.Learn(spell.Name, "Wizard"); //wrong
                var vm = new SpellViewModel(spell);
                vm.SpellKnown = true;
                _allSpellModels.Add(vm);
                _allSpells.Add(spell);
                NotifyPropertyChanged("AllSpells");
            }
        }

        private List<SpellViewModel> _allSpellModels = new List<SpellViewModel>();
        private List<Spell> _allSpells = new List<Spell>();
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

        private ObservableCollection<SpellViewModel> _spellsForSelectedLevel;

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
            var model = new AddNewSpellViewModel(_dB, _classNamesSource.ExtractClassNames());
            Views.WindowManager.DisplayDialog(type, model, AddNewSpellDelegate);
        }

        private void LearnSpellCommandExecuted(object obj)
        {
            var spell = SelectedSpell;
            _caster.Learn(spell.Name, "Wizard");
            NotifyPropertyChanged("SpellKnown");
        }

        private void LoadAllSpells()
        {
            for (int i = 0; i <= 9; i++)
            {
                LoadSpellsOfLevel(i);
            }
        }

        private void LoadSpellsOfLevel(int level)
        {
            //var spells = _caster.AllSpellsForLevel(level);
            //foreach (var spell in spells)
            //{
            //    var vm = new SpellViewModel(spell);
            //    if (_caster.IsSpellKnown(spell)) { vm.SpellKnown = true; }
            //    _allSpellModels.Add(vm);
            //    _allSpells.Add(spell);
            //}
        }

        private ObservableCollection<SpellViewModel> spellViewModelsForLevel(int level)
        {
            return new ObservableCollection<SpellViewModel>(_allSpellModels.Where(x => x.Level == level));
        }

        private void ToggleSpellPreparationExecuted(object obj)
        {
            var spell = SelectedSpell;

            _caster.Prepare(spell.Name, "Wizard");
            NotifyPropertyChanged("AllSpells");
            NotifyPropertyChanged("IsPrepared");
        }
    }
}