using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using Interactive_Character_Sheet_Core;

namespace ICSheet5e.ViewModels
{
    public class NewCharacterViewModel
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

        private List<string> classesChosen = new List<string>() { "", "", "" };
        public List<string> ClassesChosen { get { return classesChosen; } }

        private List<int> levelsOfClasses = new List<int>() { 0, 0, 0 };
        public List<int> LevelsOfClasses { get { return levelsOfClasses; } }

        public ICommand StartNewCharacterCommand
        {
            get { return new Views.DelegateCommand<object>(StartNewCharacterCommandExecuted, StartNewCharacterCommandCanExecute); }
        }

        private void StartNewCharacterCommandExecuted(object sender)
        {
            for (int ii = 0; ii < 3; ii++)
            {
                if (levelsOfClasses[ii] != 0 && classesChosen[ii] != "") 
                {
                    var cl = new Tuple<Model.CharacterClassType, int>(classMap[classesChosen[ii]], levelsOfClasses[ii]);
                    levels.Add(cl);
                }       
            }

            if (delegateAction != null)
            {
                delegateAction(CharacterName, Race, levels);
            }

        }

        private bool StartNewCharacterCommandCanExecute(object sender)
        {
            bool canExecute = true;
            canExecute = canExecute && (CharacterName != null);
            canExecute = canExecute && (Race != null);
            canExecute = canExecute && (levelsOfClasses[0] != 0);
            canExecute = canExecute && (classesChosen[0] != "");
            return canExecute;
        }

        public Action<string, string, List<Tuple<Model.CharacterClassType, int>>> delegateAction { get; set; }
    }
}
