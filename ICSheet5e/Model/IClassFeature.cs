using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheet5e.Model
{
    interface IClassFeature
    {
        public string Name { get; }
        public string Uses { get; }
        public string Description { get; }
        public string ToString();
    }
}
