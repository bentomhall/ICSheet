using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.ComponentModel;
using System.Runtime.CompilerServices;

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

        }
    }
}
