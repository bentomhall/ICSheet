using System;
using System.Windows.Input;

namespace ICSheet5e.ViewModels
{
    public class UserPreferencesViewModel : BaseViewModel
    {
        public UserPreferencesViewModel(Action onCompletion)
        {
            _callback = onCompletion;
            EncumbranceMultiplier = Properties.Settings.Default.EncumbranceMultiplier;
            CoinWeight = Properties.Settings.Default.CashWeight;
        }

        public double EncumbranceMultiplier
        {
            get { return _encumbranceMultiplier; }
            set
            {
                if (value != _encumbranceMultiplier)
                {
                    _encumbranceMultiplier = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public double CoinWeight
        {
            get { return _coinWeight; }
            set
            {
                if (value != _coinWeight)
                {
                    _coinWeight = value;
                    NotifyPropertyChanged();
                }
            }
        }

        public ICommand OnSave
        {
            get { return new Views.DelegateCommand<object>(x => { _callback(); Properties.Settings.Default.Save(); }); }
        }

        public ICommand CancelCommand
        {
            get { return new Views.DelegateCommand<object>(x => _callback()); }
        }

        private Action _callback;
        private double _encumbranceMultiplier;
        private double _coinWeight;
    }
}
