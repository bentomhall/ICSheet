using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace ICSheet5e.ViewModels
{
    class CharacterViewModel : INotifyPropertyChanged
    {
        private Model.Character character;
        private bool canEdit = false;
        private AttributeBoxViewModel attributes = new AttributeBoxViewModel();
        private ClassInformationViewModel info = new ClassInformationViewModel();
        private SkillViewModel skills = new SkillViewModel();


        public AttributeBoxViewModel AbilitiesVM
        {
            get { return attributes; }
        }

        public ClassInformationViewModel ClassInfoVM
        {
            get { return info; }
        }

        public SkillViewModel SkillsVM
        {
            get { return skills; }
        }
        public bool CanEdit
        {
            get { return canEdit; }
            set
            {
                canEdit = value;
                NotifyPropertyChanged();
                if (!canEdit) NotifyEditingEnded(); //raise property changed notifications for directly bound properties
            }
        }
        public Model.Character Character 
        {
            get { return character; }
            set
            {
                character = value;
                NotifyPropertyChanged();
            }
        }
        public int ArmorClass
        {
            get { return character.ArmorClass; }
            set
            {
                character.ArmorClass = value;
                NotifyPropertyChanged();
            }
        }
        public int Proficiency
        {
            get { return character.Proficiency; }
        }
        public int Movement
        {
            get { return character.Movement; }
        }
        public int Initiative
        {
            get { return character.Initiative; }
        }

        public CharacterViewModel() { }

        public CharacterViewModel(Model.Character c)
        {
            character = c;
            attributes.SetAllAbilityScores(c.Abilities);
            info.Name = c.CharacterName;
            info.initializeLevels(c.Levels);
            info.Race = c.Race;
            info.Experience = c.Experience;
            skills.SetAllSkills(c.Skills);
        }

        public void NotifyEditingEnded()
        {
            NotifyPropertyChanged("ArmorClass");
            NotifyPropertyChanged("Proficiency");
            NotifyPropertyChanged("Movement");
            NotifyPropertyChanged("Initiative");
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
