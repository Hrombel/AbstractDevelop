using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AbstractDevelop.machines.regmachine;
using AbstractDevelop.machines;
using AbstractDevelop.projects;
using AbstractDevelop.controls.ui.ceditor;
using System.IO;
using AbstractDevelop.controls.visuals.additionals;
using AbstractDevelop.controls.environment.debugwindow;
using AbstractDevelop.controls.ui.ceditor.editors;
using System.Numerics;
using System.Collections.ObjectModel;
using System.Text.RegularExpressions;
using AbstractDevelop.errors.dev;

namespace AbstractDevelop.controls.visuals
{
    /// <summary>
    /// Представляет средство визуализиции (параллельной) машины с бесконечными регистрами.
    /// </summary>
    public partial class RegisterMachineVisualizer : UserControl, IMachineVisualizer
    {
        /// <summary>
        /// Представляет программный модуль для ПМБР.
        /// </summary>
        private class RegisterUnit
        {
            /// <summary>
            /// Имя модуля.
            /// </summary>
            public string Name;
            /// <summary>
            /// Код модуля.
            /// </summary>
            public string Code;
        }

        /// <summary>
        /// Возникает после внесения любых изменений пользователем.
        /// </summary>
        public event EventHandler DataChanged;
        /// <summary>
        /// Возникает после сохранения всех изменений, совершенных пользователем.
        /// </summary>
        public event EventHandler DataSaved;
        /// <summary>
        /// Возникает после смены состояния.
        /// </summary>
        public event EventHandler<MachineVisualizerStateChangedEventArgs> OnStateChanged;

        private RegisterMachine _machine;
        private VisualizerState _state;
        private bool _parallelMode;
        private AbstractProject _project;
        private List<RegisterUnit> _units;

        public RegisterMachineVisualizer()
        {
            _state = VisualizerState.Stopped;
            _parallelMode = false;
            _units = new List<RegisterUnit>();
            InitializeComponent();
            threadFrame.Enabled = false;

            createUnitItem.Visible = false;
            deleteUnitItem.Visible = false;

            renameUnitItem.Click += renameUnitItem_Click;
            tabContext.Opening += tabContext_Opening;
            registersVis.StateChanged += DataChangedHandler;
            threadFrame.SelectionChanged += threadFrame_SelectionChanged;
        }
        ~RegisterMachineVisualizer()
        {
            threadFrame.SelectionChanged -= threadFrame_SelectionChanged;
            createUnitItem.Click -= createUnitItem_Click;
            renameUnitItem.Click -= renameUnitItem_Click;
            deleteUnitItem.Click -= deleteUnitItem_Click;
            tabContext.Opening -= tabContext_Opening;

            registersVis.StateChanged -= DataChangedHandler;
        }

        private void threadFrame_SelectionChanged(object sender, EventArgs e)
        {
            UpdateCommandMarkers();
        }

        private void tabContext_Opening(object sender, CancelEventArgs e)
        {
            Point p = tabCtrl.PointToClient(MousePosition);
            for (int i = 0; i < tabCtrl.TabCount; i++)
            {
                Rectangle r = tabCtrl.GetTabRect(i);
                if (r.Contains(p))
                {
                    tabCtrl.SelectedIndex = i;
                    return;
                }
            }
            e.Cancel = true;
        }

        private void renameUnitItem_Click(object sender, EventArgs e)
        {
            TextInputControl inputCtrl = new TextInputControl();
            inputCtrl.Size = inputCtrl.MinimumSize;
            inputCtrl.RegExp = @"[a-zA-Z\d]+";
            inputCtrl.InputText = tabCtrl.SelectedTab.Text;
            inputCtrl.OKPressed += inputCtrl_OKPressed;

            Form form = new Form();
            form.Text = "Переименование модуля";
            form.Controls.Add(inputCtrl);
            inputCtrl.Dock = DockStyle.Fill;
            form.ClientSize = inputCtrl.MinimumSize;
            form.MinimumSize = new Size(form.Width, form.Height);
            form.MaximumSize = new Size(inputCtrl.MaximumSize.Width, form.Height);
            form.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            form.ShowDialog();
            inputCtrl.OKPressed -= inputCtrl_OKPressed;
        }

