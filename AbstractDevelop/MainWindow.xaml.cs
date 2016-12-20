using AbstractDevelop.Machines;
using AbstractDevelop.Properties;
using System.Windows;

namespace AbstractDevelop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {

        public RiscMachine Machine
        {
            get { return (RiscMachine)GetValue(MachineProperty); }
            set { SetValue(MachineProperty, value); }
        }


        public static readonly DependencyProperty MachineProperty =
            DependencyProperty.Register("Machine", typeof(RiscMachine), typeof(MainWindow), new PropertyMetadata(null));

        public MainWindow()
        {
            Machine = new RiscMachine(256, 8);

            //(GetValue(MachineProperty) as RiscMachine).Registers.CollectionChanged += (o, e)


            InitializeComponent();
        }

        private void OpenSchemaMenuCommand(object sender, RoutedEventArgs e)
        {

        }

        private void OpenSamplesMenuCommand(object sender, RoutedEventArgs e)
        {

        }
        private void OpenHelpAboutMenuCommand(object sender, RoutedEventArgs e)
        {

        }

        private void OpenTestSystemMenuCommand(object sender, RoutedEventArgs e)
        {

        }

        private void NewCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void FileCloseCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void OpenCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void SaveCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void SaveAsCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void CloseCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void UndoCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void RedoCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void CopyCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void CutCopyExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void PasteCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void CustomizeLayoutCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        void UpdateMemory()
        {
            foreach (var entry in Machine.Memory)
                entry.OnPropertyChanged("Value");
        }

        void SelectFormatCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (binaryFormat.IsChecked ?? false)
                Settings.Default.NumberFormat = NumberFormat.Binary;
            if (octalFormat.IsChecked ?? false)
                Settings.Default.NumberFormat = NumberFormat.Octal;
            if (hexFormat.IsChecked ?? false)
                Settings.Default.NumberFormat = NumberFormat.Hex;

            UpdateMemory();
        }

        void formatShowPrefix_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ShowFormatPrefixes = formatShowPrefix.IsChecked;
            UpdateMemory();
        }

        private void ClearAllCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void ClearDebugCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void DebugStartStopCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void DebugStepCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void DebugPauseCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void HelpCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void SaveCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {

        }

        private void FileCloseCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {

        }

        private void UndoCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {

        }

        private void RedoCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {

        }

        private void CopyCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {

        }

        private void CutCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {

        }

        private void PasteCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {

        }

        private void ClearDebugCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {

        }

        private void DebugStepCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {

        }

        private void DebugPauseCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {

        }

        private void DebugBreakpointCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {

        }

        private void DebugBreakpointCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {

        }

     
    }
}
