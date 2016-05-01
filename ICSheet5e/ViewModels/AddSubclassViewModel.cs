﻿using System;
using System.Collections.Generic;
using ICSheetCore;
using System.Linq;
using System.ComponentModel;
using System.Text;
using System.Threading.Tasks;

namespace ICSheet5e.ViewModels
{
    public class AddSubclassViewModel : BaseViewModel
    {
        private string _selectedClass;
        private string _selectedSubclass;
        private XMLFeatureFactory _factory;
        private IEnumerable<IFeature> _features;

        public AddSubclassViewModel(IEnumerable<string> validClassNames, XMLFeatureFactory featureFactory)
        {
            _factory = featureFactory;
            Classes = validClassNames;
            PropertyChanged += SelectionChanged;
            SelectedClass = validClassNames.First();
        }

        public IEnumerable<string> Classes { get; private set; }

        public IEnumerable<string> Subclasses { get; private set; }

        public string SelectedClass
        {
            get { return _selectedClass; }
            set
            {
                _selectedClass = value;
                NotifyPropertyChanged();
            }
        }

        public string SelectedSubclass
        {
            get { return _selectedSubclass; }
            set
            {
                _selectedSubclass = value;
                NotifyPropertyChanged();
            }
        }

        public IEnumerable<IFeature> Features
        {
            get { return _features; }
            set
            {
                _features = value;
                NotifyPropertyChanged();
            }
        }

        void SelectionChanged(object sender, PropertyChangedEventArgs e)
        {
            if (e.PropertyName == "SelectedClass")
            {
                Subclasses = _factory.ExtractSubclassesFor(_selectedClass);
                SelectedSubclass = Subclasses.First();
                NotifyPropertyChanged("Subclasses");
            }
            else if (e.PropertyName == "SelectedSubclass")
            {
                Features = _factory.ExtractSubclassFeaturesFor(_selectedSubclass);
            }
        }

    }
}