        private void inputCtrl_OKPressed(object sender, EventArgs e)
        {
            TextInputControl ctrl = sender as TextInputControl;

            if (GetUnitIndexByName(ctrl.InputText) == -1)
            {
                RenameUnit(tabCtrl.SelectedTab.Text, ctrl.InputText);
                SaveUnit(ctrl.InputText);
                _project.Save();
                ctrl.ParentForm.Close();
                ctrl.Dispose();
            }
            else if(tabCtrl.SelectedTab.Text == ctrl.InputText)
            {
                ctrl.ParentForm.Close();
                ctrl.Dispose();
            }
            else
                MessageBox.Show("Модуль с таким названием уже существует. Пожалуйста, придумайте другое название");
        }

        private void deleteUnitItem_Click(object sender, EventArgs e)
        {
            DeleteUnit((tabCtrl.SelectedTab.Tag as RegisterUnit).Name);
            if(_units.Count == 0)
            {
                CreateUnit("Untitled");
                SaveUnit(_units[0].Name);
            }
            _project.Save();
        }

        /// <summary>
        /// Возвращает модуль по его имени.
        /// </summary>
        /// <param name="name">Имя модуля.</param>
        /// <returns>Модуль с указанным именем.</returns>
        private RegisterUnit GetUnitByName(string name)
        {
            return _units[GetUnitIndexByName(name)];
        }

        /// <summary>
        /// Возвращает вкладку по заданному имени модуля.
        /// </summary>
        /// <param name="unitName">Имя модуля.</param>
        /// <returns>Вкладка, содержащая код модуля.</returns>
        private TabPage GetTabPageByUnitName(string unitName)
        {
            int n = tabCtrl.TabPages.Count;
            for (int i = 0; i < n; i++)
            {
                if (tabCtrl.TabPages[i].Text == unitName)
                {
                    return tabCtrl.TabPages[i];
                }
            }

            return null;
        }

        /// <summary>
        /// Удаляет указанный модуль.
        /// </summary>
        /// <param name="name">Имя удаляемого модуля.</param>
        private void DeleteUnit(string name)
        {
            CloseUnitTab(name);
            if(_project.FileExists(name + ".rmc"))
                _project.RemoveFile(name + ".rmc");
            _units.RemoveAt(GetUnitIndexByName(name));
        }

        /// <summary>
        /// Получает индекс модуля по его имени.
        /// </summary>
        /// <param name="name">Имя модуля.</param>
        /// <returns>Индекс модуля или -1.</returns>
        private int GetUnitIndexByName(string name)
        {
            return _units.FindIndex(x => x.Name == name);
        }

        /// <summary>
        /// Переименовывает программный модуль.
        /// </summary>
        /// <param name="oldName">Текущее имя модуля.</param>
        /// <param name="newName">Новое имя модуля.</param>
        private void RenameUnit(string oldName, string newName)
        {
            RegisterUnit unit = _units.Find(x => x.Name == oldName);
            TabPage page = GetTabPageByUnitName(oldName);

            unit.Name = newName;
            page.Text = newName;
            if(_project.FileExists(oldName + ".rmc"))
            _project.RemoveFile(oldName + ".rmc");
        }

        private void createUnitItem_Click(object sender, EventArgs e)
        {
            string name = "Untitled";
            RegisterUnit current = _units.Find(x => x.Name == "Untitled");
            if(current == null)
                CreateUnit("Untitled");
            else
            {
                int i = 1;
                do
                {
                    name = "Untitled" + i;
                    current = _units.Find(x => x.Name == name);
                    i++;

                }while(current != null);
                CreateUnit(name);
            }
            SaveUnit(name);
            _project.Save();
        }


