namespace ICSheetCore
{
    /// <summary>An inventory item</summary>
    public interface IItem
    {
        /// <summary></summary>
        double Weight { get; }
        /// <summary>In gp.</summary>
        double Value { get; }
        /// <summary></summary>
        string Name { get; }
        /// <summary>Which slot to equip the item in (or None)</summary>
        ItemSlot Slot { get; }
        /// <summary></summary>
        string ToString();
        /// <summary></summary>
        int EnhancementBonus { get; }
        /// <summary></summary>
        bool IsWeapon { get; }
        /// <summary></summary>
        bool IsArmor { get; }
        /// <summary></summary>
        string BaseEffect { get; }
        /// <summary>Stack size.</summary>
        int Count { get; set; }
        /// <summary>Additional properties. As text.</summary>
        string Properties { get; set; }

    }

    /// <summary></summary>
    public enum ItemSlot
    {
        /// <summary></summary>
        Head,
        /// <summary></summary>
        Hand,
        /// <summary></summary>
        Neck,
        /// <summary></summary>
        Armor,
        /// <summary></summary>
        Mainhand,
        /// <summary></summary>
        Offhand,
        /// <summary></summary>
        TwoHanded,
        /// <summary>Replaces both Mainhand and Offhand items.</summary>
        Waist,
        /// <summary></summary>
        Feet,
        /// <summary></summary>
        None
    }
}
