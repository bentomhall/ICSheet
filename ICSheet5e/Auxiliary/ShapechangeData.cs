using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using ICSheet5e.ViewModels;
using ICSheetCore;

namespace ICSheet5e.Auxiliary
{
    public class ShapechangeData
    {
        string _name;
        string _challengeRating;
        Dictionary<AbilityType, Ability> _abilities;
        List<AttackViewModel> _attacks;
        Dictionary<string, int> _skillBonuses;
        string _movement;
        int _armorClass;
        int _maxHP;
        List<IFeature> _features;

        public ShapechangeData()
        {
            _abilities = new Dictionary<AbilityType, Ability>();
            _attacks = new List<AttackViewModel>();
            _skillBonuses = new Dictionary<string, int>();
            _features = new List<IFeature>();
        }

        public string Name
        {
            get
            {
                return _name;
            }

            set
            {
                _name = value;
            }
        }

        public string ChallengeRating
        {
            get
            {
                return _challengeRating;
            }

            set
            {
                _challengeRating = value;
            }
        }

        public Dictionary<AbilityType, Ability> Abilities
        {
            get
            {
                return _abilities;
            }

            set
            {
                _abilities = value;
            }
        }

        public List<AttackViewModel> Attacks
        {
            get
            {
                return _attacks;
            }

            set
            {
                _attacks = value;
            }
        }

        public Dictionary<string, int> SkillBonuses
        {
            get
            {
                return _skillBonuses;
            }

            set
            {
                _skillBonuses = value;
            }
        }

        public int MaxHP
        {
            get
            {
                return _maxHP;
            }

            set
            {
                _maxHP = value;
            }
        }

        public List<IFeature> Features
        {
            get
            {
                return _features;
            }

            set
            {
                _features = value;
            }
        }

        public string Movement
        {
            get
            {
                return _movement;
            }

            set
            {
                _movement = value;
            }
        }

        public int ArmorClass
        {
            get
            {
                return _armorClass;
            }

            set
            {
                _armorClass = value;
            }
        }
    }
}
