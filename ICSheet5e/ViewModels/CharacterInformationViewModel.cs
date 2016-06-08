using ICSheetCore;
using System.Windows.Input;
using System.Collections.ObjectModel;

namespace ICSheet5e.ViewModels
{
    public class CharacterInformationViewModel : BaseViewModel
    {
        private CharacterRPInformation _characterInfo;
        private PlayerCharacter _character;

        public CharacterInformationViewModel(PlayerCharacter character) : base()
        {
            _character = character;
            _characterInfo = character.CharacterInformation;
        }

        public string Name { get { return _characterInfo.Name; } }
        public string Alignment { get { return _characterInfo.Alignment; } set { _characterInfo.Alignment = value; NotifyPropertyChanged(); } }
        public string Background { get { return _characterInfo.Background; } }
        public int BaseWeight { get { return _characterInfo.BaseWeight; } }
        public double TotalWeight { get { return _characterInfo.BaseWeight + _character.CarriedWeight; } }
        public string Weight { get { return $"{_character.CarriedWeight} / {_characterInfo.BaseWeight}"; } }
        public string Height { get { return _characterInfo.Height; } }

        public string CharacterTraits
        {
            get { return _characterInfo.CharacterTraits; }
            set { _characterInfo.CharacterTraits = value;  NotifyPropertyChanged(); }
        }

        public string Ideals
        {
            get { return _characterInfo.Ideals; }
            set { _characterInfo.Ideals = value;  NotifyPropertyChanged(); }
        }

        public string Bonds
        {
            get { return _characterInfo.Bonds; }
            set { _characterInfo.Bonds = value;  NotifyPropertyChanged(); }
        }

        public string Flaws
        {
            get { return _characterInfo.Flaws; }
            set { _characterInfo.Flaws = value;  NotifyPropertyChanged(); }
        }

        public string Deity
        {
            get { return _characterInfo.Deity; }
            set { _characterInfo.Deity = value; NotifyPropertyChanged(); }
        }


        public ObservableCollection<string> Languages { get { return new ObservableCollection<string>(_characterInfo.Languages); } }
        public ObservableCollection<string> Contacts { get { return new ObservableCollection<string>(_characterInfo.Contacts); } }
        public ObservableCollection<string> Tools { get { return new ObservableCollection<string>(_characterInfo.Tools); } }
        
        public string Notes { get { return _characterInfo.Notes; } set { _characterInfo.Notes = value; NotifyPropertyChanged(); } }

        public bool IsEncumbered
        {
            get { return (_character.CarriedWeight + 0.01 * (double)_character.Cash.Total) > 5.0 * _character.AbilityScoreFor(AbilityType.Strength); }
        }

        public ICommand AddLanguageCommand
        {
            get { return new Views.DelegateCommand<string>(AddLanguageCommandExecuted); }
        }

        private void AddLanguageCommandExecuted(string obj)
        {
            if (obj != null)
            {
                _characterInfo.AddLanguage(obj);
                NotifyPropertyChanged("Languages");
            }
        }

        public ICommand AddContactCommand
        {
            get { return new Views.DelegateCommand<string>(AddContactCommandExecuted); }
        }

        private void AddContactCommandExecuted(string obj)
        {
            if (obj != null) { _characterInfo.AddContact(obj); NotifyPropertyChanged("Contacts"); }
        }

        public ICommand AddToolCommand
        {
            get { return new Views.DelegateCommand<string>(AddToolCommandExecuted); }
        }

        private void AddToolCommandExecuted(string obj)
        {
            if (obj != null) { _characterInfo.AddTool(obj); NotifyPropertyChanged("Tools"); }
        }
    }
}
