using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheet5e.ViewModels
{
    public class IndividualSkillViewModel: BaseViewModel
    {
        private bool _isProficient = false;
        public bool IsProficient 
        {
            get { return _isProficient; }
            set
            {
                if (value != _isProficient) { delegateProficiencyChanged(Name, value); }
                _isProficient = value;
                NotifyPropertyChanged();

            }
        }
        public string Name { get; set; }

        private int _bonus = 0;
        public int Bonus
        {
            get { return _bonus; }
            set
            {
                _bonus = value;
                NotifyPropertyChanged();
            }
        }

        public Action<string, bool> delegateProficiencyChanged;
    }
}
