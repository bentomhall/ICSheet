using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;

namespace Interactive_Character_Sheet_Core
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
            if (isSlotOccupied(item) && EquippedItems[item.Slot].Name == item.Name) { EquippedItems.Remove(item.Slot); } //toggles equipped status
            EquippedItems[item.Slot] = item; //only one item per slot
            var eventArgs = new EquipmentChangedEventArgs();
            eventArgs.Items = EquippedItems.Values.ToList<T>() as List<IItem>;
            OnEquipmentChanged(eventArgs);
        }
        public void AddItem(T newItem)
        {
            _inventory.Add(newItem);
            _currentLoad += newItem.Weight;
        }
        public void RemoveItem(T item)
        {
            _inventory.Remove(item);
            _currentLoad -= item.Weight;
        }
        [DataMember] public int CurrentGold { get; set; }
        public void Earn(int gold)
        {
            CurrentGold += gold;
        }
        public bool Pay(int gold)
        {
            if (CurrentGold < gold)
            {
                return false;
            }
            else
            {
                CurrentGold -= gold;
                return true;
            }
        }
        [DataMember] public Dictionary<ItemSlot, T> EquippedItems = new Dictionary<ItemSlot, T>();

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
