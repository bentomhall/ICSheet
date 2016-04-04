using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;
using System.Runtime.CompilerServices;
using System.ComponentModel;

namespace InteractiveCharacterSheetCore
{
    [DataContract]
    public class Inventory<T> where T: IItem
    {
        public Inventory(int strengthMod)
        {
            MaxCarryWeight = 15*strengthMod;
        }

        [DataMember] private double MaxCarryWeight = 50.0;
        [DataMember] private double _currentLoad = 0.0;
        public double CurrentLoad { get { return _currentLoad; } }
        [DataMember] private List<T> _inventory = new List<T>();
        public List<T> InventoryContents { get { return _inventory; } }
        public List<T> FilterContentsBy(Func<T, bool> criteria)
        {
            var items = _inventory.Where(criteria).ToList<T>();
            return items;
        }

        private bool isSlotOccupied(T item)
        {
            return EquippedItems.Keys.Contains(item.Slot);
        }

        public void EquipItem(T item)
        {
            if (!_inventory.Contains(item)) { AddItem(item); }
            if (item.Slot == ItemSlot.None) { return; } //non-slot items cannot be equipped. They should still be added to inventory.
            if (item.Slot == ItemSlot.TwoHanded) { EquippedItems.Remove(ItemSlot.Offhand); }
            if (isSlotOccupied(item) && EquippedItems[item.Slot].Name == item.Name) { EquippedItems.Remove(item.Slot); } //unequip item
            else { EquippedItems[item.Slot] = item; } //replace equipped item
        }

        public void AddItem(T newItem)
        {
            var matchingItem = _inventory.SingleOrDefault(x => x.Name == newItem.Name);
            if ( matchingItem != null)
            {
                matchingItem.Count += 1;
            }
            else { _inventory.Add(newItem); }
            _currentLoad += newItem.Weight;
        }
        public void RemoveItem(T item)
        {
            var matchingItem = _inventory.SingleOrDefault(x => x.Name == item.Name);
            if (matchingItem != null && matchingItem.Count > 1)
            {

                matchingItem.Count -= 1;
            }
            else { _inventory.Remove(item); }
            _currentLoad -= item.Weight;
        }

        public void DropItemStack(T item)
        {
            _inventory.Remove(item);
            _currentLoad -= item.Weight * item.Count;
        }
        [DataMember] public double CurrentGold { get; set; }
        public void Earn(double gold)
        {
            CurrentGold += gold;
        }
        public bool Pay(double gold)
        {
            if (CurrentGold < gold)
            {
                return false;
            }
            else
            {
                CurrentGold += gold; //adding a negative amount
                return true;
            }
        }

        [DataMember] private Dictionary<ItemSlot, T> _equippedItems = new Dictionary<ItemSlot, T>();

        public Dictionary<ItemSlot, T> EquippedItems { get { return _equippedItems; } set { _equippedItems = value; } }

        public bool IsEncumbered()
        {
            return _currentLoad > MaxCarryWeight && _currentLoad < 2* MaxCarryWeight;
        }

        public bool IsHeavyEncumbered()
        {
            return _currentLoad > 2 * MaxCarryWeight;
        }

        public event EventHandler<EquipmentChangedEventArgs> EquipmentChanged;

        protected virtual void OnEquipmentChanged(EquipmentChangedEventArgs e)
        {
            var handler = EquipmentChanged;
            if (handler != null)
            {
                handler(this, e);
            }
        }

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

    public class EquipmentChangedEventArgs: EventArgs
    {
        public List<IItem> Items { get; set; }
    }

    public enum EncumbranceType
    {
        None,
        Light,
        Heavy,
        Immobile
    }
}
