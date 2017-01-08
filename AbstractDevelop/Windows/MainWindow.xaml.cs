using AbstractDevelop.Controls;
using AbstractDevelop.Debug.BreakPoints;
using AbstractDevelop.Machines;
using AbstractDevelop.Projects;
using AbstractDevelop.Properties;
using AbstractDevelop.Storage.Formats;
using AbstractDevelop.Translation;
using Gu.Localization;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Globalization;
using System.IO;
using System.Threading;
using System.Windows;

namespace AbstractDevelop
{
    /// <summary>
    /// Логика взаимодействия для MainWindow.xaml
    /// </summary>
    public partial class MainWindow : Window
    {
        #region [Свойства и Поля]

        const string
            DefaultProjectTitle = "Untitled",
            ProjectExtension = ".adp";

        public ObservableCollection<DebugEntry> DebugMessages { get; } =
            new ObservableCollection<DebugEntry>();

        public static MainWindow Instance { get; private set; }

        public string FilePath
        {
            get { return (string)GetValue(FilePathProperty); }
            set
            {
                SetValue(FilePathProperty, value.Replace(Environment.CurrentDirectory + '\\', ""));
                Title = $"{PlainTitle} - {FilePath}";
            }
        }

        public RiscMachine Machine
        {
            get { return (RiscMachine)GetValue(MachineProperty); }
            set { SetValue(MachineProperty, value); }
        }

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

        public bool StepMode { get; private set; }

        public static readonly DependencyProperty 
            FilePathProperty = DependencyProperty.Register("FilePath", typeof(string), typeof(MainWindow), new PropertyMetadata($"{DefaultProjectTitle}{ProjectExtension}")),
            MachineProperty  = DependencyProperty.Register("Machine", typeof(RiscMachine), typeof(MainWindow), new PropertyMetadata(null)),
            PlatformProperty = DependencyProperty.Register("Platform", typeof(RiscPlatform), typeof(MainWindow), new PropertyMetadata(null)),
            ProjectProperty  = DependencyProperty.Register("Project", typeof(AbstractProject), typeof(MainWindow), new PropertyMetadata(null));
       
        static JsonFormat jsonFormat = new JsonFormat();

        static string PlainTitle;
        Window schemaWindow, settingsWindow, testSystemWindow, helpWindow, aboutWindow;

        #endregion

        #region [Методы]

        #region [Команды]

        private void ClearAllCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            ClearDebugCommandExecuted(sender, e);

            Machine.Memory.RestoreValues(new byte[Machine.Memory.Count]);
            Machine.Registers.RestoreValues(new byte[Machine.Registers.Count]);
        }

