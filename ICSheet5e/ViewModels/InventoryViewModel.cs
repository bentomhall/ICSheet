﻿using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using ICSheetCore;

namespace ICSheet5e.ViewModels
{

    public class InventoryViewModel : BaseViewModel
    {
        public InventoryViewModel(PlayerCharacter c, ApplicationModel parent, ItemDataBase itemDB)
        {
            Parent = parent;
            currentCharacter = c;
            NewItemModel = new NewItemFactory(itemDB.Weapons, itemDB.Armors, itemDB.Items);
            NewItemModel.delegateAddItem = AddItemToInventory;
            setEquippedItems();
        }

        public bool isEncumbered
        {
            get { return (currentCharacter.CarriedWeight + 0.01 * (double)currentCharacter.Cash.Total) > 5.0 * currentCharacter.AbilityScoreFor(AbilityType.Strength); }
        }

        private string ItemNameOrDefault(ItemSlot slot)
        {
            var item = currentCharacter.EquippedItemForSlot(slot);
            if (item == null) { return "Nothing Equipped"; }
            else { return item.Name; }
        }

        private void setEquippedItems()
        {
            handItemName = ItemNameOrDefault(ItemSlot.Hand);
            headItemName = ItemNameOrDefault(ItemSlot.Head);
            neckItemName = ItemNameOrDefault(ItemSlot.Neck);
            armorItemName = ItemNameOrDefault(ItemSlot.Armor);
            var mainWeapon = currentCharacter.EquippedItemForSlot(ItemSlot.Mainhand);
            if (mainWeapon.Slot == ItemSlot.Mainhand)
            {
                mainWeaponName = ItemNameOrDefault(ItemSlot.Mainhand);
                OffhandWeaponName = ItemNameOrDefault(ItemSlot.Offhand);
            }
            else
            {
                mainWeaponName = mainWeapon.Name;
                OffhandWeaponName = "N/A";
            }
            waistItemName = ItemNameOrDefault(ItemSlot.Waist);
            feetItemName = ItemNameOrDefault(ItemSlot.Feet);
        }

        private PlayerCharacter currentCharacter;
        private ObservableCollection<Item> _allItems = new ObservableCollection<Item>();
        private string headItemName;
        private string handItemName;
        private string neckItemName;
        private string armorItemName;
        private string mainWeaponName;
        private string offhandWeaponName;
        private string waistItemName;
        private string feetItemName;

        public NewItemFactory NewItemModel
        {
            get;
            private set;
        }

        private bool _showOverlay;
        public bool ShowAddItemOverlay
        {
            get { return _showOverlay; }
            set
            {
                _showOverlay = value;
                NotifyPropertyChanged();
            }
        }

        public Money Gold { get { return currentCharacter.Cash; } }

        public InventoryItemViewModel SelectedItem
        {
            get { return _selectedItem; }
            set
            {
                _selectedItem = value;
                NotifyPropertyChanged();
            }
        }

        private InventoryItemViewModel _selectedItem;

        public ObservableCollection<InventoryItemViewModel> Items 
        {
            get
            {
                var items = new ObservableCollection<InventoryItemViewModel>();
                foreach (var item in currentCharacter.ItemsMatching(x => x.Name != "Unarmed Strike" && x.Name != "Clothing"))
                {
                    items.Add(modelForItem(item));
                }
                return items;
            }
        }

        public string HeadItemName { get { return headItemName; } set { headItemName = value; NotifyPropertyChanged(); } }
        public string HandItemName { get { return handItemName; } set { handItemName = value; NotifyPropertyChanged(); } }
        public string NeckItemName { get { return neckItemName; } set { neckItemName = value; NotifyPropertyChanged(); } }
        public string ArmorItemName { get { return armorItemName; } set { armorItemName = value; NotifyPropertyChanged(); } }
        public string MainWeaponName { get { return mainWeaponName; } set { mainWeaponName = value; NotifyPropertyChanged(); } }
        public string OffhandWeaponName { get { return offhandWeaponName; } set { offhandWeaponName = value; NotifyPropertyChanged(); } }
        public string WaistItemName { get { return waistItemName; } set { waistItemName = value; NotifyPropertyChanged(); } }
        public string FeetItemName { get { return feetItemName; } set { feetItemName = value; NotifyPropertyChanged(); } }

        static public Array SlotNames
        {
            get { return Enum.GetValues(typeof(ItemSlot)); }
        }

        public void AddItemToInventory(IItem item)
        {
            if (ShowAddItemOverlay)
            {
                NewItemModel.Clear();
                ShowAddItemOverlay = false;
            }
            currentCharacter.AddItemToInventory(item);
            NotifyPropertyChanged("Items");
        }

