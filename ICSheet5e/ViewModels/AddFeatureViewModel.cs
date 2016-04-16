using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using ICSheetCore;

namespace ICSheet5e.ViewModels
{
    public class AddFeatureViewModel : BaseViewModel
    {
        public AddFeatureViewModel() {}

        public string FeatureName { get; set; }
        public string Uses { get; set; }
        public string FeatureText { get; set; }
        public int StartingLevel { get; set; }

        public MartialFeature ToFeature()
        {
            var feature = new MartialFeature(FeatureName, FeatureText, Uses ?? "");
            feature.MinimumLevel = StartingLevel;
            return feature;
        }
    }
}
