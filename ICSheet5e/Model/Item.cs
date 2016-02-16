using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactive_Character_Sheet_Core;

namespace ICSheet5e.Model
{
    class Item: IItem
    {
        private string properties;
        private string _name;
        private int _weight;
        private int _value;
        private ItemSlot _slot;
        private int _enhancement = 0;
        private string description;

        public string Name { get { return _name; } }
        public int Weight { get { return _weight; } }
        public int Value { get { return _value; } }
        public ItemSlot Slot { get { return _slot; } }
        public int EnhancementBonus { get { return _enhancement; } }
        public bool IsWeapon { get { return (_slot == ItemSlot.Mainhand || _slot == ItemSlot.Offhand); } }
        public bool IsArmor { get { return (_slot == ItemSlot.Armor); } }
        Item(string name, int weight, int value, ItemSlot slot, int bonus=0, string properties)
        {
            _name = name;
            _weight = weight;
            _value = value;
            _slot = slot;
            _enhancement = bonus;
            this.properties = properties;
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

        public string ToString() { return description; }

    }
}