        private void ClearDebugCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = DebugMessages.Count > 0;
        }

        private void ClearDebugCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            DebugMessages.Clear();
        }

        private void CloseCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Close();
        }

        private void CopyCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(codeEditor?.Component.SelectedText);
        }

        private void CopyCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            codeEditor.Component.Copy();
        }

        private void CustomizeLayoutCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            memoryRow.Height = new GridLength(memEditorSetting.IsChecked ? 1 : 0, GridUnitType.Star);
            debugRow.Height = new GridLength(outputSetting.IsChecked ? 1 : 0, GridUnitType.Star);

            registerColumn.Width = new GridLength(regEditorSetting.IsChecked ? 1 : 0, GridUnitType.Star);
            codeColumn.Width = new GridLength(codeEditorSetting.IsChecked ? 1 : 0, GridUnitType.Star);

            codeEditor.Visibility = codeEditorSetting.IsChecked ? Visibility.Visible : Visibility.Collapsed;
        }

        private void CutCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(codeEditor?.Component.SelectedText);
        }

        private void CutCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            codeEditor.Component.Cut();
        }

        private void DebugBreakpointCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = !string.IsNullOrEmpty(codeEditor.CurrentLine.MarginText);
        }

        void PlaceBreakpoint(int index)
            => Machine.BreakPoints.Add(new ActionBreakPoint(index, () => Machine.Instructions.CurrentIndex ?? -1));

        private void DebugBreakpointCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (int.TryParse(codeEditor.CurrentLine.MarginText, out var index))
            {
                index--;

                if (Machine.BreakPoints.Any(bp => bp is ActionBreakPoint actionBP && actionBP.ActionIndex == index, out var value))
                    Machine.BreakPoints.Remove(value);
                else
                    PlaceBreakpoint(index);

                codeEditor.ToggleBreakPoint(codeEditor.CurrentLine);
            }
        }

        private void DebugCommandStop(object sender, RoutedEventArgs e)
        {
            Machine.Activate();
            Machine.Stop();
        }

        private void DebugPauseCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Machine?.IsActive ?? false;
        }

        private void DebugPauseCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            Machine.Stop(StopReason.BreakPoint);
            codeEditor.ExecutionLine = Machine.Instructions.CurrentIndex ?? 0;
        }

        private void DebugStartStopCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = codeEditor?.InstructionCount > 0;
        }

        private void DebugStartStopCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            // трансляция кода
            if (Machine.Instructions.CurrentIndex.HasValue || TranslateInstructions())
            {
                codeEditor.ExecutionLine = -1;
                Machine.Start();
            }
        }

        private void DebugStepCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = codeEditor?.InstructionCount > 0;
        }

        private void DebugStepCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            if (Machine.Instructions.Count == 0)
                TranslateInstructions();

            if (codeEditor.ExecutionLine == -1)
                Machine.AccessTimer = 0;

            menuDebugStop.Visibility = toolbarDebugStop.Visibility = Visibility.Visible;
            Machine.Step(breakpointsActive: false);
        }

        private void FileCloseCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Machine != null;
        }

        private void FileCloseCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            CheckUnsavedData(sender, e);

            Machine = null;
            FilePath = null;
        }

        void formatShowPrefix_Click(object sender, RoutedEventArgs e)
        {
            Settings.Default.ShowFormatPrefixes = formatShowPrefix.IsChecked;
            UpdateMemory();
        }

        void HelpCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            (helpWindow = (helpWindow ?? new HelpWindow())).Closed += (x, y) => { helpWindow = null; };
            helpWindow.Show();
        }

        private void NewCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            CheckUnsavedData(sender, e);

            Machine = CreateMachine();
            FilePath = "Untitled";
        }

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

                codeEditor.Text = DeserializeResource<string>("code").Do(codeEditor.Refresh);
                (codeEditor.BreakPoints = DeserializeResource<List<int>>("breakpoints")).Apply(PlaceBreakpoint);

                // загрузка данных
                DeserializeResource<byte[]>("memory", Machine.Memory.RestoreValues);
                DeserializeResource<byte[]>("registers", Machine.Registers.RestoreValues);
            }
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

        void OpenSchemaMenuCommand(object sender, RoutedEventArgs e)
        {
            (schemaWindow = (schemaWindow ?? new SchemaWindow())).Closed += (x, y) => { schemaWindow = null; };
            schemaWindow.Show();
        }

        void OpenTestSystemMenuCommand(object sender, RoutedEventArgs e)
        {
            if (TranslateInstructions())
            {
                (testSystemWindow = (testSystemWindow ?? new TestSystemWindow(Machine))).Closed += (x, y) => { testSystemWindow = null; };
                testSystemWindow.Show();
            }
            else MessageBox.Show(
                Translate.Key("TestSystemTranslationError", Properties.Resources.ResourceManager),
                Translate.Key("ErrorTitle", Properties.Resources.ResourceManager));
                
        }

        private void PasteCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = codeEditor?.Component.CanPaste ?? false;
        }

        private void PasteCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            codeEditor.Component.Paste();
        }

        private void RedoCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = codeEditor?.Component.CanRedo ?? false;
        }

        private void RedoCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            codeEditor.Component.Redo();
        }

        private void SaveAsCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            SaveCanExecute(sender, e);
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

        private void SaveCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = Machine != null;
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

        void SettingsCommandExecuted(object sender, RoutedEventArgs e)
        {
            (settingsWindow = (settingsWindow ?? new SettingsWindow())).Closed += (x, y) => { settingsWindow = null; };
            settingsWindow.Show();
        }

        private void UndoCanExecute(object sender, System.Windows.Input.CanExecuteRoutedEventArgs e)
        {
            e.CanExecute = codeEditor?.Component.CanUndo ?? false;
        }

        private void UndoCommandExecuted(object sender, System.Windows.Input.ExecutedRoutedEventArgs e)
        {
            codeEditor.Component.Undo();
        }

        #endregion

        protected override void OnClosing(CancelEventArgs e)
        {
            // TODO: проверки на несохраненные данные
            Settings.Default.Save();
            base.OnClosing(e);
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
       
        RiscMachine CreateMachine(AbstractProject source = null)
            => (Platform.CreateMachine(Project = (source ?? CreateProject(DefaultProjectTitle).Do(project => FilePath = $"{project.Title}.adp"))) as RiscMachine).
                // подписка на события
                Do(machine =>
                {
                    machine.ReadInput = ReadQuery;
                    machine.WriteOutput = WriteQuery;

                    machine.Started += (o, e) =>
                    {
                        if (!Machine.Instructions.CurrentIndex.HasValue)
                        {
                            menuDebugStart.Visibility = toolbarDebugStart.Visibility = Visibility.Collapsed;
                            menuDebugStop.Visibility = toolbarDebugStop.Visibility = Visibility.Visible;

                            DebugMessages.Add(new DebugEntry(Translate.Key("MachineStarted", Properties.Resources.ResourceManager)));

                            codeEditor.IsReadonly = true;
                        }
                    };

                    machine.Stopped += (o, reason) =>
                    {
                        menuDebugStart.Visibility = toolbarDebugStart.Visibility = Visibility.Visible;
                        if (reason != StopReason.BreakPoint)
                        {
                            menuDebugStop.Visibility = toolbarDebugStop.Visibility = Visibility.Collapsed;
                            DebugMessages.Add(new DebugEntry(Translate.Key("MachineStopped", Properties.Resources.ResourceManager, machine.AccessTimer)));

                            codeEditor.ExecutionLine = -1;
                            codeEditor.IsReadonly = false;
                        }
                    };

                    machine.BreakPointReached += (o, e) =>
                    {
                        menuDebugStart.Visibility = toolbarDebugStart.Visibility = Visibility.Visible;
                        codeEditor.ExecutionLine = machine.Instructions.CurrentIndex ?? 0;
                    };

                    machine.BeforeStep += (o, e) =>
                    {
                        if (StepMode) codeEditor.ExecutionLine = machine.Instructions.CurrentIndex ?? 0;
                    };
                });

        AbstractProject CreateProject(string title)
            => new AbstractProject(title, Platform, jsonFormat);

        T DeserializeResource<T>(string name)
            => OpenOrCreate($@"res\{name}").Deserialize<T>(jsonFormat);

        void DeserializeResource<T>(string name, Action<T> reciever)
            => reciever(DeserializeResource<T>(name));

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

        ProjectFile OpenOrCreate(string path)
            => (Project[path] = (Project[path] as ProjectFile) ?? new ProjectFile()) as ProjectFile;
      
        DataReference ReadQuery()
        {
            var dialog = new InputDialog();
            return new ValueReference(Machine, (dialog.ShowDialog() ?? false) ? dialog.Value : (byte)0);
        }

        void PrintError(Exception ex)
        {
            if (ex is AggregateException aggregate)
                aggregate.InnerExceptions.Apply(PrintError);
            else
                DebugMessages.Add(new DebugEntry(ex.Message, (ex as RiscMachine.Exceptions.ILineException)?.Line + 1, DebugEntryType.Error));
        }

        void ClearErrors()
        {
            for (int i = 0; i < DebugMessages.Count; i++)
            {
                if (DebugMessages[i].Type == DebugEntryType.Error)
                {
                    DebugMessages.RemoveAt(i);
                    i--;
                }
            }
        }

        void PrintErrors(IEnumerable<Exception> exceptions)
        {
            // удаление старых сообщений об ошибках
            ClearErrors();
            exceptions.Apply(PrintError);
        }

        bool TranslateInstructions()
        {
            Machine.Instructions.Clear();
            return (Machine.Translator.Translate(codeEditor.Lines, out var instructions) && instructions.Try(Machine.Instructions.Load)).
                Decision(ClearErrors, () => PrintErrors(Machine.Translator.State.Exceptions));
        }

        void UpdateMemory()
        {
            foreach (var entry in Machine.Memory)
                entry.OnPropertyChanged("Value");

            foreach (var entry in Machine.Registers)
                entry.OnPropertyChanged("Value");

            registers.Columns[1].Header = Settings.Default.NumberFormat;
        }

        void WriteQuery(DataReference reference)
            => DebugMessages.Add(new DebugEntry(reference.Value.ToString()));

        #endregion

        #region [Конструкторы и деструкторы]

        public MainWindow()
        {
            PlatformService.Initialize();
            PlatformService.Add(Platform = new RiscPlatform());

            try
            {
                var culture = CultureInfo.GetCultureInfo(Settings.Default.LanguageCode == 1 ? "en" : "ru");
                Translator.Culture = culture;
                Thread.CurrentThread.CurrentUICulture = culture;
                Thread.CurrentThread.CurrentCulture = culture;
            }
            catch { Translator.Culture = Thread.CurrentThread.CurrentUICulture; }

            InitializeComponent();

            LoadSettings(Settings.Default);

            PlainTitle = Title;
            Machine = CreateMachine();

            Instance = this;
        }

        #endregion
    }
}