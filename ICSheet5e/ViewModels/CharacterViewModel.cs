using InteractiveCharacterSheetCore;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;

namespace ICSheet5e.ViewModels
{
    public class CharacterViewModel : BaseViewModel
    {
        public CharacterViewModel()
        {
        }

        public CharacterViewModel(Model.Character c, ApplicationModel parent)
        {
            character = c;
            _setSkills(c.Skills);
            Parent = parent;
            Parent.PropertyChanged += ParentEditingPropertyChanged;
            Attacks = new ObservableCollection<AttackViewModel>();
            Attacks.Add(attackModelFor(null));
            character.PropertyChanged += ParentEditingPropertyChanged;
        }

        public ICommand AddTemporaryHealthCommand
        {
            get { return new Views.DelegateCommand<object>(AddTHPCommandExecuted); }
        }

        public string AvailableSpellSlots
        {
            get { return slotListAsString(character.SpellSlots.Item2); }
        }

        public bool CanCastSpells
        {
            get { return character.IsSpellCaster; }
        }

        public bool CanEdit
        {
            get { return canEdit; }
            set
            {
                canEdit = value;
                NotifyPropertyChanged();
                if (!canEdit) NotifyEditingEnded(); //raise property changed notifications for directly bound properties
                if (canEdit) NotifyAttributesDisplayChanged();
            }
        }

