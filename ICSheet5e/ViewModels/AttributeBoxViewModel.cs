﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Interactive_Character_Sheet_Core;
using System.ComponentModel;
using System.Runtime.CompilerServices;


namespace ICSheet5e.ViewModels
{
    public class AttributeBoxViewModel: INotifyPropertyChanged
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public bool IsEditing { get; set; }
        private int strength = 10;
        public int Strength
        {
            get
            {
                if (IsEditing) return strength;
                else return ModifierFor(strength);
            }

            set
            {
                strength = value;
                NotifyPropertyChanged();
            }
        }

        private int dexterity = 10;
        public int Dexterity
        {
            get
            {
                if (IsEditing) return dexterity;
                else return ModifierFor(dexterity);
            }
            set
            {
                dexterity = value;
                NotifyPropertyChanged();
            }
        }

        private int constitution = 10;
        public int Constitution
        {
            get
            {
                if (IsEditing) return constitution;
                else return ModifierFor(constitution);
            }

            set
            {
                constitution = value;
                NotifyPropertyChanged();
            }
        }

        private int intelligence = 10;
        public int Intelligence
        {
            get
            {
                if (IsEditing) return intelligence;
                else return ModifierFor(intelligence);
            }

            set 
            { 
                intelligence = value;
                NotifyPropertyChanged();
            }
        }

        private int wisdom = 10;
        public int Wisdom
        {
            get
            {
                if (IsEditing) return wisdom;
                else return ModifierFor(wisdom);
            }
            set
            {
                wisdom = value;
                NotifyPropertyChanged();
            }
        }

        private int charisma = 10;
        public int Charisma
        {
            get
            {
                if (IsEditing) return charisma;
                else return ModifierFor(charisma);
            }
            set
            {
                charisma = value;
                NotifyPropertyChanged();
            }
        }

        public void SetAllAbilityScores(Dictionary<AbilityType, Ability> attributes)
        {
            Strength = attributes[AbilityType.Strength].score;
            Dexterity = attributes[AbilityType.Dexterity].score;
            Constitution = attributes[AbilityType.Constitution].score;
            Intelligence = attributes[AbilityType.Intelligence].score;
            Wisdom = attributes[AbilityType.Wisdom].score;
            Charisma = attributes[AbilityType.Charisma].score;
        }

        public AttributeBoxViewModel()
        {
            IsEditing = false;
        }

        private int ModifierFor(int attributeScore)
        {
            return (attributeScore - 10) / 2;
        }
    }
}