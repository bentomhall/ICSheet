namespace ICSheetCore
{
    /// <summary>
    /// Implements IFeature for races. Starting level of 1 for everything.
    /// </summary>
    public class RaceFeature : IFeature
    {
        private string _name;
        private string _description;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="description"></param>
        public RaceFeature(string name, string description)
        {
            _name = name;
            _description = description;
        }

        /// <summary>
        /// Concats the two descriptions together if the names match.
        /// </summary>
        /// <param name="other"></param>
        public void CombineDescriptions(RaceFeature other)
        {
            if (Name != other.Name) { return; }
            _description = $"{_description}. {other.Description}";
        }

        /// <summary>
        /// 
        /// </summary>
        public string Description
        {
            get
            {
                return _description;
            }
        }

        /// <summary>
        /// Not applicable to race features.
        /// </summary>
        public bool IsMulticlassInheritable
        {
            get
            {
                return false;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string Name
        {
            get
            {
                return _name;
            }
        }

        /// <summary>
        /// All racial features start at level 1.
        /// </summary>
        public int StartsFromLevel
        {
            get
            {
                return 1;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public string FullDescription
        {
            get
            {
                return Description;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return $"{_name}: {_description}";
        }
    }
}
