using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheetCore
{
    public static class ListExtensions
    {
        public static List<int> Zeros(this List<int> lst, int elementCount)
        {
            return Enumerable.Repeat(0, elementCount).ToList();
        } 
    }
}
