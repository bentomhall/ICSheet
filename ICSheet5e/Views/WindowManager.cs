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

        public static System.Windows.Window MainWindow
        {
            get { return Application.Current.MainWindow; }
        }

        public enum DialogType
        {
            HealthDialog,
            LevelUpDialog,
            AddNewSpellsDialog,
            AddNewFeatureDialog,
            AddSubclassDialog
        }

        public static void DisplayDialog(DialogType type, IViewModel model, Action<IViewModel> completionHandler)
        {
            Window dlg;
            switch (type)
            {
                case DialogType.HealthDialog:
                    dlg = new HealthChangeWindow();
                    break;
                case DialogType.LevelUpDialog:
                    dlg = new LevelUpWindow();
                    break;
                case DialogType.AddNewSpellsDialog:
                    dlg = new AddKnownSpellWindow();
                    break;
                case DialogType.AddNewFeatureDialog:
                    dlg = new AddNewFeatureWindow();
                    break;
                case DialogType.AddSubclassDialog:
                    dlg = new AddSubClassWindow();
                    break;
                default:
                    throw new NotImplementedException("OOPs!, wrong dialog type");
            }

            dlg.Owner = MainWindow;
            dlg.DataContext = model;
            var result = dlg.ShowDialog();

            if (result == true)
            {
                completionHandler(model);
            }
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
