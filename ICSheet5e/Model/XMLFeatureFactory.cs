using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Xml.Linq;
using Interactive_Character_Sheet_Core;

namespace ICSheet5e.Model
{
    class XMLFeatureFactory
    {
        private string _raceFeaturesXMLPath;
        private string _classFeaturesXMLPath;

        public List<IClassFeature> RacialFeatures(Race forRace)
        {
            throw new NotImplementedException();
        }

        public List<IClassFeature> ClassFeatures(CharacterClassType classType)
        {
            throw new NotImplementedException();
        }
    }
}