        public DebugWindow Debug { get; set; }

        /// <summary>
        /// Получает или задает текущий проект для МБР.
        /// </summary>
        public AbstractProject CurrentProject
        {
            get
            {
                return _project;
            }
            set
            {
                if(value != null)
                {
                    if (value.Machine != MachineId.Register)
                        throw new Exception("Указан проект, предназначенный не для МБР");
                }
                _project = value;
                if(_project != null)
                {
                    LoadProjectUnits();
                    CurrentMachine = GetMachineState();
                    ParallelMode = _project.VisualizerSettings["parallel"];
                    if (_units.Count == 0)
                    {
                        CreateUnit("Untitled");
                        SaveUnit(_units[0].Name);
                        _project.Save();
                    }
                    else
                    {
                        if (ParallelMode)
                            _units.ForEach(x => OpenUnitTab(x));
                        else
                        {
                            OpenUnitTab(_units[0]);
                            if (_units.Count > 1)
                            {
                                while (_units.Count > 1)
                                    DeleteUnit(_units[_units.Count - 1].Name);
                            }
                        }
                    }
                }
            }
        }

        /// <summary>
        /// Загружает все программные модули, связанные с проектом, и заносит их в список модулей.
        /// </summary>
        /// <returns>Массив загруженных программных модулей.</returns>
        private void LoadProjectUnits()
        {
            string ext = ".rmc";
            string[] files = Array.FindAll(_project.GetFiles(), x => x.Substring(x.Length - ext.Length) == ext);

            List<RegisterUnit> units = new List<RegisterUnit>();
            RegisterUnit unit;
            foreach(string file in files)
            {
                unit = new RegisterUnit()
                {
                    Name = Path.GetFileNameWithoutExtension(file),
                    Code = _project.LoadString(file)
                };
                units.Add(unit);
            }
            
            _units = units;
        }

        /// <summary>
        /// Сохраняет указанный программный модуль.
        /// </summary>
        /// <param name="name">Имя сохраняемого модуля.</param>
        private void SaveUnit(string name)
        {
            CodeEditor editor = GetEditorByUnitName(name);

            _project.SaveFile(name + ".rmc", editor.Text);
            GetUnitByName(name).Code = editor.Text;
        }

        /// <summary>
        /// Создает новый программный модуль с указанным именем и открывает его во вкладке.
        /// </summary>
        /// <param name="name">Имя создаваемого модуля.</param>
        private void CreateUnit(string name)
        {
            RegisterUnit unit = new RegisterUnit() { Name = name, Code = "" };
            _units.Add(unit);
            OpenUnitTab(unit);
        }

        /// <summary>
        /// Открывает указанный программный модуль во вкладке.
        /// </summary>
        /// <param name="unit">Открываемый программный модуль.</param>
        private void OpenUnitTab(RegisterUnit unit)
        {
            TabPage page = new TabPage(unit.Name);
            page.Tag = unit;

            RegisterCodeEditor editor = new RegisterCodeEditor();
            editor.IsParallel = _parallelMode;
            editor.Text = unit.Code;
            page.Controls.Add(editor);
            editor.Dock = DockStyle.Fill;
            editor.TextChanged += DataChangedHandler;
            editor.BreakPointToggled += BreakPointToggled;

            tabCtrl.TabPages.Add(page);
        }

        private void BreakPointToggled(object sender, EventArgs e)
        {
            if (_state == VisualizerState.Stopped) return;

            _machine.BreakPoints = CollectAllBreakPoints();
        }

        /// <summary>
        /// Генерирует событие о том, что пользователь внес изменения.
        /// </summary>
        private void InvalidateData()
        {
            if (DataChanged != null)
                DataChanged(this, EventArgs.Empty);
        }

        private void DataChangedHandler(object sender, EventArgs e)
        {
            InvalidateData();
        }

