using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;
using System.Runtime.CompilerServices;
using ICSheet5e.Model;
using Interactive_Character_Sheet_Core;

namespace ICSheet5e.ViewModels
{
    public class InventoryViewModel : BaseViewModel
    {
        public InventoryViewModel(Character c, ApplicationModel parent)
        {
            Parent = parent;
            currentCharacter = c;
        }

        private Character currentCharacter;
        private ObservableCollection<string> _equippedItems = new ObservableCollection<string>() {"", "", "", "", "", "" ,"", ""};
        private ObservableCollection<Item> _allItems = new ObservableCollection<Item>();
        private ObservableCollection<Item> _headSlotItems = new ObservableCollection<Item>();
        public ObservableCollection<string> EquippedItems
        {
            get { return _equippedItems; }
            set
            {
                _equippedItems = value;
                NotifyPropertyChanged("EquippedItems");
            }
        }
        public ObservableCollection<Item> Items
        {
            get { return _allItems; }
        }

        public ObservableCollection<Item> HeadSlotItems
        {
            get { return _headSlotItems; }
        }

        public ICommand AddItemCommand
        {
            get { return new Views.DelegateCommand<object>(AddItemCommandExecuted, AddItemCommandCanExecute); }
        }

        private void AddItemCommandExecuted(object obj)
        {
            var item = new Item("Diadem of Power", 0.1, 1000.0, Interactive_Character_Sheet_Core.ItemSlot.Head, false, "Gives Ultimate Power");
            currentCharacter.AddItemToInventory(item);
            _allItems = new ObservableCollection<Item>(currentCharacter.AllCarriedItems);
            _headSlotItems = new ObservableCollection<Item>(currentCharacter.ItemsMatching(x => x.Slot == Interactive_Character_Sheet_Core.ItemSlot.Head));
            NotifyPropertyChanged("Items");
            NotifyPropertyChanged("HeadSlotItems");
        }

        private bool AddItemCommandCanExecute(object obj)
        {
            return true;
        }

        public ICommand EquipItemCommand
        {
            get { return new Views.DelegateCommand<Item>(EquipItemCommandExecuted); }
        }

        private void EquipItemCommandExecuted(Item item)
        {
            if (item == null) { return; }
            if (item.IsEquipped) { _equippedItems[EquipmentSlotMap[item.Slot]] = ""; } //de-equip
            else { _equippedItems[EquipmentSlotMap[item.Slot]] = item.Name; } //equip
            currentCharacter.Equip(item);
            NotifyPropertyChanged("EquippedItems");
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
            { ItemSlot.Offhand, 7}
        };
    }
}
