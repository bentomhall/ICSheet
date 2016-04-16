using System;
using System.Collections.Generic;
using System.Windows.Input;
using ICSheetCore;

namespace ICSheet5e.ViewModels
{
    public class NewCharacterViewModel : BaseViewModel
    {
        public NewCharacterViewModel() : base()
        {
            RaceList = new List<Race>();
            foreach (var rtype in _allRaceTypes)
            {
                RaceList.Add(new Race(rtype));
            }
        }

        public string CharacterName { get; set; }
        public Race CharacterRace { get; set; }
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

        static private Dictionary<string, CharacterClassType> classMap = new Dictionary<string, CharacterClassType>()
        {
            {"Barbarian", CharacterClassType.Barbarian},
            {"Bard", CharacterClassType.Bard},
            {"Cleric", CharacterClassType.Cleric},
            {"Druid", CharacterClassType.Druid},
            {"Fighter", CharacterClassType.Fighter},
            {"Monk", CharacterClassType.Monk},
            {"Paladin", CharacterClassType.Paladin},
            {"Ranger", CharacterClassType.Ranger},
            {"Rogue", CharacterClassType.Rogue},
            {"Sorcerer", CharacterClassType.Sorcerer},
            {"Warlock", CharacterClassType.Warlock},
            {"Wizard", CharacterClassType.Wizard},
            {"Eldritch Knight*", CharacterClassType.EldritchKnight},
            {"Arcane Trickster*", CharacterClassType.ArcaneTrickster}
        };

        private List<Race.RaceType> _allRaceTypes = new List<Race.RaceType>()
        {
            Race.RaceType.DarkElf,
            Race.RaceType.Dragonborn,
            Race.RaceType.Dwarf,
            Race.RaceType.Elf,
            Race.RaceType.ForestGnome,
            Race.RaceType.Gnome,
            Race.RaceType.HalfElf,
            Race.RaceType.Halfling,
            Race.RaceType.HalfOrc,
            Race.RaceType.HighElf,
            Race.RaceType.HillDwarf,
            Race.RaceType.Human,
            Race.RaceType.Lightheart,
            Race.RaceType.MountainDwarf,
            Race.RaceType.RockGnome,
            Race.RaceType.Stout,
            Race.RaceType.Tiefling,
            Race.RaceType.VariantHuman,
            Race.RaceType.WoodElf
        };

        public List<Race> RaceList { get; set; }

        private List<CharacterClassItem> levels =  new List<CharacterClassItem>();

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
            var c1 = new CharacterClassItem(classMap[_class1], _levels1);
            levels.Add(c1);
            if (!string.IsNullOrEmpty(_class2) && _levels2 != 0)
            { 
                var c2 = new CharacterClassItem(classMap[_class2], _levels2);
                levels.Add(c2);
            }
            if (!string.IsNullOrEmpty(_class3) && _levels3 != 0)
            {
                var c3 = new CharacterClassItem(classMap[_class3], _levels3);
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

        public Action<string, Race, IEnumerable<CharacterClassItem>> delegateAction { get; set; }
    }
}
