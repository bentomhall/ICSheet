using System.Collections.Generic;
using System.Runtime.Serialization;
using System.Linq;

namespace ICSheetCore
{
    /// <summary>
    /// Data source for all items defined in XML.
    /// </summary>
    [DataContract]
    public class ItemDataBase
    {
        [DataMember]
        private List<ArmorItem> _armors;
        [DataMember]
        private List<Item> _items;
        [DataMember]
        private List<WeaponItem> _weapons;

        /// <summary>
        /// All armors defined in XML.
        /// </summary>
        public ICollection<ArmorItem> Armors { get { return _armors; } }
        /// <summary>
        /// All weapons defined in XML
        /// </summary>
        public ICollection<WeaponItem> Weapons { get { return _weapons; } }
        /// <summary>
        /// All non-weapon, non-armor items in XML.
        /// </summary>
        public ICollection<Item> Items { get { return _items; } }

        /// <summary></summary>
        public bool Contains(IItem item)
        {
            if (item.IsArmor) { return _armors.Count(x => x.Name == item.Name) > 0; }
            if (item.IsWeapon) { return _weapons.Count(x => x.Name == item.Name) > 0; }
            else { return _items.Count(x => x.Name == item.Name) > 0; }
        }

        /// <summary>
        /// 
        /// </summary>
        /// <param name="itemReader"></param>
        public ItemDataBase(XMLItemReader itemReader)
        {
            _armors = itemReader.ParseBasicArmors();
            _weapons = itemReader.ParseBasicWeapons();
            _items = itemReader.ParseItems();
        }



    }
}
