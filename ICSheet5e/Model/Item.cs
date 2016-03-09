﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactive_Character_Sheet_Core;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ICSheet5e.Model
{

    [DataContract, KnownType(typeof(ArmorItem)), KnownType(typeof(WeaponItem))]
    public class Item: IItem
    {
        [DataMember] private string properties;
        [DataMember] private string _name;
        [DataMember] private double _weight;
        [DataMember] private double _value;
        [DataMember] private ItemSlot _slot;
        [DataMember] private int _enhancement = 0;
        [DataMember] private string description = "";
        [DataMember] private bool _isProficient;
        [DataMember] private List<AbilityType> associatedAbility = new List<AbilityType>();
        [DataMember]
        private bool _isEquipped = false;
        [DataMember]
        private int _stackSize = 1;

        public List<AbilityType> AssociatedAbilities { get { return associatedAbility; } }
        public string Name { get { return _name; } }
        public bool IsProficient { get { return _isProficient; } }
        public double Weight { get { return _weight; } }
        public double Value { get { return _value; } }
        public ItemSlot Slot { get { return _slot; } set { _slot = value; NotifyPropertyChanged(); } }
        public int EnhancementBonus { get { return _enhancement; } }
        public bool IsWeapon { get { return (_slot == ItemSlot.Mainhand || _slot == ItemSlot.Offhand) || _slot == ItemSlot.TwoHanded; } }
        public bool IsArmor { get { return (_slot == ItemSlot.Armor); } }
        public int Count { get { return _stackSize; } set { _stackSize = value; NotifyPropertyChanged(); } }
        [DataMember] public string BaseEffect { get; set; }
        [DataMember] public bool IsEquipped 
        {
            get { return _isEquipped; }
            set
            {
                _isEquipped = value;
                NotifyPropertyChanged();
            }
        }
        [DataMember] public bool isRanged { get; set; }
        public Item(string name, double weight, double value, ItemSlot slot, bool proficient, string properties, int bonus=0)
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

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

    }
}
