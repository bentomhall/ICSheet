using System.Text;
using System.Runtime.Serialization;

namespace ICSheetCore
{
    /// <summary>
    /// Represents a piece of armor (something that contributes to AC directly).
    /// </summary>
    [DataContract]
    public class ArmorItem : Item
    {
        /// <summary>
        /// The maximum dexterity bonus allowed by that item. Varies mostly by type of armor.
        /// </summary>
        [DataMember]
        public int MaxDexBonus { get; set; }
        /// <summary>
        /// The type of armor (heavy, medium, or light).
        /// </summary>
        [DataMember]
        public ArmorType ArmorClassType { get; set; }
        /// <summary>
        /// The base AC of the item (without enhancements)
        /// </summary>
        public int ArmorBonus
        {
            get { return int.Parse(BaseEffect); }
        }
        /// <summary>
        /// Constructor. 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="weight"></param>
        /// <param name="value"></param>
        /// <param name="proficient"></param>
        /// <param name="properties"></param>
        /// <param name="type"></param>
        /// <param name="bonus"></param>
        public ArmorItem(string name, double weight, double value, bool proficient, string properties, ArmorType type, int bonus): base(name, weight, value, ItemSlot.Armor, proficient, properties, bonus)
        {
            ArmorClassType = type;
            switch(type)
            {
                case ArmorType.None:
                    MaxDexBonus = 99;
                    BaseEffect = "0";
                    break;
                case ArmorType.Light:
                    MaxDexBonus = 99;
                    BaseEffect = "11"; //specific items should override this, this is the minimum
                    break;
                case ArmorType.Medium:
                    MaxDexBonus = 2;
                    BaseEffect = "12";
                    break;
                case ArmorType.Heavy:
                    MaxDexBonus = 0;
                    BaseEffect = "14";
                    break;
                case ArmorType.Shield:
                    BaseEffect = "2";
                    Slot = ItemSlot.Offhand;
                    break;
                default:
                    break;
            }
        }
        /// <summary>
        /// Constructs the description of the item.
        /// </summary>
        /// <returns></returns>
        protected override string CollectDescription()
        {
            var builder = new StringBuilder(Name);
            builder.AppendLine();
            builder.AppendLine(ArmorClassType.ToString());
            builder.AppendLine($"AC: {BaseEffect}, EnhancementBonus: {EnhancementBonus}");
            builder.Append(Properties);
            return builder.ToString();
        }
    }

    /// <summary>
    /// The type of armor represented by the item.
    /// </summary>
    public enum ArmorType
    {
        /// <summary>No armor: AC = 10 + DEX</summary>
        None,
        /// <summary>Light Armor: AC = Armor AC + DEX</summary>
        Light,
        /// <summary>Medium Armor: AC = Armor AC + DEX (max 2)</summary>
        Medium,
        /// <summary>Heavy Armor: AC = Armor AC</summary>
        Heavy,
        /// <summary>+2 AC (stacks)</summary>
        Shield
    }
}
