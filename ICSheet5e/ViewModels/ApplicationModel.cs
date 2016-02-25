using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace ICSheet5e.ViewModels
{
    public class ApplicationModel
    {
        Model.Character currentCharacter = null;
        public bool IsEditingModeEnabled { get; set; }

        ObservableCollection<object> tabItemViewModels;

        public ObservableCollection<object> TabItemViewModels { get { return tabItemViewModels; } }

        public ApplicationModel()
        {

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
            Console.WriteLine("New Character Command Executed");
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
            IsEditingModeEnabled = !IsEditingModeEnabled;
            return;
        }

    }
}
