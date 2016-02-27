using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Collections.ObjectModel;
using Interactive_Character_Sheet_Core;

namespace ICSheet5e.ViewModels
{
    class SkillViewModel
    {
        public ObservableCollection<IndividualSkillViewModel> Skills { get; set; }

        public SkillViewModel()
        {

        }

        public void SetAllSkills(SkillList<Model.Skill5e> skills)
        {
            var names = skills.getSkillNames();
            Skills = new ObservableCollection<IndividualSkillViewModel>();
            foreach (var name in names)
            {
                IndividualSkillViewModel skill = new IndividualSkillViewModel();
                skill.Bonus = skills.skillBonusFor(name);
                skill.Name = name;
                skill.IsProficient = skills.IsSkillTagged(name);
                Skills.Add(skill);
            }
        }
    }
}
