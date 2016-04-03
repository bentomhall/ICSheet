using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using InteractiveCharacterSheetCore;

namespace ICSheet5e.Model
{
    [DataContract]
    public class WeaponItem : Item
    {
        [DataMember]
        public WeaponCategory Category { get; set; }
        public DamageType Damage { get; set; }
        public WeaponItem(string name, double weight, double value, ItemSlot slot, bool proficient, string properties, WeaponCategory type, int bonus): base(name, weight, value, slot, proficient, properties, bonus)
        {
            Category = type;
        }

        protected override string CollectDescription()
        {
            var builder = new StringBuilder(Name);
            builder.AppendLine();
            builder.AppendLine(Category.ToString());
            builder.AppendLine(Damage.ToString());
            builder.AppendLine(Properties);
            return builder.ToString();
        }
    }

    public enum WeaponCategory
    {
        SimpleMelee,
        SimpleRanged,
        MartialMelee,
        MartialRanged
    }
}
