using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Character_Sheet_Core
{
    public interface IRace
    {
        string ToString();
        Enum Value { get; }
    }
}
