using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheetCore
{
    class RaceFeature : IFeature
    {
        public string Description
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public bool IsMulticlassInheritable
        {
            get
            {
                return false;
            }
        }

        public string Name
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public int StartsFromLevel
        {
            get
            {
                return 1;
            }
        }
    }
}