        /// <summary>
        /// Возвращает редактор кода, сопоставленный с указанным программным модулем.
        /// </summary>
        /// <param name="name">Имя модуля.</param>
        /// <returns>Редактор кода.</returns>
        private RegisterCodeEditor GetEditorByUnitName(string name)
        {
            return GetTabPageByUnitName(name).Controls[0] as RegisterCodeEditor;
        }

        /// <summary>
        /// Закрывает вкладку, в которой открыт модуль с указанным именем.
        /// </summary>
        /// <param name="name">Имя модуля.</param>
        private void CloseUnitTab(string name)
        {
            int n = tabCtrl.TabPages.Count;
            for(int i = 0; i < n; i++)
            {
                if(tabCtrl.TabPages[i].Text == name)
                {
                    (tabCtrl.TabPages[i].Controls[0] as CodeEditor).BreakPointToggled -= BreakPointToggled;
                    (tabCtrl.TabPages[i].Controls[0] as CodeEditor).TextChanged -= DataChangedHandler;
                    tabCtrl.TabPages.RemoveAt(i);
                    return;
                }
            }

            throw new ArgumentException("Вкладка с указанным именем не найдена");
        }

        /// <summary>
        /// Определяет режим запуска МБР. Истина - как параллельная, ложь - как классическая.
        /// </summary>
        public bool ParallelMode
        {
            get { return _parallelMode; }
            set
            {
                if (value == _parallelMode) return;

                if(_parallelMode)
                {
                    createUnitItem.Click -= createUnitItem_Click;
                    deleteUnitItem.Click -= deleteUnitItem_Click;
                    createUnitItem.Visible = false;
                    deleteUnitItem.Visible = false;
                }
                else
                {
                    createUnitItem.Click += createUnitItem_Click;
                    deleteUnitItem.Click += deleteUnitItem_Click;
                    createUnitItem.Visible = true;
                    deleteUnitItem.Visible = true;
                }
                _parallelMode = value;
            }
        }

        /// <summary>
        /// Получает или задет текущую визуализируемую машину с бесконечными регистрами.
        /// </summary>
        private RegisterMachine CurrentMachine
        {
            get { return _machine; }
            set
            {
                if(_machine != null)
                {
                    _machine.StepCompleted -= _machine_StepCompleted;
                    _machine.BreakPointReached -= _machine_BreakPointReached;
                    _machine.ValueIn -= _machine_ValueIn;
                    _machine.ValueOut -= _machine_ValueOut;
                    _machine.OnMachineStopped -= _machine_OnMachineStopped;
                    registersVis.Registers = null;
                }
                _machine = value;
                if(_machine != null)
                {
                    registersVis.Registers = _machine.Registers;
                    _machine.OnMachineStopped += _machine_OnMachineStopped;
                    _machine.ValueOut += _machine_ValueOut;
                    _machine.ValueIn += _machine_ValueIn;
                    _machine.BreakPointReached += _machine_BreakPointReached;
                    _machine.StepCompleted += _machine_StepCompleted;
                }
            }
        }

        private void _machine_StepCompleted(object sender, EventArgs e)
        {
            UpdateThreadsInfo();
        }

        private void _machine_BreakPointReached(object sender, EventArgs e)
        {
            UpdateThreadsInfo();
            threadFrame.Enabled = true;
            ChangeState(VisualizerState.Paused);
        }

        private void _machine_ValueIn(object sender, RegisterMachineValueInEventArgs e)
        {
            bool dialogOk = false;

            TextInputControl inputCtrl = new TextInputControl();
            inputCtrl.Size = inputCtrl.MinimumSize;
            inputCtrl.RegExp = @"\d+";
            inputCtrl.InputText = "0";
            inputCtrl.OKPressed += (x, a) => { 
                        (x as TextInputControl).ParentForm.Close();
                        dialogOk = true;
           };

            Form form = new Form();
            form.Text = "Запрос ввода значения от ПМБР";
            form.Controls.Add(inputCtrl);
            inputCtrl.Dock = DockStyle.Fill;
            form.ClientSize = inputCtrl.MinimumSize;
            form.MinimumSize = new Size(form.Width, form.Height);
            form.MaximumSize = new Size(inputCtrl.MaximumSize.Width, form.Height);
            form.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            DialogResult res = form.ShowDialog();

            if(dialogOk)
                e.Value = BigInteger.Parse(inputCtrl.InputText);
        }

