using ICSheetCore;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Input;

namespace ICSheet5e.ViewModels
{
    public class InventoryItemViewModel : BaseViewModel
    {
        private IItem _item;
        private Action<IItem> _changeEquipmentCommand;
        private Func<IItem, bool> _isEquippedCommand;
        public InventoryItemViewModel(IItem item, Action<IItem> delegateCommand, Func<IItem, bool> isEquippedCommand)
        {
            _item = item;
            _changeEquipmentCommand = delegateCommand;
            _isEquippedCommand = isEquippedCommand;
        }

        public ICommand ToggleEquippedStatusCommand
        {
            get { return new Views.DelegateCommand<object>(ToggleEquippedStatusCommandExecuted); }
        }

        public string Name
        {
            get { return _item.Name; }
        }

        public int Count
        {
            get { return _item.Count; }
        }

        public string Slot
        {
            get { return _item.Slot.ToString(); }
        }

        public IItem Item { get { return _item; } }

        public string IsEquipped
        {
            get
            {
                if (_item.Slot == ItemSlot.None) { return ""; } //unequippable items should not be flagged
                if (_isEquippedCommand(_item)) { return "E"; }
                else { return "U"; }
            }
        }

        public double Weight { get { return _item.Weight; } }
        public double Value { get { return _item.Value; } }
        public int Enhancement { get { return _item.EnhancementBonus; } }
        public string BaseEffect { get { return _item.BaseEffect; } }
        public string TypeLabel
        {
            get
            {
                if (_item.IsArmor) { return "Armor Type"; }
                else if (_item.IsWeapon) { return "Weapon Type"; }
                else { return ""; }
            }
        }

        public string TypeIdentifier
        {
            get
            {
                if (_item.IsArmor) { return (_item as ArmorItem).ArmorClassType.ToString(); }
                else if (_item.IsWeapon) { return (_item as WeaponItem).Category.ToString(); }
                else { return ""; }
            }
        }
        public string Properties { get { return _item.Properties; } }

        private void ToggleEquippedStatusCommandExecuted(object obj)
        {
            _changeEquipmentCommand(_item);
        }
    }
}
