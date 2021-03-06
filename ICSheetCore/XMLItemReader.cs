﻿using System;
using System.Collections.Generic;
using System.Xml.Linq;

namespace ICSheetCore
{
    /// <summary>
    /// 
    /// </summary>
    public class XMLItemReader
    {
        private string _armorsXML;
        private string _weaponsXML;
        private string _itemsXML;

        /// <summary>
        /// 
        /// </summary>
        /// <param name="armors"></param>
        /// <param name="weapons"></param>
        /// <param name="items"></param>
        public XMLItemReader(string armors, string weapons, string items)
        {
            if (string.IsNullOrWhiteSpace(armors)) { throw new ArgumentNullException("armors"); }
            if (string.IsNullOrWhiteSpace(weapons)) { throw new ArgumentNullException("weapons"); }
            if (string.IsNullOrWhiteSpace(items)) { throw new ArgumentNullException("items"); }
            _armorsXML = armors;
            _weaponsXML = weapons;
            _itemsXML = items;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<ArmorItem> ParseBasicArmors()
        {
            var doc = XDocument.Parse(_armorsXML);
            var output = new List<ArmorItem>();
            foreach (var armor in doc.Descendants("Armor"))
            {
                var name = armor.Attribute("Name").Value;
                var weight = int.Parse(armor.Element("Weight").Value);
                var value = int.Parse(armor.Element("Cost").Value);
                var properties = armor.Element("Properties").Value;
                var ac = armor.Element("ArmorClass").Value;
                var armorType = armor.Attribute("ArmorType").Value;
                ArmorType type = (ArmorType)Enum.Parse(typeof(ArmorType), armorType);
                
                var newItem = new ArmorItem(name, weight, value, false, properties, type, 0); //all default items are unenchanted, so bonus of 0
                if (type != ArmorType.Shield) { newItem.Slot = ItemSlot.Armor; }
                else { newItem.Slot = ItemSlot.Offhand; }
                newItem.BaseEffect = ac;
                output.Add(newItem);
            }
            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<Item> ParseItems()
        {
            var doc = XDocument.Parse(_itemsXML);
            var output = new List<Item>();
            foreach (var item in doc.Root.Elements("Item"))
            {
                var name = item.Attribute("Name").Value;
                var weight = double.Parse(item.Element("Weight").Value);
                var value = double.Parse(item.Element("Cost").Value);
                var count = int.Parse(item.Element("Count").Value);
                var properties = item.Element("Properties").Value;
                output.Add(new Item(name, weight, value, ItemSlot.None, false, properties, 0));
            }
            return output;
        }

        /// <summary>
        /// 
        /// </summary>
        /// <returns></returns>
        public List<WeaponItem> ParseBasicWeapons()
        {
            var doc = XDocument.Parse(_weaponsXML);
            var output = new List<WeaponItem>();
            foreach (var weapon in doc.Descendants("Weapon"))
            {
                var name = weapon.Attribute("Name").Value;
                var weight = double.Parse(weapon.Element("Weight").Value);
                var value = double.Parse(weapon.Element("Cost").Value);
                var properties = weapon.Element("Properties").Value;
                var damage = weapon.Element("Damage").Value;
                var dType = weapon.Element("DamageType").Value;
                var cat = weapon.Attribute("Category").Value;
                var category = (WeaponCategory)Enum.Parse(typeof(WeaponCategory), cat);
                var slot = ItemSlot.Mainhand;
                if (properties.Contains("Two-handed")) { slot = ItemSlot.TwoHanded; }
                var newItem = new WeaponItem(name, weight, value, slot, false, properties, category, 0);
                newItem.BaseEffect = damage;
                newItem.Damage = dType;
                output.Add(newItem);

            }
            return output;
        }
    }
}
