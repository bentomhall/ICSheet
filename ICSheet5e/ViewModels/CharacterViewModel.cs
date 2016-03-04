﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;
using Interactive_Character_Sheet_Core;
using System.Windows.Input;
using System.Windows;

namespace ICSheet5e.ViewModels
{
    public class CharacterViewModel : BaseViewModel
    {
        private Model.Character character;
        private string FormatLevels()
        {
            var builder = new StringBuilder();
            foreach (var entry in character.Levels)
            {
                builder.AppendFormat("{0} {1}", entry.Item1, entry.Item2);
                builder.Append(Environment.NewLine);
            }
            return builder.ToString();
        }
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
        #region Skills
        private ObservableCollection<bool> _proficientSkills = new ObservableCollection<bool>();
        public ObservableCollection<bool> ProficientSkills
        {
            get { return _proficientSkills; }
            set
            {
                _proficientSkills = value;
                NotifyPropertyChanged();
            }
        }
        private ObservableCollection<IndividualSkillViewModel> _skills = new ObservableCollection<IndividualSkillViewModel>();
        public ObservableCollection<IndividualSkillViewModel> Skills
        {
            get { return _skills; }
            set
            {
                _skills = value;
            }
        }

        public void SkillProficiencyChanged(string name, bool isProficient)
        {
            var skillVM = _skills.Single(x => x.Name == name);
            var newBonus = skillVM.Bonus;
            if (isProficient)
            {
                newBonus += Proficiency;
            }
            else
            {
                newBonus -= Proficiency;
            }
            var skill = new Model.Skill5e(name, newBonus, true);
            skillVM.Bonus = newBonus;
            character.Skills.SetSkillBonusFor(skill);
        }

        private void _setSkills(SkillList<Model.Skill5e> skills)
        {
            var names = skills.getSkillNames();
            Skills = new ObservableCollection<IndividualSkillViewModel>();
            foreach (var name in names)
            {
                IndividualSkillViewModel skill = new IndividualSkillViewModel();
                skill.Bonus = skills.skillBonusFor(name);
                skill.Name = name;
                skill.IsProficient = skills.IsSkillTagged(name);
                skill.delegateProficiencyChanged = SkillProficiencyChanged;
                Skills.Add(skill);
            }
            NotifyPropertyChanged("Skills");
        }
        #endregion

        public string Name
        {
            get { return character.CharacterName; }
        }
        public string Race
        {
            get { return character.Race; }
        }
        private string _levels = "";
        public string Levels
        {
            get { return _levels; }
        }

        public int Experience
        {
            get { return character.Experience; }
            set
            {
                character.Experience = value;
                NotifyPropertyChanged();
            }
        }

        public int MaxHealth
        {
            get { return character.MaxHealth; }
            set
            {
                character.MaxHealth = value;
                NotifyPropertyChanged();
            }
        }

        public int CurrentHealth
        {
            get { return character.CurrentHealth; }
        }

        public int TemporaryHP
        {
            get { return character.TemporaryHP; }
        }

        public ICommand TakeDamageCommand 
        {
            get { return new Views.DelegateCommand<object>(TakeDamageCommandExecuted); }
        }

        private void TakeDamageCommandExecuted(object obj)
        {
            var type = HealthChangeViewModel.HealthChangeType.Damage;
            DisplayModalHealthDialog(type);
        }

        public ICommand HealDamageCommand
        {
            get { return new Views.DelegateCommand<object>(HealDamageCommandExecuted); }
        }

        private void HealDamageCommandExecuted(object obj)
        {
            var type = HealthChangeViewModel.HealthChangeType.Healing;
            DisplayModalHealthDialog(type); 
        }

        private bool HealDamageCommandCanExecute(object obj)
        {
            return (CurrentHealth < MaxHealth);
        }

        public ICommand AddTemporaryHealthCommand
        {
            get { return new Views.DelegateCommand<object>(AddTHPCommandExecuted); }
        }

        private void AddTHPCommandExecuted(object obj)
        {
            var type = HealthChangeViewModel.HealthChangeType.Temporary;
            DisplayModalHealthDialog(type);
        }

        private void DisplayModalHealthDialog(HealthChangeViewModel.HealthChangeType type)
        {

            var vm = new HealthChangeViewModel();
            vm.Type = type;
            Views.WindowManager.DisplayDialog(Views.WindowManager.DialogType.HealthDialog, vm,HandleHealthChange);
        }

        private void HandleHealthChange(IViewModel vm)
        {
            var model = vm as HealthChangeViewModel;
            switch(model.Type)
            {
                case HealthChangeViewModel.HealthChangeType.Damage:
                    var dmg = new DamageBase();
                    dmg.Amount = model.Amount;
                    character.TakeDamage(dmg);
                    break;
                case HealthChangeViewModel.HealthChangeType.Healing:
                    character.HealDamage(model.Amount);
                    break;
                case HealthChangeViewModel.HealthChangeType.Temporary:
                    character.AddTHP(model.Amount);
                    
                    break;
                default:
                    break;
            }
            NotifyPropertyChanged("CurrentHealth");
            NotifyPropertyChanged("TemporaryHP");
            return;
        }

        public bool CanEdit
        {
            get { return canEdit; }
            set
            {
                canEdit = value;
                NotifyPropertyChanged();
                if (!canEdit) NotifyEditingEnded(); //raise property changed notifications for directly bound properties
                if (canEdit) NotifyEditingBegan();
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

        public CharacterViewModel(Model.Character c, ApplicationModel parent)
        {
            character = c;
            _setSkills(c.Skills);
            _levels = FormatLevels();
            Parent = parent;
            Parent.PropertyChanged += ParentEditingPropertyChanged;
        }

        public void NotifyEditingBegan()
        {
            NotifyPropertyChanged("Strength");
            NotifyPropertyChanged("Dexterity");
            NotifyPropertyChanged("Constitution");
            NotifyPropertyChanged("Intelligence");
            NotifyPropertyChanged("Wisdom");
            NotifyPropertyChanged("Charisma");
        }

        public void NotifyEditingEnded()
        {
            character.RecalculateDependentBonuses();
            NotifyPropertyChanged("ArmorClass");
            NotifyPropertyChanged("Proficiency");
            NotifyPropertyChanged("Movement");
            NotifyPropertyChanged("Initiative");
            NotifyPropertyChanged("Defenses");
            NotifyPropertyChanged("ProficientDefenses");
            
        }

        private void ParentEditingPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsEditingModeEnabled")
            {
                CanEdit = !canEdit;
                if (!canEdit) NotifyEditingEnded();
            }
        }

    }
}
