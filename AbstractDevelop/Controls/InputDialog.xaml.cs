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

namespace AbstractDevelop.Controls
{
    /// <summary>
    /// Логика взаимодействия для InputDialog.xaml
    /// </summary>
    public partial class InputDialog : Window
    {
        public byte Value { get; set; }

        public InputDialog()
        { 
            InitializeComponent();
            input.Text = Value.ToString();
        }

        private void SaveValueExecuted(object sender, ExecutedRoutedEventArgs e)
        {
            try
            {
                var value = Machines.RiscMachine.GetNumberValue(input.Text);
                DialogResult = true;
            }
            catch
            {
                MessageBox.Show("Введено недопустимое значение, попробуйте ввести еще раз");
            }
        }
    }
}
