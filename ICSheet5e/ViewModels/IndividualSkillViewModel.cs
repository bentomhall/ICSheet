using System;
using ICSheetCore;
using System.Windows.Input;

namespace ICSheet5e.ViewModels
{
    public class IndividualSkillViewModel: BaseViewModel
    {
        public IndividualSkillViewModel(string name, int bonus, ProficiencyType proficiencyType)
        {
            _bonus = bonus;
            Name = name;
            proficiency = proficiencyType;
        }

        public string Proficiency 
        {
            get
            {
                switch(proficiency)
                {
                    case ProficiencyType.Expertise:
                        return "2x";
                    case ProficiencyType.Full:
                        return "1x";
                    case ProficiencyType.Half:
                        return "1/2x";
                    case ProficiencyType.None:
                        return "--";
                }
                return "--";
            }
        }
        public string Name { get; private set; }

        public int Bonus
        {
            get { return _bonus; }
            set
            {
                _bonus = value;
                NotifyPropertyChanged();
            }
        }

        public Action<IndividualSkillViewModel, ProficiencyType> delegateProficiencyChanged;

        public string FullDescription
        {
            get { return Name; }
        }

        public ICommand ToggleProficiency
        {
            get { return new Views.DelegateCommand<object>(ToggleProficiencyCommandExecuted); }
        }

        private void ToggleProficiencyCommandExecuted(object obj)
        {
            proficiency = (ProficiencyType)((int)((proficiency) + 1) % 4);
            delegateProficiencyChanged?.Invoke(this, proficiency);
            NotifyPropertyChanged("Proficiency");
        }

        private int _bonus;
        private ProficiencyType proficiency;
    }
}
