using System;
using System.Runtime.CompilerServices;
using System.ComponentModel;


namespace ICSheet5e.ViewModels
{
    public class BaseViewModel: INotifyPropertyChanged, ICSheet5e.ViewModels.IViewModel
    {
        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        [System.Diagnostics.CodeAnalysis.SuppressMessage("Microsoft.Design", "CA1026:DefaultParametersShouldNotBeUsed")]
        protected void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion


        public ApplicationModel Parent { get; set; }
    }
}
