using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;


namespace ICSheet5e.ViewModels
{
    public class BaseViewModel: INotifyPropertyChanged, IViewModel
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected void NotifyPropertyChanged([CallerMemberName] string propertyName = "")
        {
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(propertyName));
        }
        #endregion

        private bool _isOpen;
        public bool IsOpen
        {
            get { return _isOpen; }
            set
            {
                _isOpen = value;
                if (Parent != null) { Parent.IsOpen = value; }
                NotifyPropertyChanged();
            }
        }

        public IViewModel Parent { get; set; }
    }
}