        private Dictionary<ItemSlot, int> EquipmentSlotMap = new Dictionary<ItemSlot, int>()
        {
            { ItemSlot.Head, 0},
            { ItemSlot.Neck, 1},
            { ItemSlot.Armor, 2},
            { ItemSlot.Hand, 3},
            { ItemSlot.Waist, 4},
            { ItemSlot.Feet, 5},
            { ItemSlot.Mainhand, 6},
            { ItemSlot.Offhand, 7},
            { ItemSlot.TwoHanded, 6} //2H weapons get equipped to main hand
        };

        public bool IsEquipped(IItem item)
        {
            IItem current;
            if (item.Slot == ItemSlot.TwoHanded) { current = currentCharacter.EquippedItemForSlot(ItemSlot.Mainhand); }
            else { current = currentCharacter.EquippedItemForSlot(item.Slot); }
            if (current == null) { return false; }
            else { return current.Name == item.Name; }
        }

        public void ChangeEquipmentStatus(IItem item)
        {
            if (IsEquipped(item)) { currentCharacter.Unequip(item); }
            else { currentCharacter.Equip(item); }
            updateSlotListing(item.Slot);
            NotifyPropertyChanged("EquippedItems");
            NotifyPropertyChanged("Items");
        }

        private void updateSlotListing(ItemSlot slot)
        {
            IItem item;
            if (slot == ItemSlot.TwoHanded) { item = currentCharacter.EquippedItemForSlot(ItemSlot.Mainhand); }
            else { item = currentCharacter.EquippedItemForSlot(slot); }
            string name;
            if (item == null) { name = "No Item Equipped"; }
            else { name = item.Name; } 
            switch (slot)
            {
                case ItemSlot.Head:
                    HeadItemName = name;
                    break;
                case ItemSlot.Hand:
                    HandItemName = name;
                    break;
                case ItemSlot.Neck:
                    NeckItemName = name;
                    break;
                case ItemSlot.Armor:
                    ArmorItemName = name;
                    break;
                case ItemSlot.Feet:
                    FeetItemName = name;
                    break;
                case ItemSlot.Waist:
                    WaistItemName = name;
                    break;
                case ItemSlot.Mainhand:
                    MainWeaponName = name;
                    break;
                case ItemSlot.Offhand:
                    OffhandWeaponName = name;
                    break;
                case ItemSlot.TwoHanded:
                    MainWeaponName = name;
                    OffhandWeaponName = "N/A";
                    break;
            }
        }

        private InventoryItemViewModel modelForItem(IItem item)
        {
            return new InventoryItemViewModel(item, ChangeEquipmentStatus, IsEquipped);
        }

        decimal transactionAmount;
        public decimal TransactionAmount
        {
            get { return transactionAmount; }
            set
            {
                transactionAmount = value;
                NotifyPropertyChanged();
            }
        }
        public ICommand GoldTransactionCommand
        {
            get { return new Views.DelegateCommand<string>(GoldTransactionExecuted); }
        }

        private void GoldTransactionExecuted(string obj)
        {
            if (obj == "Pay")
            {
                currentCharacter.DoGoldTransaction(-1 * Math.Abs(transactionAmount));
            }
            else if (obj == "Gain")
            {
                currentCharacter.DoGoldTransaction(Math.Abs(transactionAmount));
            }
            NotifyPropertyChanged("Gold");
        }

        public ICommand ModifyItemCountCommand
        {
            get { return new Views.DelegateCommand<string>(ModifyItemCountExecuted); }
        }

        public ICommand ShouldShowItemOverlay
        {
            get { return new Views.DelegateCommand<object>(x => ShowAddItemOverlay = true); }
        }

        private void ModifyItemCountExecuted(string obj)
        {
            if (obj == "+") 
            { 
                currentCharacter.AddItemToInventory(SelectedItem.Item);
            }
            else if (obj == "-") 
            { 
                currentCharacter.DropItem(SelectedItem.Item, false);
            }
            else
            {
                currentCharacter.DropItem(SelectedItem.Item, true);
            }
            NotifyPropertyChanged("Items");
        }

    }

    public class NewItemFactory : BaseViewModel
    {
        string _name;
        string properties;
        int enhancement;
        IEnumerable<IItem> baseItems;
        double weight;
        double gpValue;
        IItem selectedItem;
        string slotName;
        ItemSlot slot;
        IEnumerable<WeaponItem> weapons;
        IEnumerable<ArmorItem> armors;
        IEnumerable<Item> items;
        int count;
        bool displayEquippableOnly;

        public bool DisplayEquippableOnly
        {
            get { return displayEquippableOnly; }
            set
            {
                displayEquippableOnly = value;
                NotifyPropertyChanged();
            }
        }
        
        public int Count
        {
            get { return count; }
            set { count = value; NotifyPropertyChanged(); }
        }

        IItem ItemWithName(string name)
        {
            if (name == "N/A") { return null; } //no base item
            var w = weapons.SingleOrDefault(x => x.Name == name) as IItem;
            var a = armors.SingleOrDefault(x => x.Name == name) as IItem;
            return w ?? a ; //returns null if both are null
        }

        ArmorItem ArmorWithName(string name)
        {
            return armors.SingleOrDefault(x => x.Name == name);
        }

