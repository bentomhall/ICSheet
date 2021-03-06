﻿using System;
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
using System.Windows.Navigation;
using System.Windows.Shapes;

namespace ICSheet5e.Views
{
    /// <summary>
    /// Interaction logic for CharacterInformationView.xaml
    /// </summary>
    public partial class CharacterInformationView : UserControl
    {
        public CharacterInformationView()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            var button = sender as Button;
            var textEntry = (TextBox)FindName(button.Tag.ToString());
            button.Command.Execute(button.CommandParameter);
            textEntry.Text = "";
        }
    }
}
