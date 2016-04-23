using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ICSheetCore
{
    public interface ISkill
    {
        string Name { get; }
        int Bonus { get; set; }
        bool IsTagged { get; set; }
    }
}
