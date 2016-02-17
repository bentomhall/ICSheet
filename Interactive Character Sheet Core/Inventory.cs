using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Character_Sheet_Core
{
    public class Inventory<T> where T: IItem
    {
        public Inventory(int strengthMod)
        {
            MaxCarryWeight = 15*strengthMod;
        }

        private int MaxCarryWeight = 50;
        private int _currentLoad = 0;
        public int CurrentLoad { get { return _currentLoad; } }
        private List<T> _inventory = new List<T>();
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
        public int CurrentGold { get; set; }
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
        public Dictionary<ItemSlot, T> EquippedItems = new Dictionary<ItemSlot, T>();

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
