using ICSheet5e.Model;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;

namespace ICSheet5e.ViewModels
{
    public class SpellBookViewModel : BaseViewModel
    {
        public SpellBookViewModel(SpellCaster caster, SpellManager dB)
        {
            this._caster = caster;
            _dB = dB;
            LoadAllSpells();
            _spellsForSelectedLevel = spellViewModelsForLevel(0);
        }

        public ObservableCollection<SpellViewModel> AllSpells
        {
            get
            {
                return _spellsForSelectedLevel;
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

        public SpellViewModel SelectedSpell
        {
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
                _caster.AddSpell(spell);
                var vm = new SpellViewModel(spell);
                vm.SpellKnown = true;
                _allSpellModels.Add(vm);
                _allSpells.Add(spell);
                NotifyPropertyChanged("AllSpells");
            }
        }

        private List<SpellViewModel> _allSpellModels = new List<SpellViewModel>();
        private List<Spell> _allSpells = new List<Spell>();
        private SpellCaster _caster;
        private SpellManager _dB;
        private int _selectedLevel = 0;
        private SpellViewModel _selectedSpell;

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
            else if (_caster.CanCastSpell(SpellLevel))
            {
                _caster.CastSpell(SpellLevel);
                NotifyPropertyChanged("AvailableSpellSlots");
            }
            else
            {
                return;
            }
        }

        private string formatSpellSlots()
        {
            var slots = _caster.Slots.Item2;
            return String.Format("{0} / {1} / {2} / {3} / {4} / {5} / {6} / {7} / {8}", slots[0], slots[1], slots[2], slots[3], slots[4], slots[5], slots[6], slots[7], slots[8]);
        }

        private void LearnNewSpellCommandExecuted(object obj)
        {
            var type = Views.WindowManager.DialogType.AddNewSpellsDialog;
            var model = new AddNewSpellViewModel(_dB);
            Views.WindowManager.DisplayDialog(type, model, AddNewSpellDelegate);
        }

        private void LearnSpellCommandExecuted(object obj)
        {
            var spell = SelectedSpell.Spell;
            _caster.AddSpell(spell);
            SelectedSpell.SpellKnown = true;
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
            var spells = _caster.AllSpellsForLevel(level);
            foreach (var spell in spells)
            {
                var vm = new SpellViewModel(spell);
                if (_caster.IsSpellKnown(spell)) { vm.SpellKnown = true; }
                _allSpellModels.Add(vm);
                _allSpells.Add(spell);
            }
        }

        private ObservableCollection<SpellViewModel> spellViewModelsForLevel(int level)
        {
            return new ObservableCollection<SpellViewModel>(_allSpellModels.Where(x => x.Level == level));
        }

        private void ToggleSpellPreparationExecuted(object obj)
        {
            var spell = SelectedSpell.Spell;
            if (!_caster.IsSpellKnown(spell)) { return; }
            _caster.PrepareSpell(spell);
            SelectedSpell.PrepareSpell();
            NotifyPropertyChanged("PreparedSpells");
        }
    }
}