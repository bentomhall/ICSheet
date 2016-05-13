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
            if ((bool)(System.ComponentModel.DesignerProperties.IsInDesignModeProperty.GetMetadata(typeof(DependencyObject)).DefaultValue))
            {
                //design mode check
                return null;
            }
            if (item == null)
            {
                return null;
            }
            ViewModels.BaseViewModel vm = item as ViewModels.BaseViewModel;
            Window window = Application.Current.MainWindow;

            if ( vm is ViewModels.NoCharacterViewModel)
            {
                return window.FindResource("NoCharacterTemplate") as DataTemplate;
            }
            else if ( vm is ViewModels.NewCharacterViewModel) {
                return window.FindResource("NewCharacterTemplate") as DataTemplate;
            }
            else if (vm is ViewModels.CharacterViewModel)
            {
                return window.FindResource("CharacterTemplate") as DataTemplate;
            }
            else if (vm is ViewModels.InventoryViewModel)
            {
                return window.FindResource("InventoryTemplate") as DataTemplate;
            }
            else if (vm is ViewModels.SpellBookViewModel)
            {
                return window.FindResource("SpellTemplate") as DataTemplate;
            }
            else if (vm is ViewModels.CharacterInformationViewModel)
            {
                return window.FindResource("InformationTemplate") as DataTemplate;
            }
            else
            {

                return base.SelectTemplate(item, container);
            }
        }
    }
}

