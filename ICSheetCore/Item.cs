using System;
using System.Collections.Generic;
using System.Text;
using System.Runtime.CompilerServices;
using System.Runtime.Serialization;
using System.ComponentModel;

namespace ICSheetCore
{
    /// <summary>
    /// A base class for inventory items. Also represents any generic non-weapon, non-armor item. More specific types should be subclassed.
    /// </summary>
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

        /// <summary></summary>
        public IEnumerable<AbilityType> AssociatedAbilities { get { return associatedAbility; } }
        /// <summary></summary>
        public string Name { get { return _name; } }
        /// <summary></summary>
        public bool IsProficient { get { return _isProficient; } }
        /// <summary></summary>
        public double Weight { get { return _weight; } }
        /// <summary></summary>
        public double Value { get { return _value; } }
        /// <summary></summary>
        public ItemSlot Slot { get { return _slot; } set { _slot = value; NotifyPropertyChanged(); } }
        /// <summary></summary>
        public int EnhancementBonus { get { return _enhancement; } }
        /// <summary></summary>
        public bool IsWeapon { get { return (_slot == ItemSlot.Mainhand || _slot == ItemSlot.Offhand) || _slot == ItemSlot.TwoHanded; } }
        /// <summary></summary>
        public bool IsArmor { get { return (_slot == ItemSlot.Armor); } }
        /// <summary></summary>
        public int Count { get { return _stackSize; } set { _stackSize = value; NotifyPropertyChanged(); } }
        /// <summary></summary>
        public string Properties { get { return properties; } set { properties = value; NotifyPropertyChanged(); } }
        /// <summary></summary>
        public string Description { get { return CollectDescription(); } }
        /// <summary></summary>
        [DataMember] public string BaseEffect { get; set; }
        /// <summary>Unused</summary>
        [DataMember] public bool IsEquipped 
        {
            get { return _isEquipped; }
            set
            {
                _isEquipped = value;
                NotifyPropertyChanged();
            }
        }
        /// <summary></summary>
        [DataMember] public bool isRanged { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="weight"></param>
        /// <param name="value"></param>
        /// <param name="slot"></param>
        /// <param name="proficient"></param>
        /// <param name="properties"></param>
        /// <param name="bonus"></param>
        public Item(string name, double weight, double value, ItemSlot slot, bool proficient, string properties, int bonus)
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

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected virtual string CollectDescription()
        {
            StringBuilder output = new StringBuilder(_name);
            output.Append(Environment.NewLine);
            output.Append(properties);
            return output.ToString();
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public override string ToString() { return description; }

        #region INotifyPropertyChanged Implementation
        /// <summary>
        /// INotifyPropertyChanged implementation
        /// </summary>
        public event PropertyChangedEventHandler PropertyChanged;
        /// <summary>
        /// INotifyPropertyChanged implementation
        /// </summary>
        /// <param name="propertyName"></param>
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

    }
}
