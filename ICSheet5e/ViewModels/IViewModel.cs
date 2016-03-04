using System;
namespace ICSheet5e.ViewModels
{
    public interface IViewModel
    {
        ApplicationModel Parent { get; set; }
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
    }
}
