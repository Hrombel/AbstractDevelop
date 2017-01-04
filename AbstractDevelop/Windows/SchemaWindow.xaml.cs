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

namespace AbstractDevelop
{
    /// <summary>
    /// Логика взаимодействия для SchemaWindow.xaml
    /// </summary>
    public partial class SchemaWindow : Window
    {
        public SchemaWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = Translate.Key("ToolsSchema", Properties.Menu.ResourceManager);
        }
    }
}