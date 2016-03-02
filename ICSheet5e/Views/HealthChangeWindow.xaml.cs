using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Controls;
using System.Windows.Data;
using System.Windows.Documents;
using System.Windows.Input;
using System.Windows.Media;
using System.Windows.Media.Imaging;
using System.Windows.Shapes;

namespace ICSheet5e.Views
{
    /// <summary>
    /// Interaction logic for HealthChangeWindow.xaml
    /// </summary>
    public partial class HealthChangeWindow : Window
    {
        public HealthChangeWindow()
        {
            InitializeComponent();
        }

        public ICommand DoHealthChangeCommand
        {
            get { return new Views.DelegateCommand<object>(DoHealthChangeCommandExecuted); }
        }

        private void DoHealthChangeCommandExecuted(object obj)
        {
            this.DialogResult = true;
        }
    }
}
