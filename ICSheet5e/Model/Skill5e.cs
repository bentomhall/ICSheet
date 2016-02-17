using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactive_Character_Sheet_Core;

namespace ICSheet5e.Model
{
    public class Skill5e: ISkill
    {
        public string name { get; set; }
        public int bonus { get { return _bonus; } }
        private int _bonus = 0;

        public Skill5e(String name, int bonus) 
        {
            this.name = name;
            this._bonus = bonus;
        }

    }
}
