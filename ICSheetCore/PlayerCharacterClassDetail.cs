using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace ICSheetCore
{
    internal class PlayerCharacterClassDetail
    {
        private readonly string _className;
        private int _level;
        private List<IFeature> _features;

        internal PlayerCharacterClassDetail(string className, int level, IEnumerable<IFeature> features)
        {
            _className = className;
            _level = level;
            setFeatures(features);
        }

        internal int speedBonus(ArmorType armor, bool isWearingShield)
        {
            var unarmoredMovement = _features.Count(x => x.Name == "Unarmored Movement") > 0;
            
            if (unarmoredMovement && Level >= 2 && armor == ArmorType.None && !isWearingShield)
            {
                return 10 + 5 * (Level - 2) / 4;
            }

            var fastMovement = _features.Count(x => x.Name == "Fast Movement") > 0;
            if (fastMovement && Level >= 5 && armor != ArmorType.Heavy)
            {
                return 10;
            }

            return 0;
        }

        internal int BaseArmorClass(IAbilityDataSource abilities, ArmorType armor, bool isWearingShield)
        {
            var ac = 10 + abilities.AbilityModifierFor(AbilityType.Dexterity);
            if (armor != ArmorType.None) { return 10; } //all the armor bonuses rely on wearing no armor. Shields count separately
            var isBarbarian = _features.Count(x => x.Name == "Unarmored Defense (Barbarian)") > 0;
            if (isBarbarian)
            {
                return ac + abilities.AbilityModifierFor(AbilityType.Constitution);
            }
            var isMonk = _features.Count(x => x.Name == "Unarmored Defense (Monk)") > 0;
            if (isMonk && !isWearingShield)
            {
                return ac + abilities.AbilityModifierFor(AbilityType.Wisdom);
            }
            var isDragonSorceror = _features.Count(x => x.Name == "Draconic Resilience") > 0;
            if (isDragonSorceror)
            {
                return ac + 3;
            }
            return ac; //this covers unarmored wizards (etc).
        }

        internal ISpellcastingFeature Spellcasting
        {
            get { return (ISpellcastingFeature)_features.SingleOrDefault(x => x.Name == "Spellcasting"); }
        }

        internal string Name { get { return _className; } }

        internal int Level
        {
            get
            {
                return _level;
            }
        }

        internal void AddLevel()
        {
            _level += 1;
        }

        internal IEnumerable<DefenseType> ProficientDefenses
        {
            get
            {
                return _proficientDefenses;
            }
        }

        internal IEnumerable<IFeature> Features { get { return _features; } }

        internal bool HasFeature(string name)
        {
            return _features.Count(x => x.Name == name) > 0;
        }

        internal void AddClassFeature(IFeature feature)
        {
            _features.Add(feature);
        }

        private void setFeatures(IEnumerable<IFeature> features)
        {
            _features = new List<IFeature>(features);
            handleDefenses();
        }

        private void handleDefenses()
        {
            var defenseFeature = _features.Single(x => x.Name == "Saving Throw Proficiencies");
            foreach (var detail in defenseFeature.Description.Split(','))
            {
                _proficientDefenses.Add((DefenseType)Enum.Parse(typeof(DefenseType), detail.Trim()));
            }
            _features.Remove(defenseFeature);
        }

        private List<DefenseType> _proficientDefenses = new List<DefenseType>();
    }
}
