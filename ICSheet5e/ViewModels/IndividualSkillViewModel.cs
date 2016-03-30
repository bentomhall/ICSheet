using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheet5e.ViewModels
{
    public class IndividualSkillViewModel: BaseViewModel
    {
        public bool IsProficient 
        {
            get { return Skill.IsTagged; }
            set
            {
                Skill.IsTagged = value;
                delegateProficiencyChanged(Skill);
                NotifyPropertyChanged();

            }
        }
        public string Name { get { return Skill.Name; } }

        public int Bonus
        {
            get { return Skill.Bonus; }
            set
            {
                Skill.Bonus = value;
                NotifyPropertyChanged();
            }
        }

        public Model.Skill5e Skill { get; set; }
        public Action<Model.Skill5e> delegateProficiencyChanged;

        public string FullDescription
        {
            get { return Name; }
        }
    }
}
