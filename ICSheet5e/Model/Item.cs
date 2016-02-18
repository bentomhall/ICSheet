using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactive_Character_Sheet_Core;
using System.Runtime.Serialization;

namespace ICSheet5e.Model
{
    [DataContract]
    public class Item: IItem
    {
        [DataMember] private string properties;
        [DataMember] private string _name;
        [DataMember] private int _weight;
        [DataMember] private int _value;
        [DataMember] private ItemSlot _slot;
        [DataMember] private int _enhancement = 0;
        [DataMember] private string description = "";
        [DataMember] private bool _isProficient;
        [DataMember] private List<AbilityType> associatedAbility = new List<AbilityType>();

        public List<AbilityType> AssociatedAbilities { get { return associatedAbility; } }
        public string Name { get { return _name; } }
        public bool IsProficient { get { return _isProficient; } }
        public int Weight { get { return _weight; } }
        public int Value { get { return _value; } }
        public ItemSlot Slot { get { return _slot; } }
        public int EnhancementBonus { get { return _enhancement; } }
        public bool IsWeapon { get { return (_slot == ItemSlot.Mainhand || _slot == ItemSlot.Offhand); } }
        public bool IsArmor { get { return (_slot == ItemSlot.Armor); } }
        [DataMember] public bool isRanged { get; set; }
        Item(string name, int weight, int value, ItemSlot slot, bool proficient, string properties, int bonus=0)
        {
            _name = name;
            _weight = weight;
            _value = value;
            _slot = slot;
            _enhancement = bonus;
            _isProficient = proficient;
            this.properties = properties;
            if (properties.Contains("Finesse"))
            {
                associatedAbility.Add(AbilityType.Dexterity);
                associatedAbility.Add(AbilityType.Strength );
            }
            else if (isRanged)
            {
                associatedAbility.Add(AbilityType.Dexterity);
            }
            else
            {
                associatedAbility.Add(AbilityType.Strength);
            }
        }

        private string CollectDescription()
        {
            StringBuilder output = new StringBuilder(_name);
            if (_enhancement != 0) 
            {
                output.Append(string.Format(" +{0}", _enhancement));
                output.Append(Environment.NewLine);
            }
            output.Append(properties);
            return output.ToString();
        }

        public override string ToString() { return description; }

    }
}
