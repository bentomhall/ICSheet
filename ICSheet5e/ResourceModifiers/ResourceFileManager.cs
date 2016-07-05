using System.IO;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System;

namespace ICSheet5e.ResourceModifiers
{
    public class ResourceFileManager
    {
        private string appName = "ICSheet5e";
        private string appDataPath => Environment.GetFolderPath(Environment.SpecialFolder.ApplicationData);
        private string getResourcePath(string forResource)
        {
            var combinedPath = Path.Combine(appDataPath, appName, forResource);
            if (File.Exists(combinedPath)) { return combinedPath; }
            else { return Path.Combine("Resources", forResource); }
        }

        public string CreatePathForResource(string resource)
        {
            Directory.CreateDirectory(Path.Combine(appDataPath, appName)); //only creates if it doesn't exist already
            return Path.Combine(appDataPath, appName, resource);
        }

        public string DefaultPathFor(string resource)
        {
            return Path.Combine("Resources", resource);
        }

        public bool ShouldMergeResources(string forResource)
        {
            var customPath = Path.Combine(appDataPath, appName, forResource);
            DateTime customModificationTime;
            DateTime defaultModificationTime = File.GetLastWriteTime(Path.Combine("Resources", forResource));
            try
            {
                customModificationTime = File.GetLastWriteTime(customPath);
            }
            catch (FileNotFoundException)
            {
                return false;
            }
            return defaultModificationTime.CompareTo(customModificationTime) > 0;
        }

        public string ArmorListPath
        {
            get { return getResourcePath("BasicArmors.xml"); }
        }

        public string WeaponListPath
        {
            get { return getResourcePath("BasicWeapons.xml"); }
        }

        public string ItemListPath
        {
            get { return getResourcePath("BasicItems.xml"); }
        }

        public string ClassFeaturesPath
        {
            get { return getResourcePath("ClassFeatures.xml"); }
        }

        public string RacialFeaturesPath
        {
            get { return getResourcePath("RacialFeatures.xml"); }
        }

        public string SpelllistPath
        {
            get { return getResourcePath("spell_list.xml"); }
        }

        public string SpellDetailsPath
        {
            get { return getResourcePath("SpellList5e.json"); }
        }
    }
}