        private void _machine_ValueOut(object sender, RegisterMachineValueOutEventArgs e)
        {
            Array.ForEach<BigInteger>(e.Buffer, x => Debug.AddMessage(_project.Name + " - МБР", string.Format("Результат: {0}", x)));
        }

        /// <summary>
        /// Получает код проекта.
        /// </summary>
        /// <returns>Строка с загруженным кодом.</returns>
        private string GetProjectCode()
        {
            string name = _project.Name + "Code.rmc";

            return _project.FileExists(name) ? _project.LoadString(name) : "";
        }

        /// <summary>
        /// Получает состояние машины проекта.
        /// </summary>
        /// <returns>Состояние машины.</returns>
        private RegisterMachine GetMachineState()
        {
            string name = _project.Name + "Machine.rms";

            return _project.FileExists(name) ? _project.LoadObject(name) as RegisterMachine : new RegisterMachine();
        }

        /// <summary>
        /// Сохраняет состояние машины проекта.
        /// </summary>
        private void SaveMachineState()
        {
            _project.SaveFile(_project.Name + "Machine.rms", CurrentMachine);
        }

        private void _machine_OnMachineStopped(object sender, RegisterMachineStoppedEventArgs e)
        {
            ChangeState(VisualizerState.Stopped);
            if (e.RegistersChanged) InvalidateData();
            threadFrame.Clear();
            threadFrame.Enabled = false;
            RemoveCommandMarkers();
            SwitchEditorsInput(true);
            if (Debug != null) Debug.AddMessage(_project.Name + " - МБР", "Машина остановлена");
        }

        /// <summary>
        /// Сохраняет все программные модули.
        /// </summary>
        private void SaveAllUnits()
        {
            _units.ForEach(x => SaveUnit(x.Name));
        }

        /// <summary>
        /// Получает состояние, в котором находится визуализатор МБР.
        /// </summary>
        public VisualizerState State
        {
            get { return _state; }
        }

        /// <summary>
        /// Устанавливает текущее состояние визуализатора с диспетчеризацией соответствующего события.
        /// </summary>
        /// <param name="state">Устанавливаемое состояние.</param>
        private void ChangeState(VisualizerState state)
        {
            VisualizerState old = _state;
            _state = state;
            if (OnStateChanged != null)
                OnStateChanged(this, new MachineVisualizerStateChangedEventArgs(old));
        }

        /// <summary>
        /// Получает индекс модуля, содержащего точку входа.
        /// </summary>
        /// <returns>Индекс модуля, если он найден, иначе - -1.</returns>
        private int GetEntryUnitIndex()
        {
            return _units.FindIndex(x => x.Code.Contains("entry"));
        }

        /// <summary>
        /// Получает полное имя файла программного модуля с указанным именем.
        /// </summary>
        /// <param name="name">Имя модуля.</param>
        /// <returns>Полное имя файла.</returns>
        private string GetUnitFileName(string name)
        {
            string ext = name + ".rmc";

            string fileName = _project.GetFiles().First(x => x.Length >= ext.Length ? x.Substring(x.Length - ext.Length) == ext : false);
            return _project.ProjectDirectory + Path.DirectorySeparatorChar + fileName;
        }

        /// <summary>
        /// Удаляет все отображаемые в редакторах кода маркеры выполняемых команд.
        /// </summary>
        private void RemoveCommandMarkers()
        {
            int n = tabCtrl.TabPages.Count;
            for (int i = 0; i < n; i++)
                (tabCtrl.TabPages[i].Controls[0] as CodeEditor).RemoveAllExecLines();
        }

