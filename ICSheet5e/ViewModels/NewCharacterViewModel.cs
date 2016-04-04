using System;
using System.Collections.Generic;
using System.Windows.Input;

namespace ICSheet5e.ViewModels
{
    public class NewCharacterViewModel : BaseViewModel
    {
        public NewCharacterViewModel() : base()
        {
            RaceList = new List<Model.Race>();
            foreach (var rtype in _allRaceTypes)
            {
                RaceList.Add(new Model.Race(rtype));
            }
        }

        public string CharacterName { get; set; }
        public Model.Race CharacterRace { get; set; }
        public List<string> Classes
        {
            get { return classes; }
        }

        static private List<string> classes = new List<string>()
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

        static private Dictionary<string, Model.CharacterClassType> classMap = new Dictionary<string,Model.CharacterClassType>()
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

        private List<Model.Race.RaceType> _allRaceTypes = new List<Model.Race.RaceType>()
        {
            Model.Race.RaceType.DarkElf,
            Model.Race.RaceType.Dragonborn,
            Model.Race.RaceType.Dwarf,
            Model.Race.RaceType.Elf,
            Model.Race.RaceType.ForestGnome,
            Model.Race.RaceType.Gnome,
            Model.Race.RaceType.HalfElf,
            Model.Race.RaceType.Halfling,
            Model.Race.RaceType.HalfOrc,
            Model.Race.RaceType.HighElf,
            Model.Race.RaceType.HillDwarf,
            Model.Race.RaceType.Human,
            Model.Race.RaceType.Lightheart,
            Model.Race.RaceType.MountainDwarf,
            Model.Race.RaceType.RockGnome,
            Model.Race.RaceType.Stout,
            Model.Race.RaceType.Tiefling,
            Model.Race.RaceType.VariantHuman,
            Model.Race.RaceType.WoodElf
        };

        public List<Model.Race> RaceList { get; set; }

        private List<Model.CharacterClassItem> levels =  new List<Model.CharacterClassItem>();

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
            var c1 = new Model.CharacterClassItem(classMap[_class1], _levels1);
            levels.Add(c1);
            if (!string.IsNullOrEmpty(_class2) && _levels2 != 0)
            { 
                var c2 = new Model.CharacterClassItem(classMap[_class2], _levels2);
                levels.Add(c2);
            }
            if (!string.IsNullOrEmpty(_class3) && _levels3 != 0)
            {
                var c3 = new Model.CharacterClassItem(classMap[_class3], _levels3);
                levels.Add(c3);
            }

            if (delegateAction != null)
            {
                delegateAction(CharacterName, CharacterRace, levels);
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
            canExecute = canExecute && (CharacterRace != null);
            canExecute = canExecute && (_levels1 != 0);
            canExecute = canExecute && !string.IsNullOrEmpty(_class1);
            CanExecute = canExecute;
        }

        public Action<string, Model.Race, IEnumerable<Model.CharacterClassItem>> delegateAction { get; set; }
    }
}
