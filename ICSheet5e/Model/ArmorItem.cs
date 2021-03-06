﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using InteractiveCharacterSheetCore;
using System.Runtime.Serialization;

namespace ICSheet5e.Model
{
    [DataContract]
    public class ArmorItem : Item
    {
        [DataMember]
        public int MaxDexBonus { get; set; }
        [DataMember]
        public ArmorType ArmorClassType { get; set; }
        public int ArmorBonus
        {
            get { return int.Parse(BaseEffect); }
        }
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
                default:
                    break;
            }
        }
        protected override string CollectDescription()
        {
            var builder = new StringBuilder(Name);
            builder.AppendLine();
            builder.Append(ArmorClassType);
            builder.Append(Properties);
            return builder.ToString();
        }
    }

    public enum ArmorType
    {
        None,
        Light,
        Medium,
        Heavy
    }
}
