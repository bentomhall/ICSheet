using System.Windows;

namespace ICSheet5e.Views
{
    /// <summary>
    /// Interaction logic for AddNewFeatureWindow.xaml
    /// </summary>
    public partial class AddNewFeatureWindow : Window
    {
        public AddNewFeatureWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            DialogResult = true;

        }
    }
}
