namespace ICSheet5e.ViewModels
{
    public interface IViewModel
    {
        IViewModel Parent { get; set; }
        event System.ComponentModel.PropertyChangedEventHandler PropertyChanged;
        bool IsOpen { get; set; }
    }
}
