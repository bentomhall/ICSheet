using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace Interactive_Character_Sheet_Core
{
    public interface IItem
    {
        double Weight { get; }
        double Value { get; }
        string Name { get; }
        ItemSlot Slot { get; }
        string ToString();
        int EnhancementBonus { get; }
        bool IsWeapon { get; }
        bool IsArmor { get; }
        string BaseEffect { get; }

    }

    public enum ItemSlot
    {
        Head,
        Hand,
        Neck,
        Armor,
        Mainhand,
        Offhand,
        TwoHanded,
        Waist,
        Feet,
        None
    }
}
