namespace InteractiveCharacterSheetCore
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
        int Count { get; set; }

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
