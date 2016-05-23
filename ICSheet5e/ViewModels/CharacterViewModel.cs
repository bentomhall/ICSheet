using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Text;
using System.Windows.Input;
using ICSheetCore;

namespace ICSheet5e.ViewModels
{
    public class CharacterViewModel : BaseViewModel
    {
        public CharacterViewModel()
        {
        }

        public CharacterViewModel(PlayerCharacter c, ApplicationModel parent)
        {
            character = c;
            _setSkills();
            Parent = parent;
            Parent.PropertyChanged += ParentEditingPropertyChanged;
            Attacks = new ObservableCollection<AttackViewModel>();
            Attacks.Add(attackModelFor(c.EquippedItemForSlot(ItemSlot.Mainhand)));
            var oh = c.EquippedItemForSlot(ItemSlot.Offhand) as WeaponItem;
            if (oh != null) { Attacks.Add(attackModelFor(oh)); }
            //character.PropertyChanged += ParentEditingPropertyChanged;
        }

        public ICommand AddTemporaryHealthCommand
        {
            get { return new Views.DelegateCommand<object>(AddTHPCommandExecuted); }
        }

        public string AvailableSpellSlots
        {
            get { return slotListAsString(character.SpellSlots); }
        }

        public bool CanCastSpells
        {
            get { return character.IsSpellcaster; }
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

        public PlayerCharacter Character
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
            get { return character.ArmorClassOverride; }
            set
            {
                character.ArmorClassOverride = value;
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
            get { return character.SpellPreparationCount; }
        }

        public ObservableCollection<IFeature> Features
        {
            get { return new ObservableCollection<IFeature>(character.Features); }
        }

        public Money Gold
        {
            get { return character.Cash; }
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
            get { return character.Name; }
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

        public IEnumerable<Spell> PreparedSpells
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

        public string Race
        {
            get { return character.RaceName; }
        }

        public int SelectedSpellLevel
        {
            get;
            set;
        }

        public string SpellAttackBonus
        {
            get
            {
                var values = character.SpellAttackBonuses;
                var sb = new List<string>();
                foreach (KeyValuePair<string, int> entry in values)
                {
                    sb.Add($"{entry.Key}: {entry.Value}");
                }
                return string.Join(Environment.NewLine, sb);
            }
        }

        public string SpellDC
        {
            get
            {
                var values = character.SpellDCs;
                var sb = new List<string>() ;
                foreach (KeyValuePair<string, int> entry in values)
                {
                    sb.Add($"{entry.Key}: {entry.Value}");
                }
                return string.Join(Environment.NewLine, sb);
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

        private bool _showHealthChangePopup;
        public bool ShowHealthChangePopup
        {
            get { return _showHealthChangePopup; }
            set
            {
                _showHealthChangePopup = value;
                NotifyPropertyChanged();
            }
        }

        public ICommand ShowHealthChangeOverlay
        {
            get
            {
                return new Views.DelegateCommand<string>(x => { ShowHealthChangePopup = !_showHealthChangePopup; isTHPChange = x == "THP"; });
            }
        }

        private bool isTHPChange;

        public int HealthChangeAmount { get; set; }

        public ICommand DoHealthChangeCommand
        {
            get { return new Views.DelegateCommand<string>(DoHealthChangeCommandExecuted); }
        }

        private void DoHealthChangeCommandExecuted(string obj)
        {
            if (isTHPChange) { character.AddTHP(Math.Abs(HealthChangeAmount)); NotifyPropertyChanged("TemporaryHP"); }
            else if (HealthChangeAmount > 0) { character.HealDamage(HealthChangeAmount); }
            else { character.TakeDamage(HealthChangeAmount); NotifyPropertyChanged("TemporaryHP"); }
            NotifyPropertyChanged("CurrentHealth");
            ShowHealthChangePopup = false;
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

        private void NotifyDefensesChanged()
        {
            NotifyPropertyChanged("StrengthSave");
            NotifyPropertyChanged("DexteritySave");
            NotifyPropertyChanged("ConstitutionSave");
            NotifyPropertyChanged("IntelligenceSave");
            NotifyPropertyChanged("WisdomSave");
            NotifyPropertyChanged("CharismaSave");
        }

        public void NotifyEditingEnded()
        {
            NotifyPropertyChanged("ArmorClass");
            NotifyPropertyChanged("Proficiency");
            NotifyPropertyChanged("Movement");
            NotifyPropertyChanged("Initiative");
            NotifyDefensesChanged();
            _setSkills();
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

        public Spell SelectedPreparedSpell { get; set; }

        private bool canEdit = false;
        private PlayerCharacter character;

        private void AddTHPCommandExecuted(object obj)
        {
            var type = HealthChangeViewModel.HealthChangeType.Temporary;
            DisplayModalHealthDialog(type);
        }

        private void CastSpellCommandExecuted(object obj)
        {
            character.UseSpellSlot(SelectedSpellLevel);
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
            var s = new List<string>();
            foreach (KeyValuePair<string, int> entry in character.Levels)
            {
                s.Add($"{entry.Key} {entry.Value}");
            }
            return string.Join(Environment.NewLine, s);
        }

        private void HandleHealthChange(IViewModel vm)
        {
            var model = vm as HealthChangeViewModel;
            switch (model.Type)
            {
                case HealthChangeViewModel.HealthChangeType.Damage:
                    character.TakeDamage(model.Amount);
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
                character.ModifyAbilityScore(AbilityType.Charisma, value);
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
                character.ModifyAbilityScore(AbilityType.Constitution, value);
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
                character.ModifyAbilityScore(AbilityType.Dexterity, value);
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
                character.ModifyAbilityScore(AbilityType.Intelligence, value);
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
                character.ModifyAbilityScore(AbilityType.Strength, value);
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
                character.ModifyAbilityScore(AbilityType.Wisdom, value);
                NotifyPropertyChanged();
            }
        }

        #endregion Attributes

        #region Defenses

        public int ArmorClass
        {
            get { return character.ArmorClassBonus; }
        }

        public Tuple<int, bool> StrengthSave { get { return character.DefenseBonusFor(DefenseType.Strength); } }
        public Tuple<int, bool> ConstitutionSave { get { return character.DefenseBonusFor(DefenseType.Constitution); } }
        public Tuple<int, bool> DexteritySave { get { return character.DefenseBonusFor(DefenseType.Dexterity); } }
        public Tuple<int, bool> IntelligenceSave { get { return character.DefenseBonusFor(DefenseType.Intelligence); } }
        public Tuple<int, bool> WisdomSave { get { return character.DefenseBonusFor(DefenseType.Wisdom); } }
        public Tuple<int, bool> CharismaSave { get { return character.DefenseBonusFor(DefenseType.Charisma); } }

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

        public void SkillProficiencyChanged(IndividualSkillViewModel sender, ProficiencyType proficiency)
        {
            sender.Bonus = character.SkillBonusFor(sender.Name, proficiency);
            character.SkillProficiencies[sender.Name] = proficiency;
        }

        private ObservableCollection<bool> _proficientSkills = new ObservableCollection<bool>();
        private ObservableCollection<IndividualSkillViewModel> _skills = new ObservableCollection<IndividualSkillViewModel>();

        private void _setSkills()
        {
            var names = XMLFeatureFactory.SkillNames;
            Skills = new ObservableCollection<IndividualSkillViewModel>();
            foreach (var name in names)
            {
                ProficiencyType proficiency;
                try { proficiency = character.SkillProficiencies[name]; }
                catch (NullReferenceException)
                {
                    character.SkillProficiencies = new Dictionary<string, ProficiencyType>();
                    proficiency = ProficiencyType.None;
                }
                catch (KeyNotFoundException)
                {
                    character.SkillProficiencies[name] = ProficiencyType.None;
                    proficiency = ProficiencyType.None;
                }
                var bonus = character.SkillBonusFor(name, proficiency);
                IndividualSkillViewModel skillVM = new IndividualSkillViewModel(name, bonus, proficiency);
                skillVM.delegateProficiencyChanged = SkillProficiencyChanged;
                skillVM.Parent = Parent;
                Skills.Add(skillVM);
            }
            NotifyPropertyChanged("Skills");
        }

        #endregion Skills

        #region Attacks

        public ObservableCollection<AttackViewModel> Attacks { get; set; }

        private AttackViewModel attackModelFor(IItem item)
        {
            var weapon = item as WeaponItem;
            if (weapon == null) { return AttackViewModel.DefaultModel(character.AttackBonusWith(item)); }
            var vm = new AttackViewModel();
            vm.Name = weapon.Name;
            vm.AttackBonus = character.AttackBonusWith(weapon);
            vm.BaseDamage = weapon.BaseEffect;
            vm.StaticBonus = character.DamageBonusWith(weapon);
            return vm;
        }

        #endregion Attacks

        static private string slotListAsString(IEnumerable<int> slots)
        {
            return string.Join(" / ", slots);
        }

        private void TakeDamageCommandExecuted(object obj)
        {
            var type = HealthChangeViewModel.HealthChangeType.Damage;
            DisplayModalHealthDialog(type);
        }
    }
}