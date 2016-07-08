using System;
using System.Collections.Generic;
using System.Linq;

namespace ICSheetCore
{
    /// <summary>
    /// A race that a player character can be.
    /// </summary>
    public class PCRace : IRace
    {
        private List<IFeature> _features;
        private int _baseMovement;
        private string _baseraceName;
        private string _subraceName;

        private int calculateMovement()
        {
            var movementFeature = _features.SingleOrDefault(x => x.Name == "Speed");
            if (movementFeature == null) { return 30; }
            _features.Remove(movementFeature);
            var movement = 30;
            try
            {
                movement = int.Parse(movementFeature.Description);
            }
            catch (FormatException)
            {
                return movement;
            }
            if (movement <= 0) { return 30; }
            return movement;
        }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="baseName"></param>
        /// <param name="subName"></param>
        /// <param name="features"></param>
        public PCRace(string baseName, string subName, IEnumerable<IFeature> features)
        {
            _baseraceName = baseName;
            _subraceName = subName;
            _features = features.ToList();
            _baseMovement = calculateMovement();
        } 

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString()
        {
            return RaceName;
        }

        /// <summary>
        /// 
        /// </summary>
        public int BaseMovement
        {
            get
            {
                return _baseMovement;
            }
        }

        /// <summary>
        /// 
        /// </summary>
        public IEnumerable<IFeature> Features
        {
            get
            {
                return _features;
            }
        }

        /// <summary>
        /// The name of a race is the name of the subrace except for variant humans (which are human) and races with no subraces. Those use their base race name instead.
        /// </summary>
        public string RaceName
        {
            get
            {
                if (_subraceName == "Variant Human") { return "Human"; }
                return _subraceName ?? _baseraceName;
            }
        }


        /// <summary>
        /// Name of the subrace chosen. Can be null (indicating that the race has no subraces).
        /// </summary>
        public string SubraceName
        {
            get
            {
                return _subraceName;
            }
        }

        /// <summary>
        /// Adds a custom racial feature.
        /// </summary>
        /// <param name="feature"></param>
        public void AddFeature(IFeature feature)
        {
            _features.Add(feature as RaceFeature);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public Tuple<string, string> GetInformation()
        {
            return new Tuple<string, string>(_baseraceName, _subraceName);
        }
    }
}
