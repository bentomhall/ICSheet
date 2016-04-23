using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace ICSheetCore.Data
{
    [DataContract]
    class CharacterData
    {
        [DataMember]
        public string Name { get; set; }

        [DataMember]
        public IDictionary<AbilityType, int> AbilityScores { get; set; }
    }
}
