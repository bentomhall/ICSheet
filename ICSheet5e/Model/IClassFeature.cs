using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheet5e.Model
{
    public interface IClassFeature
    {
        string Name { get; }
        string Uses { get; }
        string Description { get; }
        int MinimumLevel { get; }
        string ToString();
        
        void AddDescriptionText(string newText);

        bool TryUseFeature();
        void ResetUses();
    }
}
