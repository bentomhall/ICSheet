using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Collections.ObjectModel;

namespace ICSheet5e.ViewModels
{
    class CharacterViewModel : INotifyPropertyChanged
    {
        private Model.Character character;
        public Model.Character Character 
        {
            get { return character; }
            set
            {
                character = value;
                NotifyPropertyChanged();
            }
        }
        

        #region INotifyPropertyChanged Implementation
        public event PropertyChangedEventHandler PropertyChanged;
        private void NotifyPropertyChanged([CallerMemberName] String propertyName = "")
        {
            if (PropertyChanged != null)
            {
                PropertyChanged(this, new PropertyChangedEventArgs(propertyName));
            }
        }
        #endregion

        public void NewCharacterHandler(object sender, EventArgs e)
        {
            character = new Model.Character();
            NotifyPropertyChanged("Character");
        }

        public void SaveCharacterHandler(object sender, EventArgs e)
        {
            Microsoft.Win32.SaveFileDialog saveDialog = new Microsoft.Win32.SaveFileDialog();
            saveDialog.FileName = character.CharacterName;
            saveDialog.DefaultExt = ".dnd5e";
            saveDialog.Filter = "5th Edition Characters (.dnd5e)|*.dnd5e";

            Nullable<bool> result = saveDialog.ShowDialog();

            if (result == true)
            {
                string filename = saveDialog.FileName;
                System.Runtime.Serialization.DataContractSerializer serializer = new System.Runtime.Serialization.DataContractSerializer(typeof(Model.Character));
                using (System.IO.FileStream writer = new System.IO.FileStream(filename, System.IO.FileMode.Create))
                {
                    serializer.WriteObject(writer, character);
                }
            }
        }


    }
}
