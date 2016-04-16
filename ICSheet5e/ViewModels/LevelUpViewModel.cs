using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSheetCore;

namespace ICSheet5e.ViewModels
{
    public class LevelUpViewModel : BaseViewModel
    {
        public List<string> Classes
        {
            get { return classes; }
        }

        private List<string> classes = new List<string>()
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

        private Dictionary<string, CharacterClassType> classMap = new Dictionary<string, CharacterClassType>()
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

        private string selectedClassName = "";

        private List<CharacterClassItem> currentLevels;
        private List<CharacterClassItem> projectedLevels;
        
        private void addProjectedLevel(CharacterClassType ofType)
        {
            projectedLevels = new List<CharacterClassItem>(currentLevels); //clear any changes
            var matchingType = currentLevels.SingleOrDefault(x => x.Matches(ofType));
            if (matchingType != null)
            {
                matchingType.LevelUp();
            }
            else
            {
                projectedLevels.Add(new CharacterClassItem(ofType, 1));
            }
            NotifyPropertyChanged("ClassLevels");
        }

        private string formatLevels()
        {
            var output = new StringBuilder();
            foreach (var item in projectedLevels)
            {
                output.AppendFormat(" {0} /", item);
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

        public ICollection<CharacterClassItem> ChosenClassLevels
        {
            get { return projectedLevels; }
        }

        public LevelUpViewModel(ICollection<CharacterClassItem> current)
        {
            currentLevels = current.ToList();
            projectedLevels = new List<CharacterClassItem>(current);
        }


    }
}
