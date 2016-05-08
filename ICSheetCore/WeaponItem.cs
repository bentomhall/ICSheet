using System.Text;
using System.Runtime.Serialization;

namespace ICSheetCore
{
    /// <summary>
    /// An item that can be equipped in mainhand, offhand, or as a two-handed weapon. Can attack.
    /// </summary>
    [DataContract]
    public class WeaponItem : Item
    {
        /// <summary>
        /// 
        /// </summary>
        [DataMember]
        public WeaponCategory Category { get; set; }
        /// <summary>
        /// 
        /// </summary>
        public string Damage { get; set; }
        /// <summary>
        /// 
        /// </summary>
        /// <param name="name"></param>
        /// <param name="weight"></param>
        /// <param name="value"></param>
        /// <param name="slot"></param>
        /// <param name="proficient"></param>
        /// <param name="properties"></param>
        /// <param name="type"></param>
        /// <param name="bonus"></param>
        public WeaponItem(string name, double weight, double value, ItemSlot slot, bool proficient, string properties, WeaponCategory type, int bonus): base(name, weight, value, slot, proficient, properties, bonus)
        {
            Category = type;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        protected override string CollectDescription()
        {
            var builder = new StringBuilder(Name);
            builder.AppendLine();
            builder.AppendLine(Category.ToString());
            builder.AppendLine(Damage);
            builder.AppendLine(Properties);
            return builder.ToString();
        }
    }

    /// <summary>
    /// 
    /// </summary>
    public enum WeaponCategory
    {
        /// <summary>
        /// 
        /// </summary>
        SimpleMelee,
        /// <summary>
        /// 
        /// </summary>
        SimpleRanged,
        /// <summary>
        /// 
        /// </summary>
        MartialMelee,
        /// <summary>
        /// 
        /// </summary>
        MartialRanged
    }
}
