using AbstractDevelop.Machines;
using AbstractDevelop.Machines.Testing;
using AbstractDevelop.Storage.Formats;
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
    /// Логика взаимодействия для TestSystemWindow.xaml
    /// </summary>
    public partial class TestSystemWindow : Window
    {
        static JsonFormat format = new JsonFormat();

        public RiscMachine Machine { get; set; }

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set { SetValue(FilePathProperty, value); }
        }

        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(TestSystemWindow), new PropertyMetadata(string.Empty));



        public RiscTestSystem TestSystem
        {
            get { return (RiscTestSystem)GetValue(TestsProperty); }
            set { SetValue(TestsProperty, value); }
        }

        public static readonly DependencyProperty TestsProperty =
            DependencyProperty.Register("TestSystem", typeof(RiscTestSystem), typeof(TestSystemWindow), new PropertyMetadata(null));


        public static readonly DependencyProperty MachineProperty =
            DependencyProperty.Register("Machine", typeof(RiscMachine), typeof(TestSystemWindow), new PropertyMetadata(null));


        public TestSystemWindow(RiscMachine machine)
        {
            Machine = machine;
            InitializeComponent();
        }

        private void RunCommandExecuted(object sender, RoutedEventArgs e)
        {
            try
            {
                TestSystem.Run();

                if (TestSystem.Tests.All(test => test.State == TestState.Passed))
                    statsLabel.Content = Translate.Key("TestSystemPassed", Properties.Resources.ResourceManager);
                else
                    statsLabel.Content = Translate.Key("TestSystemFailed", Properties.Resources.ResourceManager, TestSystem.Tests.Count(test => test.State == TestState.Failed));
            }
            catch (Exception ex) { statsLabel.Content = Translate.Key("TestSystemError", Properties.Resources.ResourceManager, ex.Message); }
        }

        private void ExitExecuted(object sender, RoutedEventArgs e)
        {
            Close();
        }

        private void window_Loaded(object sender, RoutedEventArgs e)
        {
            Title = Translate.Key("ToolsTesting", Properties.Menu.ResourceManager);
        }

        RiscTestSystem LoadFrom(string file)
        {
            try { return new RiscTestSystem(TestSource.Load(file, format), Machine); }
            catch
            {
                MessageBox.Show(Translate.Key("TestLoadingException", Properties.Resources.ResourceManager));
                return default(RiscTestSystem);
            }
        }

        void Button_Click(object sender, RoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = Translate.Key("TestFileFilter", Properties.Resources.ResourceManager);

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = dlg.FileName.Replace(Environment.CurrentDirectory + "\\", "");
                TestSystem = LoadFrom(FilePath);
            }
        }
    }
}
