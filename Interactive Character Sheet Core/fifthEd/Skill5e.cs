using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Character_Sheet_Core.fifthEd
{
    class Skill5e: ISkill
    {
        public string name { get; set; }
        public int bonus { get; }
        private int _bonus = 0;

        public Skill5e(String name, int bonus) 
        {
            this.name = name;
            this._bonus = bonus;
        }
    }
}
