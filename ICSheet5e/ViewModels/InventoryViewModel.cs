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
            NewItemModel = new NewItemFactory(itemDB.Weapons, itemDB.Armors);
            NewItemModel.delegateAddItem = AddItemToInventory;
            setEquippedItems();
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

        public Money Gold { get { return currentCharacter.Cash; } }

        //public ObservableCollection<IItem> EquippedItems
        //{
        //    get { return new ObservableCollection<IItem>(currentCharacter.E.OrderBy(x => x.Slot)); }
        //}

        public IItem SelectedItem { get; set; }

        public ObservableCollection<IItem> Items 
        {
            get { return new ObservableCollection<IItem>(currentCharacter.ItemsMatching(x => true)); }
        }

        public string HeadItemName { get { return headItemName; } set { headItemName = value; NotifyPropertyChanged(); } }
        public string HandItemName { get { return handItemName; } set { handItemName = value; NotifyPropertyChanged(); } }
        public string NeckItemName { get { return neckItemName; } set { neckItemName = value; NotifyPropertyChanged(); } }
        public string ArmorItemName { get { return armorItemName; } set { armorItemName = value; NotifyPropertyChanged(); } }
        public string MainWeaponName { get { return mainWeaponName; } set { mainWeaponName = value; NotifyPropertyChanged(); } }
        public string OffhandWeaponName { get { return offhandWeaponName; } set { offhandWeaponName = value; NotifyPropertyChanged(); } }
        public string WaistItemName { get { return waistItemName; } set { waistItemName = value; NotifyPropertyChanged(); } }
        public string FeetItemName { get { return feetItemName; } set { feetItemName = value; NotifyPropertyChanged(); } }

        public Array SlotNames
        {
            get { return Enum.GetValues(typeof(ItemSlot)); }
        }

        public void AddItemToInventory(IItem item)
        {
            currentCharacter.AddItemToInventory(item);
            NotifyPropertyChanged("Items");
        }

        public ICommand EquipItemCommand
        {
            get { return new Views.DelegateCommand<int>(EquipItemCommandExecuted); }
        }

        private void EquipItemCommandExecuted(int index)
        {
            currentCharacter.Equip(SelectedItem);
            var name = ItemNameOrDefault(SelectedItem.Slot);
            switch (SelectedItem.Slot)
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
            NotifyPropertyChanged("EquippedItems");
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
                currentCharacter.DoGoldTransaction(-1 * transactionAmount);
            }
            else if (obj == "Gain")
            {
                currentCharacter.DoGoldTransaction(transactionAmount);
            }
            NotifyPropertyChanged("Gold");
        }

        public ICommand ModifyItemCountCommand
        {
            get { return new Views.DelegateCommand<string>(ModifyItemCountExecuted); }
        }

        private void ModifyItemCountExecuted(string obj)
        {
            if (obj == "+") 
            { 
                currentCharacter.AddItemToInventory(SelectedItem);
            }
            else if (obj == "-") 
            { 
                currentCharacter.DropItem(SelectedItem);
            }
            else
            {
                currentCharacter.DropItem(SelectedItem);
            }
            NotifyPropertyChanged("Items");
        }

    }

    public class NewItemFactory : BaseViewModel
    {
        string name;
        string properties;
        int enhancement;
        ObservableCollection<string> baseItems;
        double weight;
        double gpValue;
        string selectedItem;
        string slotName;
        ItemSlot slot;
        List<WeaponItem> weapons;
        List<ArmorItem> armors;
        int count;
        
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
            PropertyChanged -= OnBaseItemChanged; //prevent handler from firing during this function
            if (item == null)
            {
                Slot = ItemSlot.None;
                Name = "";
                Properties = "";
                Value = 0.0;
                Weight = 0.0;
                Count = 1;
            }
            else
            {
                Slot = item.Slot;
                Name = item.Name;
                Properties = item.Properties;
                Value = item.Value;
                Weight = item.Weight;
                Count = item.Count;
                SlotName = item.Slot.ToString();
            }
            PropertyChanged += OnBaseItemChanged;
            return;
        }

        public NewItemFactory(IEnumerable<WeaponItem> weapons, IEnumerable<ArmorItem> armors)
        {

            baseItems = new ObservableCollection<string>();
            baseItems.Add("N/A");
            foreach (var w in weapons)
            {
                baseItems.Add(w.Name);
            }
            foreach (var a in armors)
            {
                baseItems.Add(a.Name);
            }
            this.armors = armors.ToList();
            this.weapons = weapons.ToList();
            this.PropertyChanged += OnBaseItemChanged;
            SetValuesFromItem(null); //initialize all items
        }



        public Action<Item> delegateAddItem { get; set; }

        public void OnBaseItemChanged(object sender, System.ComponentModel.PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedItemName")
            {
                SetValuesFromItem(ItemWithName(SelectedItemName));
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

        public ObservableCollection<string> BaseItems
        {
            get { return baseItems; }
            set { baseItems = value; }
        }


        public string Name
        {
            get { return name; }
            set
            {
                name = value;
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


        public string SelectedItemName { get { return selectedItem; } set { selectedItem = value; NotifyPropertyChanged(); SetValuesFromItem(ItemWithName(value)); } }


        public ICommand CreateNewItemCommand
        {
            get { return new Views.DelegateCommand<object>(CreateItemCommandExecuted);}
        }

        private void CreateItemCommandExecuted(object obj)
        {
            if (string.IsNullOrEmpty(name)) { return; }


            
            if (Slot == ItemSlot.Armor || (name.Contains("Shield") && Slot == ItemSlot.Offhand))
            {
                var baseItem = ArmorWithName(SelectedItemName);
                var armor = new ArmorItem(Name, Weight, Value, true, Properties, baseItem.ArmorClassType, Enhancement);
                armor.Slot = slot;
                armor.BaseEffect = baseItem.BaseEffect;
                armor.Count = Count;
                delegateAddItem(armor);
            }
            else if (Slot == ItemSlot.Mainhand || Slot == ItemSlot.Offhand || Slot == ItemSlot.TwoHanded)
            {
                var baseItem = WeaponWithName(SelectedItemName);
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

        public ICommand ClearItemCommand
        {
            get { return new Views.DelegateCommand<object>(x => this.SetValuesFromItem(null)); }
        }
    }
}