        /// <summary>
        /// Устанавливает маркер выполняемой команды в указанную программу для ПМБР.
        /// </summary>
        /// <param name="program">Полное название программы, включая модуль, в котором она находится.</param>
        /// <param name="cmd">Номер выполняемой команды.</param>
        private void SetExecLine(string program, int cmd)
        {
            RegisterCodeEditor editor;

            string[] strs = program.Split('.');
            string unit = strs[0];
            string subProg = strs[1];
            
            editor = GetEditorByUnitName(unit);
            int line = editor.FindLine(subProg, cmd);
            editor.SetExecutionLine(line);
        }

        /// <summary>
        /// Устанавливает маркер выполняемой команды в указанную программу для классической МБР.
        /// </summary>
        /// <param name="cmd">Номер выполняемой команды.</param>
        private void SetExecLine(int cmd)
        {
            RegisterCodeEditor editor = (tabCtrl.TabPages[0].Controls[0] as RegisterCodeEditor);
            int line = editor.FindLine(cmd);
            editor.SetExecutionLine(line);
        }

        /// <summary>
        /// Обновляет состояния маркеров текущих выполняемых команд.
        /// </summary>
        private void UpdateCommandMarkers()
        {
            RemoveCommandMarkers();
            if(_parallelMode)
                threadFrame.GetSelected().ForEach(x => SetExecLine(x.Program, x.Command));
            else
                threadFrame.GetSelected().ForEach(x => SetExecLine(x.Command));
        }

        /// <summary>
        /// Обновляет информацию в окне потоков и отображаемые маркеры в редакторе кода.
        /// </summary>
        private void UpdateThreadsInfo()
        {
            ReadOnlyCollection<ThreadInfo> threads = _machine.GetThreadsInfo();
            int n = threads.Count;
            for (int i = 0; i < n; i++)
            {
                if (!threadFrame.Contains(threads[i]))
                    threadFrame.AddThreadInfo(threads[i], false);
            }
            threadFrame.UpdateInfo();
            UpdateCommandMarkers();
        }

        /// <summary>
        /// Возващает все точки останова, созданные пользователем во всех модулях.
        /// </summary>
        /// <returns>Массив точек останова.</returns>
        private RegisterBreakPoint[] CollectAllBreakPoints()
        {
            RegisterBreakPoint[] points;

            if(_parallelMode)
            {
                List<RegisterBreakPoint> list = new List<RegisterBreakPoint>();
                int n = tabCtrl.TabPages.Count;
                for (int i = 0; i < n; i++)
                {
                    list.AddRange((tabCtrl.TabPages[i].Controls[0] as RegisterCodeEditor).GetParallelBreakPoints());
                }
                points = list.ToArray();
            }
            else
            {
                int[] cmds = (tabCtrl.TabPages[0].Controls[0] as RegisterCodeEditor).GetClassicBreakPoints();
                string prog = tabCtrl.TabPages[0].Text;
                points = Array.ConvertAll<int, RegisterBreakPoint>(cmds, x => new RegisterBreakPoint() { Program = prog, Command = x });
            }

            return points;
        }

        /// <summary>
        /// Блокирует или разблокировывает пользовательский ввод во все редакторы кода.
        /// </summary>
        /// <param name="on">Определяет, заблокировать или разблокировать пользовательский ввод.</param>
        private void SwitchEditorsInput(bool on)
        {
            if(_parallelMode)
            {
                int n = tabCtrl.TabPages.Count;
                for (int i = 0; i < n; i++)
                    (tabCtrl.TabPages[i].Controls[0] as RegisterCodeEditor).ReadOnly = !on;
            }
            else
                (tabCtrl.TabPages[0].Controls[0] as RegisterCodeEditor).ReadOnly = !on;
            
        }

