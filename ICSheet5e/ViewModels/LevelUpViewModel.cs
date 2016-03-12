using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ICSheet5e.ViewModels
{
    public class LevelUpViewModel : BaseViewModel
    {
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

        private Dictionary<string, Model.CharacterClassType> classMap = new Dictionary<string, Model.CharacterClassType>()
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

        private string selectedClassName = "Barbarian";

        private List<Tuple<Model.CharacterClassType, int>> currentLevels;
        private List<Tuple<Model.CharacterClassType, int>> projectedLevels;
        
        private void addProjectedLevel(Model.CharacterClassType ofType)
        {
            projectedLevels = new List<Tuple<Model.CharacterClassType,int>>(currentLevels); //clear any changes
            var matchingType = currentLevels.SingleOrDefault(x => x.Item1 == ofType);
            if (matchingType != null)
            {
                projectedLevels.Remove(matchingType);
                projectedLevels.Add(new Tuple<Model.CharacterClassType, int>(ofType, matchingType.Item2 + 1));
            }
            else
            {
                projectedLevels.Add(new Tuple<Model.CharacterClassType, int>(ofType, 1));
            }
            NotifyPropertyChanged("ClassLevels");
        }

        private string formatLevels()
        {
            var output = new StringBuilder();
            foreach (var item in projectedLevels)
            {
                output.AppendFormat(" {0} {1} /", item.Item1, item.Item2);
            }
            var lvls = output.ToString();
            if (lvls.EndsWith("/"))
            {
                lvls = lvls.TrimEnd('/');
            }
            return lvls;
        }

        public string SelectedClassName
        {
            get { return selectedClassName; }
            set
            {
                selectedClassName = value;
                addProjectedLevel(classMap[value]);
                NotifyPropertyChanged();

            }
        }

        public string ClassLevels
        {
            get
            {
                return formatLevels();
            }
        }

        public List<Tuple<Model.CharacterClassType, int>> ChosenClassLevels
        {
            get { return projectedLevels; }
        }

        public LevelUpViewModel(List<Tuple<Model.CharacterClassType, int>> current)
        {
            currentLevels = current;
            projectedLevels = new List<Tuple<Model.CharacterClassType,int>>(current);
        }


    }
}
