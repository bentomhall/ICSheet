using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.IO;

namespace ICSheetCore
{
    [DataContract]
    public class ItemDataBase
    {
        [DataMember]
        private List<ArmorItem> _armors;
        [DataMember]
        private List<Item> _items;
        [DataMember]
        private List<WeaponItem> _weapons;

        public ICollection<ArmorItem> Armors { get { return _armors; } }
        public ICollection<WeaponItem> Weapons { get { return _weapons; } }
        public ICollection<Item> Items { get { return _items; } }

        public bool Contains(IItem item)
        {
            if (item.IsArmor) { return _armors.Contains(item as ArmorItem); }
            if (item.IsWeapon) { return _weapons.Contains(item as WeaponItem); }
            else { return _items.Contains(item as Item); }
        }

        public ItemDataBase(XMLItemReader itemReader)
        {
            _armors = itemReader.ParseBasicArmors();
            _weapons = itemReader.ParseBasicWeapons();
            _items = itemReader.ParseItems();
        }



    }
}
