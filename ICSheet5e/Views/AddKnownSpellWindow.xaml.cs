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
using System.Windows.Shapes;

namespace ICSheet5e.Views
{
    /// <summary>
    /// Interaction logic for AddKnownSpellWindow.xaml
    /// </summary>
    public partial class AddKnownSpellWindow : Window
    {
        public AddKnownSpellWindow()
        {
            InitializeComponent();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            this.DialogResult = true;
            
        }
    }
}
