using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using System.Windows.Controls;
using System.ComponentModel;
using System.Runtime.Serialization;

namespace ICSheet5e.ViewModels
{
    public class ApplicationModel: BaseViewModel
    {
        Model.Character currentCharacter = null;
        Model.ItemDataBase itemDB = new Model.ItemDataBase();
        Model.SpellManager spellDB = new Model.SpellManager();
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
                return currentCharacter.IsSpellCaster;
            }
        }

        private bool hasCharacterCreationStarted = false;
        private bool canEdit = false;
        private bool isInitialized = false;
        private List<BaseViewModel> _viewModels = new List<BaseViewModel>();

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

        public void NewCharacterInformationReceived(string name, Model.Race race, List<Model.CharacterClassItem> classes)
        {
            var characterBuilder = new Model.CharacterFactory(name, race, classes, itemDB, spellDB);
            currentCharacter = characterBuilder.Build();
            //currentCharacter.ItemDB = itemDB;
            //currentCharacter.SpellDB = spellDB;
            setViewModels();
        }

        public void NewCharacterCommandExecuted(object sender)
        {
            var vm = new NewCharacterViewModel();
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
            ViewModels[1] = new InventoryViewModel(currentCharacter, this);
            if (CanCastSpells) { 
                var sp = new SpellBookViewModel(currentCharacter.Spellcasting[0], spellDB);
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
            var serializer = new DataContractSerializer(typeof(Model.Character));
            System.IO.FileStream reader = new System.IO.FileStream(location, System.IO.FileMode.Open);
            var c = (Model.Character)serializer.ReadObject(reader);
            reader.Close();
            currentCharacter = c;
            currentCharacter.ItemDB = itemDB;
            currentCharacter.SpellDB = spellDB;
            setViewModels();
        }

        
        public void SaveCommandExecuted(object sender)
        {
            if (!IsCharacterInitialized) return;
            var saveLocation = Views.WindowManager.SelectSaveLocation();
            if (saveLocation == null) { return; } //user canceled save dialog
            List<Type> knownTypes = new List<Type>() { typeof(Model.ArmorItem), typeof(Model.WeaponItem) };
            var serializer = new DataContractSerializer(typeof(Model.Character));
            System.IO.FileStream stream = new System.IO.FileStream(saveLocation, System.IO.FileMode.Create);
            serializer.WriteObject(stream, currentCharacter);
            stream.Close();
        }

        public bool SaveCommandCanExecute(object sender)
        {
            return IsCharacterInitialized;
        }

        public void ToggleEditingCommandExecuted(object sender)
        {
            IsEditingModeEnabled = !canEdit;
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
            }
        }

        public ICommand LevelUpCommand
        {
            get { return new Views.DelegateCommand<object>(DoLevelUpCommandExecuted); }
        }

        private void DoLevelUpCommandExecuted(object obj)
        {
            if (currentCharacter == null) { return; }
            var vm = new LevelUpViewModel(currentCharacter.Levels);
            Views.WindowManager.DisplayDialog(Views.WindowManager.DialogType.LevelUpDialog, vm, OnLevelUpCompleted);
        }

        private void OnLevelUpCompleted(IViewModel obj)
        {
            var vm = obj as LevelUpViewModel;
            if (vm == null) { return; }
            var oldLevels = currentCharacter.Levels;
            var newLevels = vm.ChosenClassLevels;
            var featureFactory = new Model.XMLFeatureFactory();
            List<Model.MartialFeature> newFeatures = new List<Model.MartialFeature>();
            foreach (var cls in newLevels)
            {
                if (oldLevels.SingleOrDefault(x => x.Matches(cls.ClassType)) != null)
                {
                    newFeatures.AddRange(featureFactory.ClassFeatures(cls.ClassType));
                }
            }
            currentCharacter.DoLevelUp(newLevels, newFeatures);
            if (currentCharacter.IsSpellCaster)
            {
                NotifyPropertyChanged("CanCastSpells");
                if (ViewModels[3] is BaseViewModel && CanCastSpells) //change enabled spellcasting
                {
                    ViewModels[3] = new SpellBookViewModel(currentCharacter.Spellcasting[0], spellDB);
                    NotifyPropertyChanged("ViewModels");
                }
            }
        }

        public ICommand AddFeatureCommand
        {
            get { return new Views.DelegateCommand<object>(AddFeatureCommandExecuted); }
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
        }


    }
}