        public ICommand CastSpellCommand
        {
            get { return new Views.DelegateCommand<object>(CastSpellCommandExecuted); }
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

        public int ArmorClassBonus
        {
            get { return character.ArmorClassBonus; }
            set
            {
                character.ArmorClassBonus = value;
                character.RecalculateArmorClass();
                NotifyPropertyChanged("ArmorClass");
            }
        }

        public int CurrentHealth
        {
            get { return character.CurrentHealth; }
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

        public string PreparedSpellsCount
        {
            get { return string.Format("{0} / {1}", PreparedSpells.Count(x => x.Level != 0), character.Spellcasting[0].MaxPreparedSpells); }
        }

        public ObservableCollection<Model.MartialFeature> Features
        {
            get { return new ObservableCollection<Model.MartialFeature>(character.Features.Where(x => x.MinimumLevel <= character.Levels.Max(y => y.Level))); }
        }

        public ObservableCollection<int> Gold
        {
            get { return ParseGold(character.Gold); }
        }

        public ICommand HealDamageCommand
        {
            get { return new Views.DelegateCommand<object>(HealDamageCommandExecuted); }
        }

        public int Initiative
        {
            get { return character.Initiative; }
        }

        public string Levels
        {
            get { return FormatLevels(); }
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

        public int Movement
        {
            get { return character.Movement; }
        }

        public string Name
        {
            get { return character.CharacterName; }
        }

        public string Notes
        {
            get { return character.Notes; }
            set
            {
                character.Notes = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<Model.Spell> PreparedSpells
        {
            get
            {
                return character.PreparedSpells;
            }
        }

        public int Proficiency
        {
            get { return character.Proficiency; }
        }

        public Model.Race Race
        {
            get { return character.CharacterRace; }
        }

        public Model.Spell SelectedPreparedSpell
        {
            get { return _selectedSpell; }
            set { _selectedSpell = value; NotifyPropertyChanged(); SelectedSpellLevel = value.Level; NotifyPropertyChanged("SelectedSpellLevel"); }
        }

        public int SelectedSpellLevel
        {
            get;
            set;
        }

        public int SpellAttackBonus
        {
            get
            {
                if (CanCastSpells) { return character.Spellcasting[0].SpellAttackModifier; }
                else { return 0; }
            }
        }

        public int SpellDC
        {
            get
            {
                if (CanCastSpells) { return character.Spellcasting[0].SpellDC; }
                else { return 0; }
            }
        }

        public ICommand TakeDamageCommand
        {
            get { return new Views.DelegateCommand<object>(TakeDamageCommandExecuted); }
        }

        public int TemporaryHP
        {
            get { return character.TemporaryHP; }
        }

        public void NotifyAttributesDisplayChanged()
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
            List<Model.Skill5e> newSkills = new List<Model.Skill5e>();
            foreach (var vm in Skills)
            {
                var skill = new Model.Skill5e(vm.Name, vm.Bonus, vm.IsProficient);
                newSkills.Add(skill);
            }
            character.RecalculateDependentBonuses();
            character.RecalculateSkillsAfterAbilityScoreChange(newSkills);
            NotifyPropertyChanged("ArmorClass");
            NotifyPropertyChanged("Proficiency");
            NotifyPropertyChanged("Movement");
            NotifyPropertyChanged("Initiative");
            NotifyPropertyChanged("Defenses");
            NotifyPropertyChanged("ProficientDefenses");
            _setSkills(character.Skills);
            NotifyPropertyChanged("Skills");
            var args = new PropertyChangedEventArgs("EquippedItems");
            OnEquipmentChanged(this, args);
            NotifyPropertyChanged("SpellAttackBonus");
            NotifyPropertyChanged("SpellDC");
            NotifyPropertyChanged("PreparedSpellsCount");
            NotifyAttributesDisplayChanged();
        }

        public void OnEquipmentChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "EquippedItems")
            {
                var mh = character.EquippedItemForSlot(ItemSlot.Mainhand);
                var oh = character.EquippedItemForSlot(ItemSlot.Offhand);
                var th = character.EquippedItemForSlot(ItemSlot.TwoHanded);
                if (th != null)
                {
                    Attacks.Clear();
                    Attacks.Add(attackModelFor(th));
                }
                else if (mh != null)
                {
                    Attacks.Clear();
                    Attacks.Add(attackModelFor(mh));
                    if (oh != null)
                    {
                        Attacks.Add(attackModelFor(oh));
                    }
                }
                NotifyPropertyChanged("Attacks");
            }
        }

        //private string _levels = "";
        private Model.Spell _selectedSpell;
        private bool canEdit = false;
        private Model.Character character;

        private void AddTHPCommandExecuted(object obj)
        {
            var type = HealthChangeViewModel.HealthChangeType.Temporary;
            DisplayModalHealthDialog(type);
        }

        private void CastSpellCommandExecuted(object obj)
        {
            if (SelectedPreparedSpell == null || SelectedSpellLevel < SelectedPreparedSpell.Level) { return; }
            character.CastSpell(SelectedPreparedSpell, SelectedSpellLevel);
            NotifyPropertyChanged("AvailableSpellSlots");
        }

        private void DisplayModalHealthDialog(HealthChangeViewModel.HealthChangeType type)
        {
            var vm = new HealthChangeViewModel();
            vm.Type = type;
            Views.WindowManager.DisplayDialog(Views.WindowManager.DialogType.HealthDialog, vm, HandleHealthChange);
        }

        private string FormatLevels()
        {
            var builder = new StringBuilder();
            foreach (var entry in character.Levels)
            {
                builder.Append(entry);
            }
            return builder.ToString();
        }

        private void HandleHealthChange(IViewModel vm)
        {
            var model = vm as HealthChangeViewModel;
            switch (model.Type)
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

        private void HealDamageCommandExecuted(object obj)
        {
            var type = HealthChangeViewModel.HealthChangeType.Healing;
            DisplayModalHealthDialog(type);
        }

        private void ParentEditingPropertyChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "IsEditingModeEnabled")
            {
                CanEdit = !canEdit;
            }
            else if (e.PropertyName == "Levels")
            {
                if (CanCastSpells)
                {
                    NotifyPropertyChanged("CanCastSpells");
                    NotifyPropertyChanged("SpellAttackBonus");
                    NotifyPropertyChanged("SpellDC");
                    NotifyPropertyChanged("AvailableSpellSlots");
                }
            }
            NotifyPropertyChanged(e.PropertyName);
        }

        static private ObservableCollection<int> ParseGold(double gold)
        {
            var goldList = new ObservableCollection<int>();
            var temp = gold;
            int g = (int)Math.Truncate(temp);
            temp = (temp - (double)g) * 100;
            int s = (int)Math.Truncate(temp);
            temp = (temp - (double)s) * 100;
            int c = (int)Math.Truncate(temp);
            goldList.Add(g);
            goldList.Add(s);
            goldList.Add(c);
            return goldList;
        }

        #region Attributes

        public int Charisma
        {
            get
            {
                if (canEdit) return character.AbilityScoreFor(AbilityType.Charisma);
                else return character.AbilityModifierFor(AbilityType.Charisma);
            }
            set
            {
                character.MutateAbilityScore(AbilityType.Charisma, value);
                NotifyPropertyChanged();
            }
        }

        public int Constitution
        {
            get
            {
                if (canEdit) return character.AbilityScoreFor(AbilityType.Constitution);
                else return character.AbilityModifierFor(AbilityType.Constitution);
            }
            set
            {
                character.MutateAbilityScore(AbilityType.Constitution, value);
                NotifyPropertyChanged();
            }
        }

        public int Dexterity
        {
            get
            {
                if (canEdit) return character.AbilityScoreFor(AbilityType.Dexterity);
                else return character.AbilityModifierFor(AbilityType.Dexterity);
            }
            set
            {
                character.MutateAbilityScore(AbilityType.Dexterity, value);
                NotifyPropertyChanged();
            }
        }

        public int Intelligence
        {
            get
            {
                if (canEdit) return character.AbilityScoreFor(AbilityType.Intelligence);
                else return character.AbilityModifierFor(AbilityType.Intelligence);
            }
            set
            {
                character.MutateAbilityScore(AbilityType.Intelligence, value);
                NotifyPropertyChanged();
            }
        }

        public int Strength
        {
            get
            {
                if (canEdit) return character.AbilityScoreFor(AbilityType.Strength);
                else return character.AbilityModifierFor(AbilityType.Strength);
            }
            set
            {
                character.MutateAbilityScore(AbilityType.Strength, value);
                NotifyPropertyChanged();
            }
        }

        public int Wisdom
        {
            get
            {
                if (canEdit) return character.AbilityScoreFor(AbilityType.Wisdom);
                else return character.AbilityModifierFor(AbilityType.Wisdom);
            }
            set
            {
                character.MutateAbilityScore(AbilityType.Wisdom, value);
                NotifyPropertyChanged();
            }
        }

        #endregion Attributes

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

        public IList<Defense> Defenses
        {
            get { return character.Defenses; }
        }

        public IReadOnlyCollection<bool> ProficientDefenses
        {
            get { return character.ProficientDefenses; }
        }

        #endregion Defenses

        #region Skills

        public ObservableCollection<bool> ProficientSkills
        {
            get { return _proficientSkills; }
            set
            {
                _proficientSkills = value;
                NotifyPropertyChanged();
            }
        }

        public ObservableCollection<IndividualSkillViewModel> Skills
        {
            get { return _skills; }
            set
            {
                _skills = value;
            }
        }

        public void SkillProficiencyChanged(Model.Skill5e skill)
        {
            var skillVM = _skills.Single(x => x.Skill == skill);
            if (skillVM.IsProficient)
            {
                skillVM.Bonus += Proficiency;
            }
            else
            {
                skillVM.Bonus -= Proficiency;
            }
            character.Skills.SetSkillBonusFor(skill);
            NotifyPropertyChanged("Skills");
        }

        private ObservableCollection<bool> _proficientSkills = new ObservableCollection<bool>();
        private ObservableCollection<IndividualSkillViewModel> _skills = new ObservableCollection<IndividualSkillViewModel>();

        private void _setSkills(SkillList<Model.Skill5e> skills)
        {
            var names = skills.SkillNames;
            Skills = new ObservableCollection<IndividualSkillViewModel>();
            foreach (var name in names)
            {
                IndividualSkillViewModel skillVM = new IndividualSkillViewModel();
                skillVM.Skill = skills.SkillForName(name);
                skillVM.delegateProficiencyChanged = SkillProficiencyChanged;
                skillVM.Parent = this.Parent;
                Skills.Add(skillVM);
            }
            NotifyPropertyChanged("Skills");
        }

        #endregion Skills

        #region Attacks

        public ObservableCollection<AttackViewModel> Attacks { get; set; }

        private AttackViewModel attackModelFor(IItem item)
        {
            var weapon = item as Model.WeaponItem;
            if (weapon == null) { return AttackViewModel.DefaultModel(character.AbilityModifierFor(AbilityType.Strength)); }
            var vm = new AttackViewModel();
            vm.Name = weapon.Name;
            vm.AttackBonus = character.AttackBonusWith(weapon);
            vm.BaseDamage = weapon.BaseEffect;
            vm.StaticBonus = character.DamageBonusWith(weapon);
            return vm;
        }

        #endregion Attacks

        static private string slotListAsString(List<int> slots)
        {
            return String.Format("{0} / {1} / {2} / {3} / {4} / {5} / {6} / {7} / {8}", slots[0], slots[1], slots[2], slots[3], slots[4], slots[5], slots[6], slots[7], slots[8]);
        }

        private void TakeDamageCommandExecuted(object obj)
        {
            var type = HealthChangeViewModel.HealthChangeType.Damage;
            DisplayModalHealthDialog(type);
        }
    }
}