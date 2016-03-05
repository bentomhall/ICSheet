using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Runtime.Serialization;
using System.Xml;
using System.Xml.Linq;
using System.IO;
using Interactive_Character_Sheet_Core;

namespace ICSheet5e.Model
{
    [DataContract]
    class ItemDataBase
    {
        [DataMember]
        private List<ArmorItem> _armors;
        [DataMember]
        private List<Item> _items;
        [DataMember]
        private List<WeaponItem> _weapons;

        public List<ArmorItem> Armors { get { return _armors; } }
        public List<WeaponItem> Weapons { get { return _weapons; } }
        public List<Item> Items { get { return _items; } }

        public bool Contains(IItem item)
        {
            if (item.IsArmor) { return _armors.Contains(item as ArmorItem); }
            if (item.IsWeapon) { return _weapons.Contains(item as WeaponItem); }
            else { return _items.Contains(item as Model.Item); }
        }

        public ItemDataBase()
        {
            if (_armors == null) { _armors = ParseBasicArmors(); }
            if (_items == null) { _items = ParseItems(); }
            if (_weapons == null) { _weapons = ParseBasicWeapons(); }
        }

        private List<ArmorItem> ParseBasicArmors()
        {
            var data = ICSheet5e.Properties.Resources.BasicArmors;
            var doc = XDocument.Parse(data);
            var output = new List<ArmorItem>();
            foreach (var armor in doc.Descendants("Armor"))
            {
                var name = armor.Attribute("Name").Value;
                var weight = int.Parse(armor.Element("Weight").Value);
                var value = int.Parse(armor.Element("Cost").Value);
                var properties = armor.Element("Properties").Value;
                var ac = armor.Element("ArmorClass").Value;
                var armorType = armor.Attribute("ArmorType").Value;
                ArmorType type = ArmorType.None;
                if (armorType == "Light")
                {
                    type = ArmorType.Light;
                }
                else if (armorType == "Medium")
                {
                    type = ArmorType.Medium;
                }
                else if (armorType == "Heavy")
                {
                    type = ArmorType.Heavy;
                }
                var newItem = new ArmorItem(name, weight, value, false, properties, type);
                newItem.BaseEffect = ac;
                output.Add(newItem);
            }
            return output;
        }

        private List<Item> ParseItems()
        {
            return new List<Item>();
        }

        private List<WeaponItem> ParseBasicWeapons()
        {
            var data = ICSheet5e.Properties.Resources.BasicWeapons;
            var doc = XDocument.Parse(data);
            var output = new List<WeaponItem>();
            foreach (var weapon in doc.Descendants("Weapon"))
            {
                var name = weapon.Attribute("Name").Value;
                var weight = int.Parse(weapon.Element("Weight").Value);
                var value = int.Parse(weapon.Element("Cost").Value);
                var properties = weapon.Element("Properties").Value;
                var damage = weapon.Element("Damage").Value;
                var dType = weapon.Element("DamageType").Value;
                var cat = weapon.Attribute("Category").Value;
                var category = WeaponCategory.SimpleMelee;
                if (cat == "SimpleRanged")
                {
                    category = WeaponCategory.SimpleRanged;
                }
                else if (cat == "MartialMelee")
                {
                    category = WeaponCategory.MartialMelee;
                }
                else if (cat == "MartialRanged")
                {
                    category = WeaponCategory.MartialRanged;
                }
                var type = DamageType.Bludgeoning;
                if (dType == "Piercing") { type = DamageType.Piercing; }
                else if (dType == "Slashing") { type = DamageType.Slashing; }
                var slot = ItemSlot.Mainhand;
                if (properties.Contains("Two-handed")) { slot = ItemSlot.TwoHanded; }
                var newItem = new WeaponItem(name, weight, value, slot, false, properties, category);
                newItem.BaseEffect = damage;
                newItem.Damage = type;
                output.Add(newItem);

            }
            return output;
        }

    }
}