        WeaponItem WeaponWithName(string name)
        {
            return weapons.SingleOrDefault(x => x.Name == name);
        }

        void SetValuesFromItem(IItem item)
        {
            //PropertyChanged -= OnBaseItemChanged; //prevent handler from firing during this function
            if (item == null)
            {
                Slot = ItemSlot.None;
                Name = "";
                Properties = "";
                Value = 0.0;
                Weight = 0.0;
                Count = 1;
                selectedItem = null;
            }
            else
            {
                Slot = item.Slot;
                Name = item.Name;
                Properties = item.Properties;
                Value = item.Value;
                Weight = item.Weight;
                Count = item.Count;
                //SlotName = item.Slot.ToString();
            }
            //PropertyChanged += OnBaseItemChanged;
            return;
        }

        public NewItemFactory(IEnumerable<WeaponItem> weapons, IEnumerable<ArmorItem> armors, IEnumerable<Item> items)
        {

            baseItems = new ObservableCollection<IItem>();
            //baseItems.Add("N/A");
            this.armors = armors;
            this.weapons = weapons;
            this.items = items;
            PropertyChanged += OnBaseItemChanged;
            SetValuesFromItem(null); //initialize all items
        }



        public Action<Item> delegateAddItem { get; set; }

        public void OnBaseItemChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItem")
            {
                SetValuesFromItem(SelectedItem);
            }
        }

        public string SlotName
        {
            get { return slotName; }
            set
            {
                slotName = value;
                slot = (ItemSlot)Enum.Parse(typeof(ItemSlot), value);
                NotifyPropertyChanged();
            }
        }

        public ItemSlot Slot 
        { 
            get { return slot; } 
            set 
            { 
                slot = value; 
                NotifyPropertyChanged();
                SlotName = Enum.GetName(typeof(ItemSlot), value);
            } 
        }

        public IEnumerable<IItem> BaseItems
        {
            get
            {
                switch(ItemType)
                {
                    case "Item":
                        return items;
                    case "Weapon":
                        return weapons;
                    case "Armor":
                        return armors;
                    default:
                        return new List<IItem>();
                }
            }
        }

        private List<string> _itemTypes = new List<string>() { "Item", "Weapon", "Armor" };
        public IEnumerable<string> ItemTypes { get { return _itemTypes; } }

        private string _itemType = "Item";
        public string ItemType
        {
            get { return _itemType; }
            set
            {
                _itemType = value;
                NotifyPropertyChanged();
                NotifyPropertyChanged("BaseItems");
            }
        }


        public string Name
        {
            get { return _name; }
            set
            {
                _name = value;
                NotifyPropertyChanged();
                CommandManager.InvalidateRequerySuggested();
            }
        }

        public string Properties
        {
            get { return properties; }
            set
            {
                properties = value;
                NotifyPropertyChanged();
            }
        }

        public int Enhancement { get { return enhancement; } set { enhancement = value; NotifyPropertyChanged(); } }
        public double Weight { get { return weight; } set { weight = value; NotifyPropertyChanged(); } }
        public double Value { get { return gpValue; } set { gpValue = value; NotifyPropertyChanged(); } }


        public IItem SelectedItem { get { return selectedItem; } set { selectedItem = value; NotifyPropertyChanged(); SetValuesFromItem(value); } }


        public ICommand CreateNewItemCommand
        {
            get { return new Views.DelegateCommand<object>(CreateItemCommandExecuted);}
        }

        private void CreateItemCommandExecuted(object obj)
        {
            if (string.IsNullOrEmpty(_name)) { return; }


            
            if (Slot == ItemSlot.Armor || (_name.Contains("Shield") && Slot == ItemSlot.Offhand))
            {
                var baseItem = selectedItem as ArmorItem;
                var armor = new ArmorItem(Name, Weight, Value, true, Properties, baseItem.ArmorClassType, Enhancement);
                armor.Slot = slot;
                armor.BaseEffect = baseItem.BaseEffect;
                armor.Count = Count;
                delegateAddItem(armor);
            }
            else if (Slot == ItemSlot.Mainhand || Slot == ItemSlot.Offhand || Slot == ItemSlot.TwoHanded)
            {
                var baseItem = selectedItem as WeaponItem;
                var weapon = new WeaponItem(Name, Weight, Value, Slot, true, Properties, baseItem.Category, Enhancement);
                weapon.BaseEffect = baseItem.BaseEffect;
                weapon.Count = Count;
                weapon.Damage = baseItem.Damage;
                delegateAddItem(weapon);
            }
            else
            {
                var newItem = new Item(Name, Weight, Value, Slot, false, Properties, Enhancement);
                newItem.Count = Count;
                delegateAddItem(newItem);
            }
            
        }

        public void Clear()
        {
            SetValuesFromItem(null);
        }

        public ICommand ClearItemCommand
        {
            get { return new Views.DelegateCommand<object>(x => this.SetValuesFromItem(null)); }
        }
    }
}

