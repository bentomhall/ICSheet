using System;
using System.Linq;
using System.Collections.Generic;

namespace ICSheetCore
{
    internal class PlayerClassAggregate
    {
        private List<PlayerCharacterClassDetail> _playerClasses;
        private int _proficiencyBonus;
        private SpellCastingAggregate _spellcastingAggregate;
        private List<IFeature> _customFeatures = new List<IFeature>();
        private Dictionary<DefenseType, int> _proficienctDefenses = new Dictionary<DefenseType, int>()
        {
            {DefenseType.Strength, 0 },
            {DefenseType.Dexterity, 0 },
            {DefenseType.Constitution, 0 },
            {DefenseType.Intelligence, 0 },
            {DefenseType.Wisdom, 0 },
            {DefenseType.Charisma, 0 }
        };
        private int _totalLevel;

        private int calculateProficiencyBonus()
        {
            return (_totalLevel - 1) / 4 + 2;
        }


        internal PlayerClassAggregate(IEnumerable<PlayerCharacterClassDetail> classesAndLevels, SpellManager spellDB)
        {
            _playerClasses = classesAndLevels.ToList();
            _totalLevel = _playerClasses.Sum(x => x.Level);
            _proficiencyBonus = calculateProficiencyBonus();
            calculateDefenses();
            _spellcastingAggregate = createSpellcastingAggregate(spellDB);
            collectFeatures();
        }

        private void calculateDefenses()
        {
            foreach (var defense in _playerClasses[0].ProficientDefenses)
            {
                _proficienctDefenses[defense] = _proficiencyBonus;
            }
        }

        internal IEnumerable<int> AvailableSpellSlots { get { return _spellcastingAggregate.AvailableSpellSlots; } }

        internal bool IsSpellcaster { get { return _spellcastingAggregate.CanCastSpells; } }

        internal string SpellsPreparedOfMax(IAbilityDataSource abilities)
        {
            var spellcastingLevels = _playerClasses.Where(x => x.Spellcasting != null)
                                                   .ToDictionary(x => x.Name,
                                                                 x => x.Level);
            return _spellcastingAggregate.PreparedSpellUtilization(abilities, spellcastingLevels);
        }

        private SpellCastingAggregate createSpellcastingAggregate(SpellManager db)
        {
            var castingFeatures = new List<ISpellcastingFeature>();
            var levels = new List<int>();
            foreach (var c in _playerClasses)
            {
                var f = c.Spellcasting;
                if (f != null)
                {
                    castingFeatures.Add(f);
                    levels.Add(c.Level);
                }
            }
            return new SpellCastingAggregate(castingFeatures, levels, db);
        }

        internal void AddFeature(IFeature feature)
        {
            _customFeatures.Add(feature);
            _shouldRefreshFeatures = true;
        }



        internal IDictionary<DefenseType, int> ProficiencyForDefenses
        {
            get { return _proficienctDefenses; }
        }

        internal int ProficiencyBonus { get { return _proficiencyBonus; } }

        internal bool HasFeature(string withName)
        {
            return _playerClasses.Count(x => x.HasFeature(withName)) > 0 || _customFeatures.Count(x => x.Name == withName) > 0;
        }

        internal IEnumerable<IFeature> AllFeatures
        {
            get
            {
                if (_shouldRefreshFeatures) { collectFeatures(); }
                return _features;
            }
        }

        private bool _shouldRefreshFeatures = false;
        private List<IFeature> _features;
        private void collectFeatures()
        {
            _features = new List<IFeature>();
            foreach (var c in _playerClasses)
            {
                _features.AddRange(c.Features.Where(x => x.StartsFromLevel <= c.Level));
            }
            _features.AddRange(_customFeatures);
        }

        internal IEnumerable<Spell> PreparedSpells { get { return _spellcastingAggregate.PreparedSpells; } }

        internal void LevelUp(string className, IEnumerable<IFeature> newFeatures)
        {
            var currentClass = _playerClasses.SingleOrDefault(x => x.Name == className);
            if (currentClass != null) {
                currentClass.AddLevel();
                _spellcastingAggregate.IncreaseLevel(currentClass.Name);
            }
            else
            {
                var cls = new PlayerCharacterClassDetail(className, 1, newFeatures);
                _playerClasses.Add(cls);
                _spellcastingAggregate.AddSpellcasting(cls.Features.SingleOrDefault(x => x.Name == "Spellcasting"));
            }
            invalidateClassData();
        }

