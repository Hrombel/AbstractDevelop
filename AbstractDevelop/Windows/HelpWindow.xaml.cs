using System;
using System.Collections.Generic;
using System.IO;
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
using System.Windows.Resources;
using System.Windows.Shapes;

namespace AbstractDevelop
{
    /// <summary>
    /// Логика взаимодействия для HelpWindow.xaml
    /// </summary>
    public partial class HelpWindow : Window
    {
        public HelpWindow()
        {
            InitializeComponent();
        }

        private void Window_Loaded(object sender, RoutedEventArgs e)
        {
            var doc = document.Document = new FlowDocument();

            Uri uri = new Uri("pack://application:,,,/Resources/description.rtf");
            StreamResourceInfo info = Application.GetResourceStream(uri);

            TextRange textRange = new TextRange(doc.ContentStart, doc.ContentEnd);
            textRange.Load(info.Stream, DataFormats.Rtf);
        }
    }
}
