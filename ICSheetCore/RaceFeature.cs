namespace ICSheetCore
{
    /// <summary>
    /// Implements IFeature for races. Starting level of 1 for everything.
    /// </summary>
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

        public override string ToString()
        {
            return $"{_name}: {_description}";
        }
    }
}
