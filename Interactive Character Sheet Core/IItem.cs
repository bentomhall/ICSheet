using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Character_Sheet_Core
{
    public interface IItem
    {
        public int Weight { get; }
        public int Value { get; }
        public string Name { get; }
        public ItemSlot Slot { get; }
        public string ToString();
        public int EnhancementBonus;
        public bool IsWeapon { get; }
        public bool IsArmor { get; }

    }

    public enum ItemSlot
    {
        Head,
        Hand,
        Neck,
        Armor,
        Mainhand,
        Offhand,
        Waist,
        Feet,
        None
    }
}
