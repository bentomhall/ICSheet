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
    public class ApplicationModel: INotifyPropertyChanged
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
        public Control CharacterSheetView
        {
            get { return characterSheetView; }
            set
            {
                characterSheetView = value;
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

        private bool canCastSpells = false;
        private Control characterSheetView;
        private CharacterViewModel characterVM;
        private bool canEdit = false;
        private bool isInitialized = false;
        

        public ApplicationModel()
        {
            if (currentCharacter == null)
            {
                IsCharacterInitialized = false;
                var view = new Label();
                view.Content = "Open or load a character to begin";
                CharacterSheetView = view;
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

        public void NewCharacterCommandExecuted(object sender)
        {
            currentCharacter = new Model.Character(); //not complete...need to open dialog to get name, race, and classes/levels.
            characterVM = new CharacterViewModel(currentCharacter);
            var view = new Views.MainSheetControl();
            view.DataContext = characterVM;
            CharacterSheetView = view;
            IsCharacterInitialized = true;
            IsEditingModeEnabled = true;
            CanCastSpells = false;
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


        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
