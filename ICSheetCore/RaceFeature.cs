using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheetCore
{
    public class RaceFeature : IFeature
    {
        private string _name;
        private string _description;

        public RaceFeature(string name, string description)
        {
            _name = name;
            _description = description;
        }

        public void CombineDescriptions(RaceFeature other)
        {
            _description = $"{_description}. {other.Description}";
        }

        public string Description
        {
            get
            {
                return _description;
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
                return _name;
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