        internal void ResetSpellSlots(IDictionary<int, int> slotsToRecover)
        {
            foreach (KeyValuePair<int, int> entry in slotsToRecover)
            {
                if (entry.Value == 0) { continue; }
                if (entry.Key == -1)
                {
                    _spellcastingAggregate.ResetAllSlots();
                    return;
                }
                else if (HasFeature("Arcane Recovery") || HasFeature("Pact Magic") ||HasFeature("Flexible Casting") || HasFeature("Natural Recovery"))
                {
                    _spellcastingAggregate.RegainSpellSlot(entry.Key, entry.Value);
                }
            }
        }

        private void invalidateClassData()
        {
            _totalLevel += 1;
            _proficiencyBonus = calculateProficiencyBonus();
            calculateDefenses();
            collectFeatures();
        }

        internal void UseSpellSlot(int level)
        {
            _spellcastingAggregate.UseSpellSlot(level);
        }

        internal IReadOnlyDictionary<string, int> SpellAttackBonuses(IAbilityDataSource abilities)
        {
            return _spellcastingAggregate.SpellAttackBonusesWith(abilities, ProficiencyBonus);
        }

        internal IReadOnlyDictionary<string, int> SpellDCs(IAbilityDataSource abilities)
        {
            return _spellcastingAggregate.SpellDCsWith(abilities, ProficiencyBonus);
        }

        internal int BaseACWith(IAbilityDataSource abilities, ArmorType armorWeight, bool hasShield)
        {
            return _playerClasses.Max(x => x.BaseArmorClass(abilities, armorWeight, hasShield));
        }

        internal void LearnSpell(string spellName, string asClass, bool isBonus)
        {
            _spellcastingAggregate.LearnSpell(spellName, asClass, isBonus);
        }

        internal void UnlearnSpell(string spellName, string asClass)
        {
            _spellcastingAggregate.UnlearnSpell(spellName, asClass);
        }

        internal SpellcastingLookup.CastingType CastingType
        {
            get
            {
                var f = _features.FindAll(x => x.Name == "Spellcasting");
                if (f.Count == 0) { return SpellcastingLookup.CastingType.None; }
                else 
                {
                    var sf = f[0] as SpellcastingFeature;
                    if (sf != null) { return sf.CasterType; }
                }
                return SpellcastingLookup.CastingType.None;
            }
        }


        internal void PrepareSpell(string spellName, string asClass, bool asBonus)
        {
            _spellcastingAggregate.PrepareSpell(spellName, asClass, asBonus);
        }

        internal void UnprepareSpell(string spellName, string asClass)
        {
            _spellcastingAggregate.UnprepareSpell(spellName, asClass);
        }

        internal int MovementSpeed(IInventoryDataSource inventory)
        {
            var armorType = (inventory.EquippedItemForSlot(ItemSlot.Armor) as ArmorItem).ArmorClassType;
            var hasShield = inventory.EquippedItemForSlot(ItemSlot.Offhand) as ArmorItem != null;
            return _playerClasses.Max(x => x.speedBonus(armorType, hasShield));
        }

        internal int InitiativeBonus(IAbilityDataSource abilities)
        {
            var hasBonus = _playerClasses.Count(x => x.HasFeature("Jack of All Trades")) > 0;
            return abilities.AbilityModifierFor(AbilityType.Dexterity) + (hasBonus ? ProficiencyBonus / 2 : 0);
        }

        internal IDictionary<string, int> Levels
        {
            get { return _playerClasses.ToDictionary(x => x.Name, x => x.Level); }
        }

        internal IEnumerable<IFeature> CustomFeatures
        {
            get { return _customFeatures; }
        }

        public IEnumerable<Spell> KnownSpells
        {
            get { return _spellcastingAggregate.KnownSpells; }
        }

        public IEnumerable<string> SpellcastingClasses
        {
            get { return _playerClasses.Where(x => x.HasFeature("Spellcasting")).Select(x => x.Spellcasting.SpellBookName); }
        }

        internal void AddSubclass(string forClass, string withName, IEnumerable<IFeature> features)
        {
            var pcClass = _playerClasses.SingleOrDefault(x => x.Name == forClass);
            foreach (var f in features)
            {
                if (f.Name != withName) { pcClass.AddClassFeature(f); }
            }
            var newSpellcasting = features.SingleOrDefault(x => x.Name == "Spellcasting");
            if (newSpellcasting != null)
            {
                _spellcastingAggregate.AddSpellcasting(newSpellcasting);
            }
            _shouldRefreshFeatures = true;
        }
    }

    internal class ClassInformationChangedEventArgs : EventArgs
    {

    }
}