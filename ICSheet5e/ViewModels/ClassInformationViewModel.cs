using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InteractiveCharacterSheetCore;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace ICSheet5e.ViewModels
{
    class ClassInformationViewModel
    {
        private string name = "";
        public string Name
        {
            get { return name; }
            set
            {
                name = value;
                NotifyPropertyChanged();
            }
        }
        private Dictionary<Model.CharacterClassType, int> levels = new Dictionary<Model.CharacterClassType, int>();
        public string Levels { get { return FormatClasses(); } }
        private string race = "";
        public string Race
        {
            get { return race; }
            set
            {
                race = value;
                NotifyPropertyChanged();
            }
        }
        private int experience = 0;
        public int Experience
        {
            get { return experience; }
            set
            {
                experience = value;
                NotifyPropertyChanged();
            }
        }
        private bool isEditing = false;
        public bool IsEditing
        {
            get { return isEditing; }
            set
            {
                isEditing = value;
                NotifyPropertyChanged();
            }
        }
        
        public void initializeLevels(Dictionary<Model.CharacterClassType, int> levels)
        {
            this.levels = levels;
            NotifyPropertyChanged("Levels");
        }

        private string FormatClasses()
        {
            var builder = new StringBuilder();
            foreach (KeyValuePair<Model.CharacterClassType, int> entry in levels)
            {
                builder.AppendFormat("{0} {1}", entry.Key, entry.Value);
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion
    }
}
