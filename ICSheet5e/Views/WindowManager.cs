using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using ICSheet5e.ViewModels;

namespace ICSheet5e.Views
{

    public static class WindowManager
    {

        public static Window MainWindow
        {
            get { return Application.Current.MainWindow; }
        }

        public static void OpenSRD()
        {
            var wndow = new SRDViewWindow();
            wndow.Show();
            return;
        }

        public static void OpenSubclassCreationWindow(IViewModel vm)
        {
            var window = new CreateSubclassWindow();
            window.DataContext = vm;
            window.Show();
        }

        public static string SelectExistingFile()
        {
            Microsoft.Win32.OpenFileDialog dlg = new Microsoft.Win32.OpenFileDialog();
            dlg.FileName = "Character";
            dlg.DefaultExt = ".dnd5e";
            dlg.Filter = "5th Edition Character Sheets (.dnd5e)|*.dnd5e";

            var result = dlg.ShowDialog();
            if (result == true)
            {
                return dlg.FileName;
            }
            else
            {
                return null;
            }
        }

        public static string SelectSaveLocation()
        {
            Microsoft.Win32.SaveFileDialog dlg = new Microsoft.Win32.SaveFileDialog();
            dlg.FileName = "";
            dlg.DefaultExt = ".dnd5e";
            dlg.Filter = "5th Edition Character Sheets (.dnd5e)|*.dnd5e";

            var result = dlg.ShowDialog();
            if (result == true)
            {
                return dlg.FileName;
            }
            else
            {
                return null;
            }
        }
 
    }
}
