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

namespace ICSheet5e.ViewModels
{
    public class ApplicationModel: BaseViewModel
    {
        Model.Character currentCharacter = null;
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
            get { return canCastSpells; }
            set
            {
                canCastSpells = value;
                NotifyPropertyChanged();
            }
        }

        private bool hasCharacterCreationStarted = false;
        private bool canCastSpells = false;
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
            get { return new Views.DelegateCommand<object>(SaveCommandExecuted, SaveCommandCanExecute); }
        }
        public ICommand ToggleEditingCommand
        {
            get { return new Views.DelegateCommand<object>(ToggleEditingCommandExecuted); }
        }

        public void NewCharacterInformationReceived(string name, string race, List<System.Tuple<Model.CharacterClassType, int>> classes)
        {
            currentCharacter = new Model.Character(name, classes, race);
            ViewModels[0] = new CharacterViewModel(currentCharacter, this);
            NotifyPropertyChanged("ViewModels");
            HasCharacterCreationStarted = false;
            IsCharacterInitialized = true;
            IsEditingModeEnabled = true;
            CanCastSpells = false; //not implemented
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

        public void OpenCommandExecute(object sender)
        {
            Console.WriteLine("Open Character Command executed");
        }

        
        public void SaveCommandExecuted(object sender)
        {

        }

        public bool SaveCommandCanExecute(object sender)
        {
            return (currentCharacter == null);
        }

        public void ToggleEditingCommandExecuted(object sender)
        {
            IsEditingModeEnabled = !canEdit;
            return;
        }
    }
}
