using System;
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

        private int strength = 10;
        public int StrengthScore
        {
            get { return strength; }
            set
            {
                strength = value;
                NotifyPropertyChanged();
            }
        }
        public int StrengthModifier
        {
            get { return ModifierFor(strength); }
        }

        private int dexterity = 10;
        public int DexterityScore
        {
            get { return dexterity; }
            set
            {
                dexterity = value;
                NotifyPropertyChanged();
            }
        }
        public int DexterityModifier { get { return ModifierFor(dexterity); } }

        private int constitution = 10;
        public int ConstitutionScore
        {
            get { return constitution; }
            set
            {
                constitution = value;
                NotifyPropertyChanged();
            }
        }
        public int ConstitutionModifier { get { return ModifierFor(constitution); } }

        private int intelligence = 10;
        public int IntelligenceScore
        {
            get { return intelligence; }
            set
            {
                intelligence = value;
                NotifyPropertyChanged();
            }
        }
        public int IntelligenceModifier { get { return ModifierFor(intelligence); } }

        private int wisdom = 10;
        public int WisdomScore
        {
            get { return wisdom; }
            set { wisdom = value; NotifyPropertyChanged(); }
        }
        public int WisdomModifier { get { return ModifierFor(wisdom); } }

        private int charisma = 10;
        public int CharismaScore
        {
            get { return charisma; }
            set { charisma = value; NotifyPropertyChanged(); }
        }
        public int CharismaModifier { get { return ModifierFor(charisma); } }



        public AttributeBoxViewModel(Dictionary<AbilityType, Ability> attributes)
        {
            StrengthScore = attributes[AbilityType.Strength].score;
            DexterityScore = attributes[AbilityType.Dexterity].score;
            ConstitutionScore = attributes[AbilityType.Constitution].score;
            IntelligenceScore = attributes[AbilityType.Intelligence].score;
            WisdomScore = attributes[AbilityType.Wisdom].score;
            CharismaScore = attributes[AbilityType.Charisma].score;
        }

        private int ModifierFor(int attributeScore)
        {
            return (attributeScore - 10) / 2;
        }
    }
}
