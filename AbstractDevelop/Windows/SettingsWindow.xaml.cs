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

namespace AbstractDevelop
{
    /// <summary>
    /// Логика взаимодействия для SettingsWindow.xaml
    /// </summary>
    public partial class SettingsWindow : Window
    {
        Properties.Settings Settings => Properties.Settings.Default;

        public SettingsWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = Translate.Key("Settings", Properties.Menu.ResourceManager);
            languageCombo.SelectedIndex = Settings.LanguageCode;
        }

        private void ApplyCommandExecuted(object sender, RoutedEventArgs e)
        {
            if (Settings.LanguageCode != languageCombo.SelectedIndex)
            {
                MessageBox.Show(Translate.Key("LanguageChangeWarning", Properties.Resources.ResourceManager));
                Settings.LanguageCode = languageCombo.SelectedIndex;
            }

            Settings.Save();
            Close();
        }

        private void Button_Click(object sender, RoutedEventArgs e)
        {
            Close();
        }
    }
}
