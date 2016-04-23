using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSheetCore;

namespace ICSheet5e.ViewModels
{
    public class LevelUpViewModel : BaseViewModel
    {
        public IEnumerable<string> Classes
        {
            get; private set;
        }

        private string selectedClassName = "";

        private IDictionary<string, int> currentLevels;
        private string newClass;

        private string formatLevels()
        {
            var s = new List<string>();
            foreach (KeyValuePair<string, int> entry in currentLevels)
            {
                var level = (entry.Key == newClass) ? entry.Value + 1: entry.Value;
                s.Add($"{entry.Key} {level}");
            }
            if (!currentLevels.Keys.Contains(newClass))
            {
                s.Add($"{newClass} 1");
            }
            return string.Join(" / ", s);
        }

        public string SelectedClassName
        {
            get { return selectedClassName; }
            set
            {
                newClass = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("ClassLevels");

            }
        }

        public string ClassLevels
        {
            get
            {
                return formatLevels();
            }
        }

        public string ChosenClassLevels
        {
            get { return newClass; }
        }

        public LevelUpViewModel(IDictionary<string, int> current, XMLFeatureFactory dataSource)
        {
            Classes = dataSource.ExtractClassNames();
            currentLevels = current;
        }


    }
}
