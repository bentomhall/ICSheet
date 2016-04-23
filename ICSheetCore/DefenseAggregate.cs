using System;
using System.Collections.Generic;
namespace ICSheetCore
{
    internal class DefenseAggregate
    {
        private Dictionary<DefenseType, Defense> _defenses =  new Dictionary<DefenseType, Defense>();
        private IDictionary<DefenseType, int> _proficiencyForDefense;

        static private Dictionary<AbilityType, DefenseType> _defenseForAbility = new Dictionary<AbilityType, DefenseType>()
        {
            { AbilityType.Strength, DefenseType.Strength },
            { AbilityType.Dexterity, DefenseType.Dexterity },
            { AbilityType.Constitution, DefenseType.Constitution },
            { AbilityType.Intelligence, DefenseType.Intelligence },
            { AbilityType.Wisdom, DefenseType.Wisdom },
            { AbilityType.Charisma, DefenseType.Charisma }
        };

        internal DefenseAggregate(IAbilityDataSource abilities, IDictionary<DefenseType, int> proficientDefenses)
        {
            _defenses[DefenseType.Armor] = new Defense(10 + abilities.AbilityModifierFor(AbilityType.Dexterity), DefenseType.Armor); //there is never proficiency in armor class
            _proficiencyForDefense = proficientDefenses;
            _defenses[DefenseType.Strength] = new Defense(abilities.AbilityModifierFor(AbilityType.Strength), _proficiencyForDefense[DefenseType.Strength], 0, DefenseType.Strength);
            _defenses[DefenseType.Dexterity] = new Defense(abilities.AbilityModifierFor(AbilityType.Dexterity), _proficiencyForDefense[DefenseType.Dexterity], 0, DefenseType.Dexterity);
            _defenses[DefenseType.Constitution] = new Defense(abilities.AbilityModifierFor(AbilityType.Constitution), _proficiencyForDefense[DefenseType.Constitution], 0, DefenseType.Constitution);
            _defenses[DefenseType.Intelligence] = new Defense(abilities.AbilityModifierFor(AbilityType.Intelligence), _proficiencyForDefense[DefenseType.Intelligence], 0, DefenseType.Intelligence);
            _defenses[DefenseType.Wisdom] = new Defense(abilities.AbilityModifierFor(AbilityType.Wisdom), _proficiencyForDefense[DefenseType.Wisdom], 0, DefenseType.Wisdom);
            _defenses[DefenseType.Charisma] = new Defense(abilities.AbilityModifierFor(AbilityType.Charisma), _proficiencyForDefense[DefenseType.Charisma], 0, DefenseType.Charisma);
        }



        /// <summary>
        /// Fetch the total bonus for the requested type of defense, including base value, proficiency, and any manual overrides.
        /// </summary>
        /// <param name="defense"></param>
        /// <returns></returns>
        internal int DefenseValueFor(DefenseType defense)
        {
            return _defenses[defense].Value;
        }

        //<summary>
        //Handles all changes due to modified ability modifiers.
        //WARNING: does not handle changes to AC.
        //
        //<seealso cref="ModifyAC"/>
        //</summary>
        internal void ModifyAbilityBonus(AbilityType modifiedAbility, int newModifier)
        {
            var defenseType = _defenseForAbility[modifiedAbility];
            var oldDefense = _defenses[defenseType];
            var newDefense = new Defense(newModifier, oldDefense.Proficiency, oldDefense.Adjustment, defenseType);
            _defenses[defenseType] = newDefense;
            return;
        }

        internal void ModifyAbilityBonus(object sender, AbilityModifiedEventArgs e)
        {
            ModifyAbilityBonus(e.ModifiedAbility, e.Modifier);
        }

        //<summary>
        //Sets the AC to the given base value. Any manual override adjustments are preserved.
        //
        //<seealso cref="ModifyDefenseAdjustment"/>
        //</summary>
        internal void ModifyAC(int newValue)
        {
            var adjustment = _defenses[DefenseType.Armor].Adjustment;
            _defenses[DefenseType.Armor] = new Defense(newValue, 0, adjustment, DefenseType.Armor);
            return;
        }

        //<summary>
        //Sets a new defense adjustment. Works for all defense types including AC.
        //</summary>
        internal void ModifyDefenseAdjustment(DefenseType defense, int newAdjustment)
        {
            var oldDefense = _defenses[defense];
            _defenses[defense] = new Defense(oldDefense.BaseValue, oldDefense.Proficiency, newAdjustment, defense);
            return;
        }

        internal int GetDefenseAdjustment(DefenseType defense)
        {
            return _defenses[defense].Adjustment;
        }

        internal void ChangeACFromArmor(ArmorItem item, IAbilityDataSource abilities, int baseAC)
        {
            int ac = 10;
            switch(item.ArmorClassType)
            {
                case ArmorType.None:
                    ac = baseAC;
                    break;
                case ArmorType.Light:
                    ac = item.ArmorBonus + abilities.AbilityModifierFor(AbilityType.Dexterity) + item.EnhancementBonus;
                    break;
                case ArmorType.Medium:
                    ac = item.ArmorBonus + Math.Min(abilities.AbilityModifierFor(AbilityType.Dexterity), 2) + item.EnhancementBonus;
                    break;
                case ArmorType.Heavy:
                    ac = item.ArmorBonus + item.EnhancementBonus;
                    break;
            }
            ModifyAC(ac);
        }

        
        internal int ArmorClass { get { return DefenseValueFor(DefenseType.Armor); } }
        internal int StrengthSave { get { return DefenseValueFor(DefenseType.Strength); } }
        internal int DexteritySave { get { return DefenseValueFor(DefenseType.Dexterity); } }
        internal int ConstitutionSave { get { return DefenseValueFor(DefenseType.Constitution); } }
        internal int IntelligenceSave { get { return DefenseValueFor(DefenseType.Intelligence); } }
        internal int WisdomSave { get { return DefenseValueFor(DefenseType.Wisdom); } }
        internal int CharismaSave { get { return DefenseValueFor(DefenseType.Charisma); } }

    }
}