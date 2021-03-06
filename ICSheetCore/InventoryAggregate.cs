﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.Serialization;

namespace ICSheetCore
{

    /// <summary>
    /// Represents a sum of money. Automatically converts types
    /// </summary>
    [DataContract]
    public class Money
    {
        /// <summary>1 PP = 10 GP</summary>
        public int Platinum { get { return (int)(_cash / 10); } }
        /// <summary>Base unit</summary>
        public int Gold { get { return (int)(_cash % 10); } }
        /// <summary>10 CP = 1 SP</summary>
        public int Copper { get { return (int)((_cash * 100) % 10); } }
        /// <summary>10 SP = 1 GP</summary>
        public int Silver { get { return (int)((_cash * 10) % 10); } }
        /// <summary>The total amount (base unit of gp)</summary>
        public decimal Total { get { return _cash; } }
        [DataMember]
        private decimal _cash;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount">must be positive</param>
        internal void Add(decimal amount)
        {
            if (amount < 0) { throw new ArgumentException(); }
            _cash += amount;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="amount">must be positive</param>
        internal void Remove(decimal amount)
        {
            if (amount < 0) { throw new ArgumentException(); }
            if (_cash < amount) { throw new OutOfMoneyExeption(); }
            _cash -= amount;
        }
    }

    /// <summary>
    /// Thrown when the total remaining is less than the requested removal amount. 
    /// </summary>
    public class OutOfMoneyExeption : Exception
    {
        /// <summary></summary>
        public OutOfMoneyExeption()
        {
        }

        /// <summary></summary>
        public OutOfMoneyExeption(string message) : base(message)
        {
        }

        /// <summary></summary>
        public OutOfMoneyExeption(string message, Exception innerException) : base(message, innerException)
        {
        }
    }

    internal class InventoryAggregate
    {
        internal InventoryAggregate()
        {
            _equippedItems[ItemSlot.Mainhand] = _unarmedStrike;
            _equippedItems[ItemSlot.Armor] = _clothing;
        }

        internal IEnumerable<IItem> ContentsMatching(Func<IItem, bool> criterion)
        {
            return _inventoryItems.Where(criterion);
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="slot">The itemslot needed</param>
        /// <returns>The equipped item or null if nothing is equipped.</returns>
        internal IItem ItemEquippedIn(ItemSlot slot)
        {
            IItem e;
            _equippedItems.TryGetValue(slot, out e);
            return e;
        }

        internal void EquipItem(IItem item)
        {
            if (_inventoryItems.Count(x => x.Name == item.Name) == 0) { AddItem(item); }
            if (item.Slot == ItemSlot.None) { return; }
            if (item.Slot == ItemSlot.TwoHanded)
            {
                _equippedItems.Remove(ItemSlot.Offhand);
                _equippedItems[ItemSlot.Mainhand] = item;

            }
            else { _equippedItems[item.Slot] = item; }

        }

        internal void UnequipItem(IItem item)
        {
            if (item.Slot == ItemSlot.None) { return; }
            if (item.Slot == ItemSlot.Mainhand || item.Slot == ItemSlot.TwoHanded)
            {
                _equippedItems[ItemSlot.Mainhand] = _unarmedStrike;
            }
            else if (item.Slot == ItemSlot.Armor)
            {
                _equippedItems[ItemSlot.Armor] = _clothing;
            }
            else
            {
                _equippedItems.Remove(item.Slot);
            }
        }

        internal void AddItem(IItem item)
        {
            if (item == null) { throw new ArgumentNullException(); }
            var currentItem = _inventoryItems.SingleOrDefault(x => x.Name == item.Name);
            if (currentItem != null) { currentItem.Count += 1; }
            else { _inventoryItems.Add(item); }
        }

        internal void RemoveItem(IItem item)
        {
            if (item == null) { throw new ArgumentNullException(); }
            var currentItem = _inventoryItems.SingleOrDefault(x => x.Name == item.Name);
            if (currentItem != null && currentItem.Count > 1) { currentItem.Count -= 1; }
            else { _inventoryItems.Remove(item); }
        }

        internal void DropItemStack(IItem item)
        {
            _inventoryItems.Remove(item);

            if (!_equippedItems.ContainsKey(item.Slot)) { return; }
            else if (_equippedItems[item.Slot].Name == item.Name) { UnequipItem(item); }
        }

        internal Money CashOnHand
        {
            get { return _cash; }
        }

        internal void MoneyTransaction(decimal amount)
        {
            if (amount > 0) { _cash.Add(amount); }
            else { _cash.Remove(Math.Abs(amount)); }
        }

        private List<IItem> _inventoryItems = new List<IItem>();
        private Dictionary<ItemSlot, IItem> _equippedItems = new Dictionary<ItemSlot, IItem>();
        private WeaponItem _unarmedStrike = new WeaponItem("Unarmed Strike", 0, 0, ItemSlot.Mainhand, false, "", WeaponCategory.SimpleMelee, 0);
        private ArmorItem _clothing = new ArmorItem("Clothing", 0, 0, true, "", ArmorType.None, 0);
        private Money _cash = new Money();

        internal IDictionary<ItemSlot, IItem> GetEquippedItems()
        {
            return _equippedItems;
        }

        internal double InventoryWeight
        {
            get
            {
                return _inventoryItems.Sum(x => x.Weight * x.Count);
            }
        }
    }
}
