using System;
using System.Collections.Generic;

namespace ICSheetCore
{
    interface IInventoryDelegate
    {
        void AddItemToInventory(IItem item);
        void DropItem(IItem item);
        void Equip(IItem item);
        void Unequip(IItem item);
        void DoGoldTransaction(decimal amount);
    }

    interface IInventoryDataSource
    {
        IItem EquippedItemForSlot(ItemSlot slot);
        IEnumerable<IItem> ItemsMatching(Func<IItem, bool> predicate);
        Money Cash { get; }
    }
}
