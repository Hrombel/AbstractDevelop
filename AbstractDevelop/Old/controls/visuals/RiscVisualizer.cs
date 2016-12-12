using AbstractDevelop.controls.environment.debugwindow;
using AbstractDevelop.controls.visuals.additionals;
using AbstractDevelop.Machines.Risc;
using System;
using System.Drawing;
using System.IO;
using System.Windows.Forms;
using System.Runtime.Serialization.Formatters.Binary;

namespace AbstractDevelop.controls.visuals
{
    public partial class RiscVisualizer :
        UserControl, IMachineVisualizer
    {
        #region [События]

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

        #endregion

        #region [Свойства]

        /// <summary>
        /// Получает или задает текущий проект для RISC.
        /// </summary>
        public AbstractProject CurrentProject
        {
            get
            {
                return _project;
            }
            set
            {
                _project = value;
                CurrentMachine = GetMachineState();
            }
        }

        public DebugWindow Debug { get; set; }

        /// <summary>
        /// Получает состояние, в котором находится визуализатор RISC.
        /// </summary>
        public VisualizerState State { get { return _state; } }

        /// <summary>
        /// Получает или задет текущую визуализируемую машину с бесконечными регистрами.
        /// </summary>
        private RiscMachine CurrentMachine
        {
            get { return _machine; }
            set
            {
                if (_machine != null)
                {
                    _machine.ValueIn    -= _machine_ValueIn;
                    _machine.ValueOut   -= _machine_ValueOut;
                    _machine.OnStopped  -= _machine_OnMachineStopped;
                    _machine.RegisterUpdated -= _machine_RegisterUpdated;

                    memoryVisualizer.Registers = null;
                }
                _machine = value;
                if (_machine != null)
                {
                    memoryVisualizer.Registers = value.Memory;
                    _machine.OnStopped += _machine_OnMachineStopped;

                    _machine.ValueIn += _machine_ValueIn;
                    _machine.ValueOut += _machine_ValueOut;
                    _machine.RegisterUpdated += _machine_RegisterUpdated;
                }
            }
        }
          
        #endregion

        #region [Поля]

        private RiscMachine _machine;
        private AbstractProject _project;
        private VisualizerState _state;

        #endregion

        #region [Методы]

        public void Continue()
        {
            try
            {
                if (_machine == null)
                    throw new InvalidOperationException("RISC не определена");
                if (_state == VisualizerState.Executing)
                    throw new InvalidOperationException("Машина уже работает");

                _machine.Continue();
                ChangeState(VisualizerState.Executing);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно возобновить работу машины: \"{0}\"", ex.Message), ex);
            }
        }

        public void Pause()
        {
            try
            {
                if (_machine == null)
                    throw new InvalidOperationException("RISC не определена");
                if (_state != VisualizerState.Executing)
                    throw new InvalidOperationException("Машина не запущена");

                _machine.Pause();
                ChangeState(VisualizerState.Paused);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно приостановить машину: \"{0}\"", ex.Message), ex);
            }
        }

        public void Run()
        {
            try
            {
                if (_machine == null)
                    throw new InvalidOperationException("RISC не определена");
                if (_state == VisualizerState.Executing)
                    throw new InvalidOperationException("Машина уже запущена");

                ChangeState(VisualizerState.Executing);

                // TODO: восстановить функциональность точек останова
                //_machine.BreakPoints = CollectAllBreakPoints();
                SwitchEditorsInput(false);

                _machine.Operations.Load(_machine.SourceTranslator.Translate(codeEditor.Text.Split(Environment.NewLine.ToCharArray())));
                _machine.Start();
            }
            catch (Exception ex)
            {
                ChangeState(VisualizerState.Stopped);
                if (Debug != null) Debug.AddMessage($"{_project.Name} - RISC", $"Ошибка запуска машины: { ex.Message}");
                SwitchEditorsInput(true);
            }
        }

        public void SaveState()
        {
            try
            {
                if (_project == null)
                    throw new InvalidOperationException("Проект не определен");
                if (_state != VisualizerState.Stopped) return;

                SaveMachineState();
                SaveCode();

                DataSaved?.Invoke(this, EventArgs.Empty);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно сохранить состояние: \"{0}\"", ex.Message), ex);
            }
        }

        public void Step()
        {
            try
            {
                if (_machine == null)
                    throw new InvalidOperationException("RISC не определена");
                if (_state == VisualizerState.Executing)
                    throw new InvalidOperationException("Машина в состоянии выполнения");

                ChangeState(VisualizerState.Executing);
                _machine.Step();
                ChangeState(VisualizerState.Paused);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно перейти к следующему шагу алгоритма: \"{0}\"", ex.Message), ex);
            }
        }

        public void Stop()
        {
            try
            {
                if (_machine == null)
                    throw new InvalidOperationException("RISC не определена");
                if (_state == VisualizerState.Stopped)
                    throw new InvalidOperationException("Машина итак не запущена");

                if (Debug != null) Debug.AddMessage(_project.Name + " - RISC", "Начало процесса остановки RISC...");
       
                _machine.Stop();
                ChangeState(VisualizerState.Stopped);
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно остановить машину: \"{0}\"", ex.Message), ex);
            }
        }

        private void _machine_OnMachineStopped(object sender, EventArgs e)
        {
            ChangeState(VisualizerState.Stopped);
            SwitchEditorsInput(true);
            if (Debug != null) Debug.AddMessage(_project.Name + " - RISC", "Машина остановлена");
        }
 
        int ReadInput()
        {
            bool dialogOk = false;

            TextInputControl inputCtrl = new TextInputControl();
            inputCtrl.Size = inputCtrl.MinimumSize;
            inputCtrl.RegExp = @"\d+";
            inputCtrl.InputText = "0";
            inputCtrl.OKPressed += (x, a) =>
            {
                (x as TextInputControl).ParentForm.Close();
                dialogOk = true;
            };

            Form form = new Form();
            form.Text = "Запрос ввода значения";
            form.Controls.Add(inputCtrl);
            inputCtrl.Dock = DockStyle.Fill;
            form.ClientSize = inputCtrl.MinimumSize;
            form.MinimumSize = new Size(form.Width, form.Height);
            form.MaximumSize = new Size(inputCtrl.MaximumSize.Width, form.Height);
            form.FormBorderStyle = FormBorderStyle.SizableToolWindow;
            DialogResult res = form.ShowDialog();

            var value = (byte)0;
            if (dialogOk)
                byte.TryParse(inputCtrl.InputText, out value);

            return value;
        }

        private void _machine_ValueIn(object sender, RiscMachine.ValueEventArgs e) => e.Value = ReadInput();

        private void _machine_ValueOut(object sender, RiscMachine.ValueEventArgs e) => Debug.AddMessage(_project.Name + " - RISC", $"Результат: {e.Value}");

        /// <summary>
        /// Устанавливает текущее состояние визуализатора с диспетчеризацией соответствующего события.
        /// </summary>
        /// <param name="state">Устанавливаемое состояние.</param>
        private void ChangeState(VisualizerState state)
        {
            VisualizerState old = _state;
            _state = state;
            OnStateChanged?.Invoke(this, new MachineVisualizerStateChangedEventArgs(old));
        }

  
        /// <summary>
        /// Получает состояние машины проекта.
        /// </summary>
        /// <returns>Состояние машины.</returns>
        private RiscMachine GetMachineState()
        {
            var path = $"{Path.Combine(_project.ParentFolder.FullName, _project.Name)}_machine.risc";
            var file = _project.Files.Count > 0 ? _project.Files.Find((name) => name == path) : null;
            if (file == null)
            {
                file = new CodeFile(path, true);
                _project.Associate(file);
                return new RiscMachine();
            }
            else return file.Deserialize<RiscMachine>(new BinaryFormatter());
        }

 
        /// <summary>
        /// Генерирует событие о том, что пользователь внес изменения.
        /// </summary>
        private void InvalidateData() => DataChanged?.Invoke(this, EventArgs.Empty);

        internal void LoadCode()
        {
            var path = $"{Path.Combine(_project.ParentFolder.FullName, _project.Name)}_code.risc";
            //var file = _project.Files.Count > 0 ? _project.Files.Find((name) => name == path) : null;
            //if (file == null)
            //{
            //    file = new CodeFile(path, true);
            //    _project.Associate(file);
            //}
            if (File.Exists(path))
                codeEditor.Text = File.ReadAllText(path);
        }

        private void SaveCode()
        {
            var path = $"{Path.Combine(_project.ParentFolder.FullName, _project.Name)}_code.risc";
            //var file = _project.Files.Count > 0 ? _project.Files.Find((name) => name == path) : null;
            //if (file == null)
            //{
            //    file = new CodeFile(path, true);
            //    _project.Associate(file);
            //}
            File.WriteAllText(path, codeEditor.Text);
            //file.CreateWriter().Write(codeEditor.Text);
        }

        /// <summary>
        /// Сохраняет состояние машины проекта.
        /// </summary>
        private void SaveMachineState()
        {
            var path = $"{Path.Combine(_project.ParentFolder.FullName, _project.Name)}_machine.risc";
            var file = _project.Files.Count > 0 ? _project.Files.Find((name) => name == path) : null;
            if (file == null)
            {
                file = new CodeFile(path, true);
                _project.Associate(file);
            }

            file.Serialize(new BinaryFormatter(), CurrentMachine);
        }

        /// <summary>
        /// Блокирует или разблокировывает пользовательский ввод во все редакторы кода.
        /// </summary>
        /// <param name="on">Определяет, заблокировать или разблокировать пользовательский ввод.</param>
        private void SwitchEditorsInput(bool on)
        {
            codeEditor.ReadOnly = !on;
        }


        #endregion

        #region [Конструкторы]

        public RiscVisualizer()
        {
            _state = VisualizerState.Stopped;
            InitializeComponent();

            memoryVisualizer.FormatFunction = value => $"{ConvertToBinary(value)}\r\n({value})";
        }

        #endregion

        string ConvertToBinary(int value) => int.Parse(Convert.ToString(value, 2)).ToString("00000000");

        void VisualizeRegister(int index, int value) =>
                 listView.Items[index].SubItems[1].Text = $"{ConvertToBinary(value)} ({value})";

        void _machine_RegisterUpdated(int register) => VisualizeRegister(register, CurrentMachine.Registers[register]);

        private void RiscVisualizer_Load(object sender, EventArgs e)
        {
            for (int i = 0; i < CurrentMachine.Registers.Count; i++)
            {
                listView.Items.Add("r" + i).SubItems.Add("0");
                VisualizeRegister(i, CurrentMachine.Registers[0]);
            }
        }

        private void listView_DoubleClick(object sender, EventArgs e)
        {
            foreach (ListViewItem item in listView.SelectedItems)
            {
                CurrentMachine.Registers[item.Index] = ReadInput();
                VisualizeRegister(item.Index, CurrentMachine.Registers[item.Index]);
            }
        }
    }
}