        public void Run()
        {
            try
            {
                if (_machine == null)
                    throw new InvalidOperationException("МБР не определена");
                if (_state == VisualizerState.Executing)
                    throw new InvalidOperationException("Машина уже запущена");

                ChangeState(VisualizerState.Executing);

                _machine.BreakPoints = CollectAllBreakPoints();
                SwitchEditorsInput(false);
                if (_parallelMode)
                {
                    int i = GetEntryUnitIndex();
                    if (i == -1) throw new Exception("Модуль, содержащий точку входа, не найден");
                    string unit = GetUnitFileName(_units[i].Name);
                    unit = RegisterTranslator.GetFullSource(unit);
                    RegisterProgramCollection programs = RegisterTranslator.TranslateParallelProgram(unit);
                    _machine.StartParallel(programs);
                }
                else
                {
                    List<Operation> ops = RegisterTranslator.TranslateClassicProgram(_units[0].Code);
                    _machine.Start(ops);
                }

                threadFrame.Clear();
            }
            catch(InvalidOperationTextException ex)
            {
                ChangeState(VisualizerState.Stopped);
                SwitchEditorsInput(true);
                if (Debug != null)
                {
                    string src = _project.Name + " - МБР";
                    Debug.AddMessage(src, string.Format("Ошибка запуска машины: \"{0}\"", ex.Message));
                    int n = ex.Data.Count;
                    for(int i = 0; i < n; i++)
                        Debug.AddMessage(src, (string)ex.Data[i]);
                }
            }
            catch(Exception ex)
            {
                ChangeState(VisualizerState.Stopped);
                if (Debug != null) Debug.AddMessage(_project.Name + " - МБР", string.Format("Ошибка запуска машины: \"{0}\"", ex.Message));
                SwitchEditorsInput(true);
            }
        }

        public void Stop()
        {
            try
            {
                if (_machine == null)
                    throw new InvalidOperationException("МБР не определена");
                if (_state == VisualizerState.Stopped)
                    throw new InvalidOperationException("Машина итак не запущена");

                if (Debug != null) Debug.AddMessage(_project.Name + " - МБР", "Начало процесса остановки МБР...");
                threadFrame.Clear();
                _machine.Stop();
                ChangeState(VisualizerState.Stopped);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно остановить машину: \"{0}\"", ex.Message), ex);
            }
        }

        public void Pause()
        {
            try
            {
                if (_machine == null)
                    throw new InvalidOperationException("МБР не определена");
                if (_state != VisualizerState.Executing)
                    throw new InvalidOperationException("Машина не запущена");

                _machine.Pause();

                UpdateThreadsInfo();
                threadFrame.Enabled = true;
                ChangeState(VisualizerState.Paused);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно приостановить машину: \"{0}\"", ex.Message), ex);
            }
        }

        public void Continue()
        {
            try
            {
                if (_machine == null)
                    throw new InvalidOperationException("МБР не определена");
                if (_state == VisualizerState.Executing)
                    throw new InvalidOperationException("Машина уже работает");

                RemoveCommandMarkers();
                threadFrame.Clear();
                threadFrame.Enabled = false;

                _machine.Continue();
                ChangeState(VisualizerState.Executing);

            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно возобновить работу машины: \"{0}\"", ex.Message), ex);
            }
        }

        public void Step()
        {
            try
            {
                if (_machine == null)
                    throw new InvalidOperationException("МБР не определена");
                if (_state == VisualizerState.Executing)
                    throw new InvalidOperationException("Машина в состоянии выполнения");

                threadFrame.Enabled = false;
                ChangeState(VisualizerState.Executing);
                _machine.Forward();
                ChangeState(VisualizerState.Paused);
                threadFrame.Enabled = true;
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно перейти к следующему шагу алгоритма: \"{0}\"", ex.Message), ex);
            }
        }

        public void SaveState()
        {
            try
            {
                if (_project == null)
                    throw new InvalidOperationException("Проект не определен");
                if (_state != VisualizerState.Stopped) return;

                SaveAllUnits();
                SaveMachineState();
                if (DataSaved != null)
                    DataSaved(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно сохранить состояние: \"{0}\"", ex.Message), ex);
            }
        }

    }
}
