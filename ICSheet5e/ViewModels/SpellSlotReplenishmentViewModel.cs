using System;
using System.Collections.Generic;
using ICSheetCore;
using System.Windows.Input;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;

namespace ICSheet5e.ViewModels
{
    public class SpellSlotReplenishmentViewModel : BaseViewModel
    {
        private Action<IDictionary<int, int>> _callback;
        private IEnumerable<int> _slots;
        private int maxSlotLevel;
        private bool isWarlock;
        private Dictionary<int, int> _slotsToReplenish = new Dictionary<int, int>()
        {
            {1, 0 },
            {2, 0 },
            {3, 0 },
            {4, 0 },
            {5, 0 },
            {6, 0 },
            {7, 0 },
            {8, 0 },
            {9, 0 },
        };

        private int _selectedSlotLevel;
        private int _numberOfSlots;
        private int _totalPossible;

        public string Slots
        {
            get
            {
                if (_slots == null) { return ""; }
                return string.Join(" / ", _slots);
            }
        }

        public int MaxSlotLevel
        {
            get
            {
                return maxSlotLevel;
            }

            set
            {
                maxSlotLevel = value;
                NotifyPropertyChanged();
            }
        }

        public bool IsWarlock
        {
            get
            {
                return isWarlock;
            }

            set
            {
                isWarlock = value;
                NotifyPropertyChanged();
            }
        }

        public int SelectedSlotLevel
        {
            get
            {
                return _selectedSlotLevel;
            }

            set
            {
                _selectedSlotLevel = value;
                NotifyPropertyChanged();
            }
        }

        public int NumberOfSlots
        {
            get
            {
                return _numberOfSlots;
            }

            set
            {
                if (value > 0 && value * SelectedSlotLevel < TotalPossible)
                {
                    TotalPossible -= (value - _numberOfSlots) * SelectedSlotLevel;
                    _slotsToReplenish[SelectedSlotLevel] = value;
                }
                _numberOfSlots = value;

                NotifyPropertyChanged();
            }
        }

        public int TotalPossible
        {
            get
            {
                return _totalPossible;
            }

            set
            {
                _totalPossible = value;
                NotifyPropertyChanged();
            }
        }

        public SpellSlotReplenishmentViewModel(IViewModel parent, Action<IDictionary<int, int>> callback)
        {
            Parent = parent;
            _callback = callback;
        }

        public void SetData(IEnumerable<int> currentSlots, SpellcastingLookup.CastingType castingType, int level)
        {
            _slots = currentSlots;
            IsWarlock = castingType == SpellcastingLookup.CastingType.Warlock;
            if (isWarlock)
            {
                var allSlots = SpellcastingLookup.SpellSlotsFor(SpellcastingLookup.CastingType.Warlock, level).ToList();
                SelectedSlotLevel = allSlots.FindIndex(x => x != 0) + 1; //slots are 1 indexed
                var n = allSlots[SelectedSlotLevel - 1]; //indexes are 0-based
                TotalPossible = SelectedSlotLevel * n;
                NumberOfSlots = n;
                _slotsToReplenish[SelectedSlotLevel] = n;
                NotifyPropertyChanged("SlotsToReplenish");
            }
            else { TotalPossible = level / 2; SelectedSlotLevel = 1; }
            
            switch (castingType)
            {
                case SpellcastingLookup.CastingType.Full:
                case SpellcastingLookup.CastingType.Warlock:
                    MaxSlotLevel = Math.Min((int)Math.Ceiling((double)level / 2), 6);
                    break;
                case SpellcastingLookup.CastingType.Half:
                    MaxSlotLevel = Math.Min((int)Math.Ceiling((double)level / 4), 6);
                    break;
                case SpellcastingLookup.CastingType.Martial:
                    MaxSlotLevel = Math.Min((int)Math.Ceiling((double)level / 6), 6);
                    break;
            }
            NotifyPropertyChanged("Slots");
            
        }

        public ICommand OnCompletionCommand
        {
            get { return new Views.DelegateCommand<bool>(onCompletionCommandExecuted); }
        }

        public IEnumerable<int> SlotNames
        {
            get { return Enumerable.Range(1, MaxSlotLevel); }
        }

        private void onCompletionCommandExecuted(bool obj)
        {
            IsOpen = false;
            Parent.IsOpen = false;
            _callback(_slotsToReplenish);
        }

        public bool CanChoose
        {
            get { return !isWarlock; }
        }

        public string SlotsToReplenish
        {
            get
            {
                var formatted = _slotsToReplenish.Select(x => $"{x.Key}: {x.Value}");
                return string.Join(", ", formatted);
            }
        }
    }
}
