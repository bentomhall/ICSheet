using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSheetCore;
using System.Windows.Input;

namespace ICSheet5e.ViewModels
{
    public class CreateNewSubclassViewModel : BaseViewModel
    {
        private string _windowTitle;
        private string _featureName;
        private int _featureStartingLevel = 1;
        private string _featureText;
        private string _cantripsKnown;
        private string _spellsKnown;
        private string _bonusSpells;
        private string _spellbook;
        private bool _isPreparedCaster;
        private string _subclassName;

        public string WindowTitle { get { return _windowTitle; } set { _windowTitle = value;  NotifyPropertyChanged(); } }
        public string SubclassName { get { return _subclassName; } set { _subclassName = value; NotifyPropertyChanged(); setWindowTitle(true); } }
        public ObservableCollection<string> ClassNames { get; set; }
        public ObservableCollection<IFeature> Features { get; set; }
        public string SelectedClassName { get; set; }

        public string FeatureName { get { return _featureName; } set { _featureName = value; NotifyPropertyChanged(); } }
        public int FeatureStartingLevel { get { return _featureStartingLevel; } set { _featureStartingLevel = value; NotifyPropertyChanged(); } }
        public string FeatureText { get { return _featureText; } set { _featureText = value; NotifyPropertyChanged(); } }

        public bool IsSpellcastingFeature { get { return _isSpellcastingFeature; } set { _isSpellcastingFeature = value; NotifyPropertyChanged(); } }
        public IEnumerable<string> CastingTypes { get; set; }
        public string SelectedCastingType { get; set; }
        public IEnumerable<string> CastingAbilities { get; set; }
        public string SelectedCastingAbility { get; set; }
        public string CantripsKnown { get; set; }
        public string SpellsKnown { get; set; }
        public string BonusSpells { get; set; }
        public bool IsPreparedCaster { get; set; }
        public string Spellbook { get; set; }

        public ICommand AddFeatureCommand
        {
            get { return new Views.DelegateCommand<object>(x => createFeature()); }
        }

        public ICommand AddSubclassCommand
        {
            get { return new Views.DelegateCommand<object>(x => onCompletion()); }
        }

        public ICommand ClearFeatureCommand
        {
            get { return new Views.DelegateCommand<object>(x => clearFeature()); }
        }

        private void clearFeature()
        {
            FeatureName = "";
            FeatureStartingLevel = 1;
            FeatureText = "";
            IsSpellcastingFeature = false;
            setSpellcastingDefaults();
        }

        public CreateNewSubclassViewModel(ResourceModifiers.CustomPlayerClassSerializer manager, Action<bool> cacheInvalidationDelegate)
        {
            _resourceManager = manager;
            cacheDelegate = cacheInvalidationDelegate;
            ClassNames = new ObservableCollection<string>(_resourceManager.ClassNames);
            Features = new ObservableCollection<IFeature>();
            FeatureStartingLevel = 1;
            CastingTypes = new List<string>() { "Full", "Half", "Martial", "Warlock" };
            CastingAbilities = new List<string>() { "Strength", "Dexterity", "Constitution", "Intelligence", "Wisdom", "Charisma" };
            setSpellcastingDefaults();
        }

        private bool _isSpellcastingFeature;

        private void setSpellcastingDefaults()
        {
            CantripsKnown = "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0";
            SpellsKnown = "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0";
            BonusSpells = "0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0 0";
            Spellbook = "Wizard";
        }

        private void setWindowTitle(bool needsSaving)
        {
            if (string.IsNullOrWhiteSpace(SubclassName) || string.IsNullOrWhiteSpace(SelectedClassName)) { WindowTitle = ""; return; }
            if (needsSaving) { WindowTitle = $"{SelectedClassName} : {SubclassName} *"; }
            else { WindowTitle = $"{SelectedClassName} : {SubclassName}"; }
        }

        private ResourceModifiers.CustomPlayerClassSerializer _resourceManager;
        private Action<bool> cacheDelegate;
        private void createFeature()
        {
            if (_isSpellcastingFeature)
            {
                var ability = (AbilityType)Enum.Parse(typeof(AbilityType), SelectedCastingAbility);
                var cantrips = CantripsKnown.Split(' ').Select(x => int.Parse(x));
                var spells = SpellsKnown.Split(' ').Select(x => int.Parse(x));
                var bonusSpells = BonusSpells.Split(' ').Select(x => int.Parse(x));
                var f = new SpellcastingFeature(Spellbook, SelectedCastingType, bonusSpells, ability, IsPreparedCaster, cantrips, spells);
                Features.Add(f);
            }
            else
            {
                var f = new ClassFeature(FeatureName, FeatureStartingLevel, false, FeatureText);
                Features.Add(f);
            }
            setWindowTitle(true);
            clearFeature();
        }

        private ResourceModifiers.SubclassData createSubclass()
        {
            var sc = new ResourceModifiers.SubclassData();
            sc.Features = Features.ToList();
            sc.Name = SubclassName;
            return sc;
        }

        private void onCompletion()
        {
            var subclassData = createSubclass();
            _resourceManager.ConstructSubclassForExistingClass(SelectedClassName, subclassData);
            setWindowTitle(false);
            cacheDelegate(true);
        }


    }
}
