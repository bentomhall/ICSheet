using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheetCore
{
    internal interface IAbilityDataSource
    {
        int AbilityModifierFor(AbilityType ability);
        int AbilityScoreFor(AbilityType ability);
    }
}
