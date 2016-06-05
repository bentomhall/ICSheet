using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSheetCore;
using System.Windows.Input;

namespace ICSheet5e.ViewModels
{
    public class AddFeatureViewModel : BaseViewModel
    {
        private bool _isOpen;
        private Action<IFeature> _callback;

        public AddFeatureViewModel(Action<IFeature> callback)
        {
            _callback = callback;
        }

        public string FeatureName { get; set; }
        public string Uses { get; set; }
        public string FeatureText { get; set; }
        public int StartingLevel { get; set; }

        public ICommand CreateFeatureCommand
        {
            get { return new Views.DelegateCommand<object>(x => { _callback(ToFeature()); IsOpen = false; }); }
        }

        public IFeature ToFeature()
        {
            return new ClassFeature(FeatureName, StartingLevel, false, FeatureText);
        }
    }
}
