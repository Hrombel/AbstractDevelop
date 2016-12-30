using AbstractDevelop.Machines;
using AbstractDevelop.Properties;
using AbstractDevelop.Storage.Formats;
using System.Collections.Generic;
using System.IO;
using System.Windows;
using System.ComponentModel;
using System;
using AbstractDevelop.Projects;
using System.Collections.ObjectModel;
using AbstractDevelop.Controls;
using Gu.Localization;
using System.Globalization;
using System.Threading;

namespace AbstractDevelop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        static JsonFormat jsonFormat = new JsonFormat();

        static string PlainTitle;
        static string
            DefaultProjectTitle = "Untitled",
            ProjectExtension = ".adp";

        public static readonly DependencyProperty MachineProperty =
           DependencyProperty.Register("Machine", typeof(RiscMachine), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty FilePathProperty =
            DependencyProperty.Register("FilePath", typeof(string), typeof(MainWindow), new PropertyMetadata($"{DefaultProjectTitle}{ProjectExtension}"));

        public static readonly DependencyProperty ProjectProperty =
            DependencyProperty.Register("Project", typeof(AbstractProject), typeof(MainWindow), new PropertyMetadata(null));

        public static readonly DependencyProperty PlatformProperty =
            DependencyProperty.Register("Platform", typeof(RiscPlatform), typeof(MainWindow), new PropertyMetadata(null));

        public ObservableCollection<DebugEntry> DebugMessages { get; } =
            new ObservableCollection<DebugEntry>();

        public RiscPlatform Platform
        {
            get { return (RiscPlatform)GetValue(PlatformProperty); }
            set { SetValue(PlatformProperty, value); }
        }
        public AbstractProject Project
        {
            get { return (AbstractProject)GetValue(ProjectProperty); }
            set { SetValue(ProjectProperty, value); }
        }

        public RiscMachine Machine
        {
            get { return (RiscMachine)GetValue(MachineProperty); }
            set { SetValue(MachineProperty, value); }
        }

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set
            {
                SetValue(FilePathProperty, value.Replace(Environment.CurrentDirectory + '\\', ""));
                Title = $"{PlainTitle} - {FilePath}";
            }
        }

        void LoadSettings(Settings settings)
        {
            switch (settings.NumberFormat)
            {
                case NumberFormat.Binary:
                    binaryFormat.IsChecked = true;
                    break;
                case NumberFormat.Octal:
                    octalFormat.IsChecked = true;
                    break;
                case NumberFormat.Hex:
                    hexFormat.IsChecked = true;
                    break;
            }

            formatShowPrefix.IsChecked = settings.ShowFormatPrefixes;
        }

        public MainWindow()
        {
            PlatformService.Initialize();
            PlatformService.Add(Platform = new RiscPlatform());

            try
            {
                var culture = CultureInfo.GetCultureInfo(Settings.Default.LanguageCode == 1? "en" : "ru");
                Translator.Culture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }
            catch { Translator.Culture = Thread.CurrentThread.CurrentUICulture; }

            InitializeComponent();

            LoadSettings(Settings.Default);

            PlainTitle = Title;
            Machine = CreateMachine();
        }

        AbstractProject CreateProject(string title)
            => new AbstractProject(title, Platform, jsonFormat);

        RiscMachine CreateMachine(AbstractProject source = null)
            => (Platform.CreateMachine(Project = (source ?? CreateProject(DefaultProjectTitle).Do(project => FilePath = $"{project.Title}.adp"))) as RiscMachine).
                // подписка на события
                Do(machine =>
                {
                    machine.ReadInput = ReadQuery;
                    machine.WriteOutput = WriteQuery;

                    machine.Started += (o, e) =>
                    {
                        if (Machine.Instructions.CurrentIndex < 0)
                        {
                            menuDebugStart.Visibility = toolbarDebugStart.Visibility = Visibility.Collapsed;
                            menuDebugStop.Visibility = toolbarDebugStop.Visibility = Visibility.Visible;

                            DebugMessages.Add(new DebugEntry(Translate.Key("MachineStarted", Properties.Resources.ResourceManager)));
                        }
                    };

                    machine.Stopped += (o, reason) =>
                    {
                        menuDebugStart.Visibility = toolbarDebugStart.Visibility = Visibility.Visible;
                        if (reason != AbstractMachine.StopReason.BreakPoint)
                        {
                            menuDebugStop.Visibility = toolbarDebugStop.Visibility = Visibility.Collapsed;
                            DebugMessages.Add(new DebugEntry(Translate.Key("MachineStopped", Properties.Resources.ResourceManager, machine.AccessTimer)));

                            codeEditor.ExecutionLine = -1;
                            (o as RiscMachine).AccessTimer = 0;
                        }
                    };

                    machine.BeforeStep += (o, e) =>
                    {
                        // поиск точек останова
                        if (machine.IsActive && codeEditor.BreakPoints.Contains(machine.Instructions.CurrentIndex))
                            machine.Stop(AbstractMachine.StopReason.BreakPoint);
                       
                        if (!machine.IsActive)
                            codeEditor.ExecutionLine = machine.Instructions.CurrentIndex;
                    };

                    machine.BreakPointReached += (o, e) =>
                    {
                        menuDebugStart.Visibility = toolbarDebugStart.Visibility = Visibility.Visible;
                    };
                });

        DataReference ReadQuery()
        {
            var dialog = new InputDialog();
            return new ValueReference(Machine, (dialog.ShowDialog() ?? false) ? dialog.Value : (byte)0);
        }

        void WriteQuery(DataReference reference)
            => DebugMessages.Add(new DebugEntry(reference.Value.ToString()));

        Window schemaWindow, settingsWindow, testSystemWindow, helpWindow, aboutWindow;

        void OpenSchemaMenuCommand(object sender, RoutedEventArgs e)
        {
            (schemaWindow = (schemaWindow ?? new SchemaWindow())).Closed += (x, y) => { schemaWindow = null; };
            schemaWindow.Show();
        }

        void SettingsCommandExecuted(object sender, RoutedEventArgs e)
        {
            (settingsWindow = (settingsWindow ?? new SettingsWindow())).Closed += (x, y) => { settingsWindow = null; };
            settingsWindow.Show();
        }

        void OpenTestSystemMenuCommand(object sender, RoutedEventArgs e)
        {
            (testSystemWindow = (testSystemWindow ?? new TestSystemWindow(Machine))).Closed += (x, y) => { testSystemWindow = null; };
            testSystemWindow.Show();
        }

        void HelpCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            (helpWindow = (helpWindow ?? new HelpWindow())).Closed += (x, y) => { helpWindow = null; };
            helpWindow.Show();
        }

        private void OpenHelpAboutMenuCommand(object sender, RoutedEventArgs e)
        {
            (aboutWindow = (aboutWindow ?? new AboutWindow())).Closed += (x, y) => { aboutWindow = null; };
            aboutWindow.Show();
        }

        private void OpenSamplesMenuCommand(object sender, RoutedEventArgs e)
        {
            MessageBox.Show(
                Translate.Key("SamplesNotFoundMessage", Properties.Resources.ResourceManager), 
                Translate.Key("ErrorTitle", Properties.Resources.ResourceManager),
                MessageBoxButton.OK, MessageBoxImage.Asterisk);
        }

        void CheckUnsavedData(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (Project.Title != DefaultProjectTitle)

                if (MessageBox.Show(
                    Translate.Key("UnsavedDataMessage", Properties.Resources.ResourceManager),
                    Translate.Key("SavingTitle", Properties.Resources.ResourceManager),
                    MessageBoxButton.YesNo) == MessageBoxResult.Yes)
                    SaveCommandExecuted(sender, e);

            ClearAllCommandExecuted(sender, e);
        }

        private void ClearAllCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            ClearDebugCommandExecuted(sender, e);

            Machine.Memory.RestoreValues(new byte[Machine.Memory.Count]);
            Machine.Registers.RestoreValues(new byte[Machine.Registers.Count]);
        }

        private void ClearDebugCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            DebugMessages.Clear();
        }

        void TranslateInstructions()
            => Machine.Instructions.Load((Machine.Translator as RiscMachine.RiscTranslator).Translate(codeEditor.Lines, null));

        private void DebugStartStopCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = codeEditor?.InstructionCount > 0;
        }

        private void DebugCommandStop(object sender, RoutedEventArgs e)
        {
            Machine.Activate();
            Machine.Stop();
        }

        private void DebugStartStopCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            // трансляция кода 
            try
            {
                if (Machine.Instructions.CurrentIndex == 0)
                    TranslateInstructions();

                codeEditor.ExecutionLine = -1;
                Machine.Start();
            }
            catch (Exception ex)
            {
                // удаление старых сообщений об ошибках
                for (int i = 0; i < DebugMessages.Count; i++)
                {
                    if (DebugMessages[i].Type == DebugEntryType.Error)
                    {
                        DebugMessages.RemoveAt(i);
                        i--;
                    }
                }

                if (ex is AggregateException aggregate)
                    // добавление новых сообщений об ошибках
                    foreach (var inner in aggregate.InnerExceptions)
                        PrintError(inner);
                else
                    PrintError(ex);
            }

            void PrintError(Exception ex)
                => DebugMessages.Add(new DebugEntry(ex.Message, (ex as RiscMachine.Exceptions.ILineException)?.Line + 1, DebugEntryType.Error));
        }

        private void DebugStepCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (Machine.Instructions.Count == 0)
                TranslateInstructions();

            if (codeEditor.ExecutionLine == -1)
                Machine.AccessTimer = 0;

            menuDebugStop.Visibility = toolbarDebugStop.Visibility = Visibility.Visible;
            Machine.Step();
        }

        private void DebugPauseCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Machine.Stop(AbstractMachine.StopReason.BreakPoint);
            codeEditor.ExecutionLine = Machine.Instructions.CurrentIndex;
        }

        private void NewCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            CheckUnsavedData(sender, e);

            Machine = CreateMachine();
            FilePath = "Untitled";
        }

        private void FileCloseCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            CheckUnsavedData(sender, e);
            
            Machine = null;
            FilePath = null;
        }

        ProjectFile OpenOrCreate(string path)
            => (Project[path] = (Project[path] as ProjectFile) ?? new ProjectFile()) as ProjectFile;

        private void OpenCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.OpenFileDialog dlg = new System.Windows.Forms.OpenFileDialog();
            dlg.Filter = Translate.Key("ProjectFileFilter", Properties.Resources.ResourceManager);

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = dlg.FileName;

                // создание ресурсов
                Project = AbstractProject.Load(FilePath, jsonFormat);
                Machine = CreateMachine(Project);

                // загрузка данных
                Machine.Memory.RestoreValues(OpenOrCreate("res\\memory").Deserialize<byte[]>(jsonFormat));
                Machine.Registers.RestoreValues(OpenOrCreate("res\\registers").Deserialize<byte[]>(jsonFormat));

                codeEditor.Text = OpenOrCreate("res\\code").Deserialize<string>(jsonFormat);
                codeEditor.BreakPoints = OpenOrCreate("res\\breakpoints").Deserialize<List<int>>(jsonFormat);
            }
        }

        private void SaveCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (string.IsNullOrEmpty(FilePath) || FilePath == DefaultProjectTitle + ProjectExtension)
                SaveAsCommandExecuted(sender, e);
            else
            {
                OpenOrCreate("res\\memory").Serialize(Machine.Memory.GetSnapshot(), jsonFormat);
                OpenOrCreate("res\\registers").Serialize(Machine.Registers.GetSnapshot(), jsonFormat);
                OpenOrCreate("res\\code").Serialize(codeEditor.Text, jsonFormat);
                OpenOrCreate("res\\breakpoints").Serialize(codeEditor.BreakPoints, jsonFormat);

                using (var stream = File.OpenWrite(FilePath))
                    Project.Save(stream, jsonFormat);
            }
        }

        private void SaveAsCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            System.Windows.Forms.SaveFileDialog dlg = new System.Windows.Forms.SaveFileDialog();
            dlg.Filter = Translate.Key("ProjectFileFilter", Properties.Resources.ResourceManager);

            if (dlg.ShowDialog() == System.Windows.Forms.DialogResult.OK)
            {
                FilePath = dlg.FileName;
                Project.Title = new FileInfo(dlg.FileName).Name.Replace(ProjectExtension, "");

                SaveCommandExecuted(sender, e);
            }
        }

        protected override void OnClosing(CancelEventArgs e)
        {
            // TODO: проверки на несохраненные данные
            Settings.Default.Save();
            base.OnClosing(e);
        }

        private void CloseCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void UndoCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            codeEditor.Component.Undo();
        }

        private void RedoCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            codeEditor.Component.Redo();
        }

        private void CopyCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            codeEditor.Component.Copy();
        }

        private void CutCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            codeEditor.Component.Cut();
        }

        private void PasteCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            codeEditor.Component.Paste();
        }

        private void CustomizeLayoutCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            memoryRow.Height = new GridLength(memEditorSetting.IsChecked ? 1 : 0, GridUnitType.Star);
            debugRow.Height = new GridLength(outputSetting.IsChecked ? 1 : 0, GridUnitType.Star);

            registerColumn.Width = new GridLength(regEditorSetting.IsChecked ? 1 : 0, GridUnitType.Star);
            codeColumn.Width = new GridLength(codeEditorSetting.IsChecked ? 1 : 0, GridUnitType.Star);

            codeEditor.Visibility = codeEditorSetting.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        void UpdateMemory()
        {
            foreach (var entry in Machine.Memory)
                entry.OnPropertyChanged("Value");

            foreach (var entry in Machine.Registers)
                entry.OnPropertyChanged("Value");

            registers.Columns[1].Header = Settings.Default.NumberFormat;
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
      
        private void SaveCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Machine != null;
        }

        private void SaveAsCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            SaveCanExecute(sender, e);
        }

        private void FileCloseCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Machine != null;
        }

        private void UndoCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = codeEditor?.Component.CanUndo ?? false;
        }

        private void RedoCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = codeEditor?.Component.CanRedo ?? false;
        }

        private void CopyCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(codeEditor?.Component.SelectedText);
        }

        private void CutCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(codeEditor?.Component.SelectedText);
        }

        private void PasteCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = codeEditor?.Component.CanPaste ?? false;
        }
     
        private void ClearDebugCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DebugMessages.Count > 0;
        }

        private void DebugStepCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = codeEditor?.InstructionCount > 0;
        }

        private void DebugPauseCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Machine?.IsActive ?? false;
        }

        private void DebugBreakpointCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            codeEditor.ToggleBreakPoint(codeEditor.CurrentLine);
        }

        private void DebugBreakpointCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(codeEditor.CurrentLine.MarginText);
        }
    }
}
