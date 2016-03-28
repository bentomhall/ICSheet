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
        [DataMember] public string name { get; set; }
        public int bonus { get { return _bonus; } set { _bonus = value; } }
        [DataMember] private int _bonus = 0;
        [DataMember]
        public bool IsTagged { get; set; }

        public Skill5e(String name, int bonus, bool isTagged) 
        {
            this.name = name;
            this._bonus = bonus;
            IsTagged = isTagged;
        }

    }
}
