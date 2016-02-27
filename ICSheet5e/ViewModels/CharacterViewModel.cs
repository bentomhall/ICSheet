using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Interactive_Character_Sheet_Core;

namespace ICSheet5e.ViewModels
{
    class CharacterViewModel : INotifyPropertyChanged
    {
        private Model.Character character;
        private bool canEdit = false;
        #region Attributes
        public int Strength
        {
            get
            {
                if (canEdit) return character.AbilityScoreFor(AbilityType.Strength);
                else return character.abilityModifierFor(AbilityType.Strength);
            }
            set
            {
                character.mutateAbilityScore(AbilityType.Strength, value);
                NotifyPropertyChanged();
            }
        }
        public int Dexterity
        {
            get
            {
                if (canEdit) return character.AbilityScoreFor(AbilityType.Dexterity);
                else return character.abilityModifierFor(AbilityType.Dexterity);
            }
            set
            {
                character.mutateAbilityScore(AbilityType.Dexterity, value);
                NotifyPropertyChanged();
            }
        }
        public int Constitution
        {
            get
            {
                if (canEdit) return character.AbilityScoreFor(AbilityType.Constitution);
                else return character.abilityModifierFor(AbilityType.Constitution);
            }
            set
            {
                character.mutateAbilityScore(AbilityType.Constitution, value);
                NotifyPropertyChanged();
            }
        }
        public int Intelligence
        {
            get
            {
                if (canEdit) return character.AbilityScoreFor(AbilityType.Intelligence);
                else return character.abilityModifierFor(AbilityType.Intelligence);
            }
            set
            {
                character.mutateAbilityScore(AbilityType.Intelligence, value);
                NotifyPropertyChanged();
            }
        }
        public int Wisdom
        {
            get
            {
                if (canEdit) return character.AbilityScoreFor(AbilityType.Wisdom);
                else return character.abilityModifierFor(AbilityType.Wisdom);
            }
            set
            {
                character.mutateAbilityScore(AbilityType.Wisdom, value);
                NotifyPropertyChanged();
            }
        }
        public int Charisma
        {
            get
            {
                if (canEdit) return character.AbilityScoreFor(AbilityType.Charisma);
                else return character.abilityModifierFor(AbilityType.Charisma);
            }
            set
            {
                character.mutateAbilityScore(AbilityType.Charisma, value);
                NotifyPropertyChanged();
            }
        }
        #endregion

        #region Defenses
        public int ArmorClass
        {
            get { return character.ArmorClass; }
            set
            {
                character.ArmorClass = value;
                NotifyPropertyChanged();
            }
        }

        public List<Defense> Defenses
        {
            get { return character.Defenses; }
        }

        public List<bool> ProficientDefenses 
        {
            get { return character.ProficientDefenses; }
        }

        #endregion
        public bool CanEdit
        {
            get { return canEdit; }
            set
            {
                canEdit = value;
                NotifyPropertyChanged();
                if (!canEdit) NotifyEditingEnded(); //raise property changed notifications for directly bound properties
            }
        }
        public Model.Character Character 
        {
            get { return character; }
            set
            {
                character = value;
                NotifyPropertyChanged();
            }
        }

        public int Proficiency
        {
            get { return character.Proficiency; }
        }
        public int Movement
        {
            get { return character.Movement; }
        }
        public int Initiative
        {
            get { return character.Initiative; }
        }

        public CharacterViewModel() { }

        public CharacterViewModel(Model.Character c, INotifyPropertyChanged parent)
        {
            character = c;
            parent.PropertyChanged += ParentEditingPropertyChanged;
        }

        public void NotifyEditingEnded()
        {
            NotifyPropertyChanged("ArmorClass");
            NotifyPropertyChanged("Proficiency");
            NotifyPropertyChanged("Movement");
            NotifyPropertyChanged("Initiative");
            NotifyPropertyChanged("ProficientDefenses");
        }



        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        private void ParentEditingPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsEditingModeEnabled")
            {
                CanEdit = !canEdit;
            }
        }
        #endregion

    }
}
