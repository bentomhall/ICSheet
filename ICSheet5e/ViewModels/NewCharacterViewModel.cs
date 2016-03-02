using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Interactive_Character_Sheet_Core;

namespace ICSheet5e.ViewModels
{
    public class NewCharacterViewModel:BaseViewModel
    {
        public string CharacterName { get; set; }
        public string Race { get; set; }
        public List<string> Classes
        {
            get { return classes; }
        }

        private List<String> classes = new List<string>()
        {
            "Barbarian",
            "Bard",
            "Cleric",
            "Druid",
            "Fighter",
            "Monk",
            "Paladin",
            "Ranger",
            "Rogue",
            "Sorcerer",
            "Warlock",
            "Wizard",
            "Eldritch Knight*",
            "Arcane Trickster*",
        };

        private Dictionary<string, Model.CharacterClassType> classMap = new Dictionary<string,Model.CharacterClassType>()
        {
            {"Barbarian", Model.CharacterClassType.Barbarian},
            {"Bard", Model.CharacterClassType.Bard},
            {"Cleric", Model.CharacterClassType.Cleric},
            {"Druid", Model.CharacterClassType.Druid},
            {"Fighter", Model.CharacterClassType.Fighter},
            {"Monk", Model.CharacterClassType.Monk},
            {"Paladin", Model.CharacterClassType.Paladin},
            {"Ranger", Model.CharacterClassType.Ranger},
            {"Rogue", Model.CharacterClassType.Rogue},
            {"Sorcerer", Model.CharacterClassType.Sorcerer},
            {"Warlock", Model.CharacterClassType.Warlock},
            {"Wizard", Model.CharacterClassType.Wizard},
            {"Eldritch Knight*", Model.CharacterClassType.EldritchKnight},
            {"Arcane Trickster*", Model.CharacterClassType.ArcaneTrickster}
        };

        private List<Tuple<Model.CharacterClassType, int>> levels =  new List<Tuple<Model.CharacterClassType,int>>();

        private string _class1 = "";
        private string _class2 = "";
        private string _class3 = "";
        private int _levels1 = 0;
        private int _levels2 = 0;
        private int _levels3 = 0;

        public string Class1
        {
            get { return _class1; }
            set
            {
                _class1 = value;
                NotifyPropertyChanged();
                UpdateNewCharacterCommandCanExecute();
            }
        }
        public string Class2
        {
            get { return _class2; }
            set
            {
                _class2 = value;
                NotifyPropertyChanged();
            }
        }
        public string Class3
        {
            get { return _class3; }
            set
            {
                _class3 = value;
                NotifyPropertyChanged();
            }
        }

        public int Levels1
        {
            get { return _levels1; }
            set
            {
                _levels1 = value;
                NotifyPropertyChanged();
                UpdateNewCharacterCommandCanExecute();
            }
        }
        public int Levels2
        {
            get { return _levels2; }
            set
            {
                _levels2 = value;
                NotifyPropertyChanged();
            }
        }
        public int Levels3
        {
            get { return _levels3; }
            set
            {
                _levels3 = value;
                NotifyPropertyChanged();
            }
        }


        public ICommand StartNewCharacterCommand
        {
            get { return new Views.DelegateCommand<object>(StartNewCharacterCommandExecuted); }
        }

        private void StartNewCharacterCommandExecuted(object sender)
        {
            var c1 = new Tuple<Model.CharacterClassType, int>(classMap[_class1], _levels1);
            levels.Add(c1);
            if (_class2 != "" && _levels2 != 0)
            { 
                var c2 = new Tuple<Model.CharacterClassType, int>(classMap[_class2], _levels2);
                levels.Add(c2);
            }
            if (_class3 != "" && _levels3 != 0)
            {
                var c3 = new Tuple<Model.CharacterClassType, int>(classMap[_class3], _levels3);
                levels.Add(c3);
            }

            if (delegateAction != null)
            {
                delegateAction(CharacterName, Race, levels);
            }

        }

        public bool CanExecute
        {
            get
            {
                return _canExecute;
            }
            set
            {
                _canExecute = value;
                NotifyPropertyChanged();
            }
        }

        private bool _canExecute = false;
        private void UpdateNewCharacterCommandCanExecute()
        {
            bool canExecute = true;
            canExecute = canExecute && (CharacterName != null);
            canExecute = canExecute && (Race != null);
            canExecute = canExecute && (_levels1 != 0);
            canExecute = canExecute && (_class1 != "");
            CanExecute = canExecute;
        }

        public Action<string, string, List<Tuple<Model.CharacterClassType, int>>> delegateAction { get; set; }
    }
}
