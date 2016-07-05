using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Runtime.Serialization;
using ICSheetCore;
using System.Linq;
using System.IO;

namespace ICSheet5e.ViewModels
{
    public class ApplicationModel : BaseViewModel
    {
        PlayerCharacter currentCharacter = null;
        ItemDataBase itemDB;
        SpellManager spellDB;
        private string itemData;
        private string armorData;
        private string weaponData;
        private string raceData;
        private string classData;
        private string currentCharacterPath;
        private string _windowTitle = "Interactive Character Sheet v1.0";

        public string WindowTitle
        {
            get { return _windowTitle; }
            set
            {
                _windowTitle = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsEditingModeEnabled
        {
            get { return canEdit; }
            set
            {
                canEdit = value;
                if (!canEdit) { DoAutosave(); SetWindowTitle(true); }
                NotifyPropertyChanged();
            }
        }
        public bool IsCharacterInitialized
        {
            get { return isInitialized; }
            set
            {
                isInitialized = value;
                NotifyPropertyChanged();
                if (isInitialized) (SaveCharacterCommand as Views.DelegateCommand<object>).RaiseCanExecuteChanged();
            }
        }

        public List<BaseViewModel> ViewModels
        {
            get
            {
                return _viewModels;
            }
            set
            {
                _viewModels = value;
                NotifyPropertyChanged();
            }
        }
        public bool HasCharacterCreationStarted
        {
            get { return hasCharacterCreationStarted; }
            set
            {
                hasCharacterCreationStarted = value;
                NotifyPropertyChanged();
            }
        }

        public bool CanCastSpells
        {
            get
            {
                if (currentCharacter == null) return false;
                return currentCharacter.IsSpellcaster;
            }
        }

        private bool hasCharacterCreationStarted = false;
        private bool canEdit = false;
        private bool isInitialized = false;
        private List<BaseViewModel> _viewModels = new List<BaseViewModel>();
        private XMLFeatureFactory featureFactory;
        private string spellBookData;
        private string spellListData;
        private ResourceModifiers.CustomSpellSerializer _serializer;
        private ResourceModifiers.ResourceFileManager _fileManager = new ResourceModifiers.ResourceFileManager();

        private void SetWindowTitle(bool needsSaving)
        {
            if (!isInitialized) { WindowTitle = "Interactive Character Sheet 5e"; return; }
            if (needsSaving)
            {
                WindowTitle = currentCharacterPath + " *";
            }
            else
            {
                WindowTitle = currentCharacterPath;
            }
        }

        private void loadItemResources()
        {
            armorData = File.ReadAllText(_fileManager.ArmorListPath);
            weaponData = File.ReadAllText(_fileManager.WeaponListPath);
            itemData = File.ReadAllText(_fileManager.ItemListPath);
            return;
        }

        private void loadSpellResources()
        {
            _serializer = new ResourceModifiers.CustomSpellSerializer(_fileManager.SpelllistPath, _fileManager.SpellDetailsPath);
            _serializer.Save(); //merges changes to built-in list upward (if any exist)
            spellBookData = File.ReadAllText(_fileManager.SpelllistPath);
            spellListData = File.ReadAllText(_fileManager.SpellDetailsPath);
            
            return;
        }

        private void loadFeatureResources()
        {
            raceData = File.ReadAllText(_fileManager.RacialFeaturesPath);
            classData = File.ReadAllText(_fileManager.ClassFeaturesPath);
            return;
        }

        public ApplicationModel()
        {

            if (currentCharacter == null)
            {
                IsCharacterInitialized = false;
            }
            var noCharacterModel = new NoCharacterViewModel();
            noCharacterModel.Parent = this;
            _viewModels.Add(noCharacterModel);
            for (int ii = 1; ii < 4; ii++)
            {
                var subModel = new BaseViewModel();
                subModel.Parent = this;
                _viewModels.Add(subModel);
            }
            FeatureModel = new AddFeatureViewModel(AddFeatureCallback);
            FeatureModel.Parent = this;
            SubclassModel = new AddSubclassViewModel(new List<string>(), featureFactory, OnAddSubclass);
            SetWindowTitle(false);
            InvalidateResourceCache(false);
        }

        public void InvalidateResourceCache(bool classesOnly)
        {
            if (!classesOnly)
            {

                loadItemResources();
                var itemReader = new XMLItemReader(armorData, weaponData, itemData);
                itemDB = new ItemDataBase(itemReader);
                loadSpellResources();
                spellDB = new SpellManager(spellBookData, spellListData);
            }
            loadFeatureResources();
            featureFactory = new XMLFeatureFactory(raceData, classData);
        }

        public ICommand NewCharacterCommand
        {
            get { return new Views.DelegateCommand<object>(NewCharacterCommandExecuted); }
        }

        public ICommand OpenCharacterCommand
        {
            get { return new Views.DelegateCommand<object>(OpenCommandExecute); }
        }
        public ICommand SaveCharacterCommand
        {
            get { return new Views.DelegateCommand<object>(SaveCommandExecuted); }
        }
        //public ICommand ToggleEditingCommand
        //{
        //    get { return new Views.DelegateCommand<object>(ToggleEditingCommandExecuted); }
        //}

        public ICommand CreateNewSubclassCommand
        {
            get { return new Views.DelegateCommand<object>(OpenSubclassCreationWindowCommand); }
        }

        private void OpenSubclassCreationWindowCommand(object obj)
        {
            var manager = new ResourceModifiers.CustomPlayerClassSerializer(classData);
            var vm = new CreateNewSubclassViewModel(manager, InvalidateResourceCache);
            Views.WindowManager.OpenSubclassCreationWindow(vm);
        }

        public ICommand AddSubclassCommand
        {
            get { return new Views.DelegateCommand<object>(AddSubclassCommandExecuted); }
        }

        private void AddSubclassCommandExecuted(object obj)
        {
            var classes = currentCharacter.Levels.Keys;
            SubclassModel = new AddSubclassViewModel(classes, featureFactory, OnAddSubclass);
            SubclassModel.Parent = this;
            SubclassModel.IsOpen = true;
            IsOpen = true;
        }

        public void OnAddSubclass(string selectedClass, string selectedSubclass, IEnumerable<IFeature> features)
        {
            currentCharacter.AddSubclass(selectedClass, selectedSubclass, features);
            if (features.Count(x => x.Name == "Spellcasting") > 0) { setViewModels(); }
            NotifyPropertyChanged("Features");
            DoAutosave();
            IsOpen = false;
        }

        public AddSubclassViewModel SubclassModel
        {
            get { return _subclassModel; }
            set { _subclassModel = value; NotifyPropertyChanged(); }
        }

        private void OnAddSubclass(IViewModel obj)
        {
            var vm = obj as AddSubclassViewModel;
            if (vm != null)
            {
                currentCharacter.AddSubclass(vm.SelectedClass, vm.SelectedSubclass, vm.Features);
                if (vm.Features.Count(x => x.Name == "Spellcasting") > 0) { setViewModels(); }
                NotifyPropertyChanged("Features");
            }
            
            DoAutosave();
            
            
        }

        public void NewCharacterInformationReceived(string name, CharacterRPInformation info, Tuple<string, string> race, IDictionary<string, int> classes)
        {
            
            var characterBuilder = new CharacterFactory(name, spellDB, featureFactory);
            characterBuilder.AssignClassLevels(classes);
            characterBuilder.AssignRace(race.Item1, race.Item2);
            currentCharacter = characterBuilder.ToPlayerCharacter(info); //create a new character
            setViewModels();
        }

        public void NewCharacterCommandExecuted(object sender)
        {
            var vm = new NewCharacterViewModel(featureFactory);
            vm.delegateAction = NewCharacterInformationReceived;
            vm.Parent = this;
            ViewModels[0] = vm;
            NotifyPropertyChanged("ViewModels");
            HasCharacterCreationStarted = true;
        }

        private void setViewModels()
        {
            var cvm = new CharacterViewModel(currentCharacter, this);
            ViewModels[0] = cvm;
            ViewModels[1] = new InventoryViewModel(currentCharacter, this, itemDB);
            ViewModels[2] = new CharacterInformationViewModel(currentCharacter);
            if (CanCastSpells) {
                var sp = new SpellBookViewModel(currentCharacter, spellDB, featureFactory, _serializer);
                sp.PropertyChanged += cvm.OnEquipmentChanged;
                ViewModels[3] = sp;
            }
            
            if (ViewModels[0] as CharacterViewModel != null)
            {
                ViewModels[1].PropertyChanged += (ViewModels[0] as CharacterViewModel).OnEquipmentChanged;
            }
            NotifyPropertyChanged("ViewModels");
            NotifyPropertyChanged("CanCastSpells");
            HasCharacterCreationStarted = false;
            IsCharacterInitialized = true;
            canEdit = true;
        }

        public void OpenCommandExecute(object sender)
        {
            var location = Views.WindowManager.SelectExistingFile();
            if (location == null) { return; } //user canceled open dialog
            currentCharacterPath = location;
            var serializer = new DataContractSerializer(typeof(ICSheetCore.Data.CharacterData));
            FileStream reader = new FileStream(location, System.IO.FileMode.Open);
            var cData = (ICSheetCore.Data.CharacterData)serializer.ReadObject(reader);
            reader.Close();
            var builder = new CharacterFactory(cData.Name, spellDB, featureFactory);
            currentCharacter = builder.BuildFromStoredData(cData);
            setViewModels();
            SetWindowTitle(false);
        }

        
        public void SaveCommandExecuted(object sender)
        {
            if (!IsCharacterInitialized) return;
            string saveLocation;
            if (currentCharacterPath == null || (string)sender == "Save As")
            {
                saveLocation = Views.WindowManager.SelectSaveLocation();
                if (saveLocation == null) { return; } //user canceled save dialog
                currentCharacterPath = saveLocation; //update cached path
            }
            else { saveLocation = currentCharacterPath; }
            var serializer = new DataContractSerializer(typeof(ICSheetCore.Data.CharacterData));
            FileStream stream = new FileStream(saveLocation, System.IO.FileMode.Create);
            serializer.WriteObject(stream, currentCharacter.ToCharacterData());
            stream.Close();
            SetWindowTitle(false);
        }

        public bool SaveCommandCanExecute(object sender)
        {
            if (sender == null) { return false; }
            return IsCharacterInitialized;
        }

        //public void ToggleEditingCommandExecuted(object sender)
        //{
        //    IsEditingModeEnabled = !canEdit;
        //    if (!IsEditingModeEnabled)
        //    {
        //        DoAutosave();
        //        SetWindowTitle(true);
        //    }//save when exiting editing
        //    return;
        //}

        public ICommand DoLongRestCommand
        {
            get { return new Views.DelegateCommand<object>(LongRestCommandExecuted); }
        }

        private void LongRestCommandExecuted(object obj)
        {
            if (currentCharacter != null)
            {
                currentCharacter.TakeLongRest();
                NotifyPropertyChanged("AvailableSpellSlots");
                NotifyPropertyChanged("CurrentHealth");
                SetWindowTitle(true);
            }
        }

        private bool _levelUpOverlayOpen;

        public ICommand LevelUpCommand
        {
            get { return new Views.DelegateCommand<object>(DoLevelUpCommandExecuted); }
        }

        private LevelUpViewModel _levelUpViewModel;
        private void DoLevelUpCommandExecuted(object obj)
        {
            if (currentCharacter == null) { return; }
            LevelUpOverlayOpen = true;
            LevelUpViewModel = new LevelUpViewModel(currentCharacter.Levels, featureFactory, OnLevelUpCompleted);
        }

        //private bool _isOverlayOpen;
        private bool _isSettingsOverlayOpen;
        private UserPreferencesViewModel _userPreferencesModel;

        private void OnLevelUpCompleted(IViewModel obj)
        {
            var vm = obj as LevelUpViewModel;
            if (vm == null) { return; }
            var newLevels = vm.ChosenClassLevels;
            var newFeatures = featureFactory.ExtractFeaturesFor(newLevels);
            currentCharacter.DoLevelUp(newLevels, newFeatures);
            LevelUpOverlayOpen = false;
            NotifyPropertyChanged("Levels");
            NotifyPropertyChanged("Features");
            if (currentCharacter.IsSpellcaster)
            {
                NotifyPropertyChanged("CanCastSpells");
                if (ViewModels[3] is BaseViewModel && CanCastSpells) //change enabled spellcasting
                {
                    ViewModels[3] = new SpellBookViewModel(currentCharacter, spellDB, featureFactory, _serializer);
                    NotifyPropertyChanged("ViewModels");
                }
            }
            DoAutosave();
            SetWindowTitle(true);
        }

        public ICommand AddFeatureCommand
        {
            get { return new Views.DelegateCommand<object>(AddFeatureCommandExecuted); }
        }

        public ICommand OpenSRDCommand
        {
            get { return new Views.DelegateCommand<object>(OpenSRDCommandExecuted); }
        }

        public bool LevelUpOverlayOpen
        {
            get
            {
                return _levelUpOverlayOpen;
            }

            set
            {
                _levelUpOverlayOpen = value;
                IsOpen = value;
                NotifyPropertyChanged();
            }
        }

        public LevelUpViewModel LevelUpViewModel
        {
            get
            {
                return _levelUpViewModel;
            }

            set
            {
                _levelUpViewModel = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsSettingsOverlayOpen
        {
            get { return _isSettingsOverlayOpen; }
            set
            {
                if (value != _isSettingsOverlayOpen)
                {
                    _isSettingsOverlayOpen = value;
                    IsOpen = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public void SettingsChangeAction()
        {
            IsSettingsOverlayOpen = false;
            NotifyPropertyChanged("Movement");
            NotifyPropertyChanged("Weight");
        }

        public ICommand OpenSettingsOverlayCommand
        {
            get { return new Views.DelegateCommand<object>(OpenSettingsCommandExecuted); }
        }

        public UserPreferencesViewModel UserPreferencesModel
        {
            get { return _userPreferencesModel; }
            set
            {
                if (value != _userPreferencesModel)
                {
                    _userPreferencesModel = value;
                    NotifyPropertyChanged();
                }
            }
        }

        private void OpenSettingsCommandExecuted(object obj)
        {
            UserPreferencesModel = new UserPreferencesViewModel(SettingsChangeAction);
            IsSettingsOverlayOpen = true;
        }

        public void AddFeatureCallback(IFeature feature)
        {
            currentCharacter.AddFeature(feature);
            NotifyPropertyChanged("Features");
            IsOpen = false;
        }

        private AddFeatureViewModel featureModel;
        private AddSubclassViewModel _subclassModel;

        public AddFeatureViewModel FeatureModel
        {
            get { return featureModel; }
            set { featureModel = value; NotifyPropertyChanged(); }
        }

        private void AddFeatureCommandExecuted(object obj)
        {
            if (currentCharacter == null) { return; }
            FeatureModel.IsOpen = true;
            IsOpen = true;
        }

        private void OpenSRDCommandExecuted(object obj)
        {
            Views.WindowManager.OpenSRD();
        }

        private void DoAutosave()
        {
            if (!IsCharacterInitialized) return;
            var saveLocation = _fileManager.CreatePathForResource("autosave.dnd5e");
            var serializer = new DataContractSerializer(typeof(ICSheetCore.Data.CharacterData));
            FileStream stream = new FileStream(saveLocation, FileMode.Create);
            serializer.WriteObject(stream, currentCharacter.ToCharacterData());
            stream.Close();
        }

    }
}
