using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ICSheet5e.Model
{
    [DataContract]
    class MartialFeature: IClassFeature
    {
        [DataMember] string _name;
        [DataMember] string _description;
        [DataMember] string _uses;
        [DataMember] int _usesRemainingToday;
        [DataMember] int _totalUses;

        public string Name { get { return _name; } }
        public string Description { get { return _description; } }
        public string Uses { get { return _uses; } }

        public class UnuseableException: System.Exception
        {
            public UnuseableException() { }

            public UnuseableException(string message) : base(message) { }

            public UnuseableException(string message, Exception inner) : base(message, inner) { }
        }

        MartialFeature(string name, string description, string uses)
        {
            _name = name;
            _description = description;
            _uses = uses;
            int.TryParse(uses.Split('/')[0], out _totalUses); // eg '3/day' ->3
            ResetUses();
        }

        public override string ToString()
        {
            return String.Format("{0}: {1}",_name, _description);
        }

        public bool TryUseFeature()
        {
            if (_usesRemainingToday == 0) { return false; }
            else
            {
                _usesRemainingToday -= 1;
                return true;
            }

        }

        public void ResetUses()
        {
            _usesRemainingToday = _totalUses;
        }
    }
}
