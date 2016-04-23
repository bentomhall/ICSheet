using System;
using System.Collections.Generic;
using System.Windows.Input;
using ICSheetCore;

namespace ICSheet5e.ViewModels
{
    public class NewCharacterViewModel : BaseViewModel
    {
        private XMLFeatureFactory _nameSource;
        public NewCharacterViewModel(XMLFeatureFactory nameSource) : base()
        {
            _nameSource = nameSource;
        }

        public string CharacterName { get; set; }
        public string CharacterRace { get; set; }
        public string CharacterSubrace { get; set; }
        public string CharacterAlignment { get; set; }
        public string CharacterBackground { get; set; }
        public IEnumerable<string> Classes
        {
            get { return _nameSource.ExtractClassNames(); }
        }



        public IEnumerable<string> RaceList { get { return _nameSource.ExtractRaceNames(); } }
        public IEnumerable<string> SubraceList
        {
            get
            {
                if (CharacterRace == null) { return new List<string>(); }
                return _nameSource.ExtractSubraceNames(CharacterRace);
            }
        }

        private string _characterClass = "";

        private int _level = 0;

        public string CharacterClass
        {
            get { return _characterClass; }
            set
            {
                _characterClass = value;
                NotifyPropertyChanged();
            }
        }

        public int Level
        {
            get { return _level; }
            set
            {
                _level = value;
                NotifyPropertyChanged();
            }
        }


        public ICommand StartNewCharacterCommand
        {
            get { return new Views.DelegateCommand<object>(StartNewCharacterCommandExecuted, NewCharacterCommandCanExecute); }
        }

        private void StartNewCharacterCommandExecuted(object sender)
        {
            var raceData = new Tuple<string, string>(CharacterRace, CharacterSubrace);
            var classesAndLevels = new Dictionary<string, int>() { { CharacterClass, Level } };
            delegateAction?.Invoke(CharacterName, raceData, classesAndLevels);
        }

        private bool _canExecute = false;
        private bool NewCharacterCommandCanExecute(object sender)
        {
            if (sender == null) { return false; }
            bool canExecute = true;
            canExecute = canExecute && (CharacterName != null);
            canExecute = canExecute && (CharacterRace != null);
            canExecute = canExecute && (_level != 0);
            canExecute = canExecute && !string.IsNullOrEmpty(_characterClass);
            return canExecute;
        }

        public Action<string, Tuple<string, string>, IDictionary<string, int>> delegateAction { get; set; }
    }
}
