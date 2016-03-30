using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InteractiveCharacterSheetCore;
using System.Runtime.Serialization;

namespace ICSheet5e.Model
{
    [DataContract]
    public class Skill5e: ISkill
    {
        [DataMember] public string Name { get; set; }
        public int Bonus { get { return _bonus; } set { _bonus = value; } }
        [DataMember] private int _bonus = 0;
        [DataMember]
        public bool IsTagged { get; set; }

        public Skill5e(String name, int bonus, bool isTagged) 
        {
            this.Name = name;
            this._bonus = bonus;
            IsTagged = isTagged;
        }

    }
}
