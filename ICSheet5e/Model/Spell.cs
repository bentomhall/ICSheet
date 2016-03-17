using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;


namespace ICSheet5e.Model
{
    [DataContract]
    public class Spell
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public string Description { get; set; }

        [DataMember]
        public string Range { get; set; }

        [DataMember]
        public string Components { get; set; }

        [DataMember]
        public int Level { get; set; }

        [DataMember]
        public List<CharacterClassType> CastableBy { get; set; }

        [DataMember]
        public string School { get; set; }

        [DataMember]
        public string CastTime { get; set; }

        [DataMember]
        public bool IsPrepared { get; set; }

        [DataMember]
        public string Duration { get; set; }

        public override string ToString()
        {
            return Name;
        }

        public string FullDescription
        {
            get
            {
                return String.Format("Level: {0}\r\nCast Time: {1}\r\nRange: {2}\r\nDuration: {3}\r\n{4}", Level, CastTime, Range, Duration, Description);
            }
        }
    }
}
