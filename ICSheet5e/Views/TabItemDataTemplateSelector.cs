using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Controls;
using System.Windows;

namespace ICSheet5e.Views {

    public class TabItemDataTemplateSelector: DataTemplateSelector
    {
        public override DataTemplate SelectTemplate(object item, DependencyObject container)
        {
            if (item == null)
            {
                return base.SelectTemplate(item, container);
            }
            ViewModels.BaseViewModel vm = item as ViewModels.BaseViewModel;
            Window window = Application.Current.MainWindow;

            if ( vm as ViewModels.NoCharacterViewModel != null)
            {
                return window.FindResource("NoCharacterTemplate") as DataTemplate;
            }
            else if ( vm as ViewModels.NewCharacterViewModel != null) {
                return window.FindResource("NewCharacterTemplate") as DataTemplate;
            }
            else if (vm as ViewModels.CharacterViewModel != null)
            {
                return window.FindResource("CharacterTemplate") as DataTemplate;
            }
            else
            {
                return base.SelectTemplate(item, container);
            }
        }
    }
}

