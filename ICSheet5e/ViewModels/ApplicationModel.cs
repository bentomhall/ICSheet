﻿using System;
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
                return currentCharacter.Spellcasting.Count > 0;
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

        public void NewCharacterInformationReceived(string name, Model.Race race, List<System.Tuple<Model.CharacterClassType, int>> classes)
        {
            currentCharacter = new Model.Character(name, classes, race);
            currentCharacter.ItemDB = itemDB;
            currentCharacter.SpellDB = spellDB;
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
            ViewModels[0] = new CharacterViewModel(currentCharacter, this);
            ViewModels[1] = new InventoryViewModel(currentCharacter, this);
            if (CanCastSpells) { ViewModels[3] = new SpellBookViewModel(currentCharacter.Spellcasting[0], spellDB); }
            
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
            var newLevels = vm.ChosenClassLevels;
            currentCharacter.DoLevelUp(newLevels);
            if (currentCharacter.Spellcasting.Count > 0)
            {
                NotifyPropertyChanged("CanCastSpells");
                if (ViewModels[3] is BaseViewModel && CanCastSpells) //change enabled spellcasting
                {
                    ViewModels[3] = new SpellBookViewModel(currentCharacter.Spellcasting[0], spellDB);
                    NotifyPropertyChanged("ViewModels");
                }
            }
        }


    }
}
