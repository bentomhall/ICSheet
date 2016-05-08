using System;
using System.Collections.Generic;
using System.Windows.Input;
using System.Runtime.Serialization;
using ICSheetCore;

namespace ICSheet5e.ViewModels
{
    public class ApplicationModel: BaseViewModel
    {
        PlayerCharacter currentCharacter = null;
        ItemDataBase itemDB;
        SpellManager spellDB;
        public bool IsEditingModeEnabled
        {
            get { return canEdit; }
            set
            {
                canEdit = value;
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
            var itemReader = new XMLItemReader(Properties.Resources.BasicArmors, Properties.Resources.BasicWeapons, Properties.Resources.BasicItems);
            itemDB = new ItemDataBase(itemReader);
            spellDB = new SpellManager(Properties.Resources.spell_list, Properties.Resources.SpellList5e);
            featureFactory = new XMLFeatureFactory(Properties.Resources.RacialFeatures, Properties.Resources.ClassFeatures);
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
        public ICommand ToggleEditingCommand
        {
            get { return new Views.DelegateCommand<object>(ToggleEditingCommandExecuted); }
        }

        public ICommand AddSubclassCommand
        {
            get { return new Views.DelegateCommand<object>(AddSubclassCommandExecuted); }
        }

        private void AddSubclassCommandExecuted(object obj)
        {
            var classes = currentCharacter.Levels.Keys;
            var vm = new AddSubclassViewModel(classes, featureFactory);
            Views.WindowManager.DisplayDialog(Views.WindowManager.DialogType.AddSubclassDialog, vm, OnAddSubclass);
        }

        private void OnAddSubclass(IViewModel obj)
        {
            var vm = obj as AddSubclassViewModel;
            if (vm != null)
            {
                currentCharacter.AddSubclass(vm.SelectedClass, vm.SelectedSubclass, vm.Features);
                NotifyPropertyChanged("Features");
            }
            DoAutosave();
            
        }

        public void NewCharacterInformationReceived(string name, string alignment, string background, Tuple<string, string> race, IDictionary<string, int> classes)
        {
            
            var characterBuilder = new CharacterFactory(name, spellDB, featureFactory);
            characterBuilder.AssignClassLevels(classes);
            characterBuilder.AssignRace(race.Item1, race.Item2);
            characterBuilder.AssignAlignment(alignment);
            characterBuilder.AssignBackground(background);
            currentCharacter = characterBuilder.ToPlayerCharacter();
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
            if (CanCastSpells) {
                var sp = new SpellBookViewModel(currentCharacter, spellDB, featureFactory);
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
            var serializer = new DataContractSerializer(typeof(ICSheetCore.Data.CharacterData));
            System.IO.FileStream reader = new System.IO.FileStream(location, System.IO.FileMode.Open);
            var cData = (ICSheetCore.Data.CharacterData)serializer.ReadObject(reader);
            reader.Close();
            var builder = new CharacterFactory(cData.Name, spellDB, featureFactory);
            currentCharacter = builder.BuildFromStoredData(cData);
            setViewModels();
        }

        
        public void SaveCommandExecuted(object sender)
        {
            if (!IsCharacterInitialized) return;
            var saveLocation = Views.WindowManager.SelectSaveLocation();
            if (saveLocation == null) { return; } //user canceled save dialog
            var serializer = new DataContractSerializer(typeof(ICSheetCore.Data.CharacterData));
            System.IO.FileStream stream = new System.IO.FileStream(saveLocation, System.IO.FileMode.Create);
            serializer.WriteObject(stream, currentCharacter.ToCharacterData());
            stream.Close();
        }

        public bool SaveCommandCanExecute(object sender)
        {
            if (sender == null) { return false; }
            return IsCharacterInitialized;
        }

        public void ToggleEditingCommandExecuted(object sender)
        {
            IsEditingModeEnabled = !canEdit;
            if (!IsEditingModeEnabled) { DoAutosave(); }//save when exiting editing
            return;
        }

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
            }
        }

        public ICommand LevelUpCommand
        {
            get { return new Views.DelegateCommand<object>(DoLevelUpCommandExecuted); }
        }

        private void DoLevelUpCommandExecuted(object obj)
        {
            if (currentCharacter == null) { return; }
            var vm = new LevelUpViewModel(currentCharacter.Levels, featureFactory);
            Views.WindowManager.DisplayDialog(Views.WindowManager.DialogType.LevelUpDialog, vm, OnLevelUpCompleted);
        }

        private void OnLevelUpCompleted(IViewModel obj)
        {
            var vm = obj as LevelUpViewModel;
            if (vm == null) { return; }
            var newLevels = vm.ChosenClassLevels;
            var newFeatures = featureFactory.ExtractFeaturesFor(newLevels);
            currentCharacter.DoLevelUp(newLevels, newFeatures);
            NotifyPropertyChanged("Levels");
            NotifyPropertyChanged("Features");
            if (currentCharacter.IsSpellcaster)
            {
                NotifyPropertyChanged("CanCastSpells");
                if (ViewModels[3] is BaseViewModel && CanCastSpells) //change enabled spellcasting
                {
                    ViewModels[3] = new SpellBookViewModel(currentCharacter, spellDB, featureFactory);
                    NotifyPropertyChanged("ViewModels");
                }
            }
            DoAutosave();
        }

        public ICommand AddFeatureCommand
        {
            get { return new Views.DelegateCommand<object>(AddFeatureCommandExecuted); }
        }

        public ICommand OpenSRDCommand
        {
            get { return new Views.DelegateCommand<object>(OpenSRDCommandExecuted); }
        }


        private void AddFeatureCommandExecuted(object obj)
        {
            if (currentCharacter == null) { return; }
            var vm = new AddFeatureViewModel();
            Views.WindowManager.DisplayDialog(Views.WindowManager.DialogType.AddNewFeatureDialog, vm, AddFeatureDelegate);

        }

        private void AddFeatureDelegate(IViewModel obj)
        {
            if (!(obj is AddFeatureViewModel)) { return; }
            var vm = (AddFeatureViewModel)obj;
            var feature = vm.ToFeature();
            currentCharacter.AddFeature(feature);
            NotifyPropertyChanged("Features");
        }

        private void OpenSRDCommandExecuted(object obj)
        {
            Views.WindowManager.OpenSRD();
        }

        private void DoAutosave()
        {
            if (!IsCharacterInitialized) return;
            var saveLocation = System.IO.Path.Combine(Environment.GetFolderPath(Environment.SpecialFolder.LocalApplicationData), "autosave.dnd5e");
            var serializer = new DataContractSerializer(typeof(ICSheetCore.Data.CharacterData));
            System.IO.FileStream stream = new System.IO.FileStream(saveLocation, System.IO.FileMode.Create);
            serializer.WriteObject(stream, currentCharacter.ToCharacterData());
            stream.Close();
        }


    }
}
