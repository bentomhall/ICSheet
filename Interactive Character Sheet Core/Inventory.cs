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

        [DataMember] private int MaxCarryWeight = 50;
        [DataMember] private int _currentLoad = 0;
        public int CurrentLoad { get { return _currentLoad; } }
        [DataMember] private List<T> _inventory = new List<T>();
        public List<T> InventoryContents { get { return _inventory; } }
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
    }

    public enum EncumbranceType
    {
        None,
        Light,
        Heavy,
        Immobile
    }
}
