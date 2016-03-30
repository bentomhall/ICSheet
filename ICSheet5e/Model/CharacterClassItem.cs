using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace ICSheet5e.Model
{
    [DataContract]
    public class CharacterClassItem
    {
        [DataMember]
        private CharacterClassType _classType;
        [DataMember]
        private int level;

        public CharacterClassItem(CharacterClassType type, int numberOfLevels)
        {
            _classType = type;
            level = numberOfLevels;
        }

        public void LevelUp()
        {
            level += 1;
        }

        public CharacterClassType ClassType { get { return _classType; } }
        public int Level { get { return level; } }
        public bool Matches(CharacterClassType otherType)
        {
            return _classType == otherType;
        }

        public override string ToString()
        {
            return String.Format("{0} {1}", _classType, level);
        }
    }
}
