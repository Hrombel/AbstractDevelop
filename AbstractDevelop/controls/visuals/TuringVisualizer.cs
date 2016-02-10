using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AbstractDevelop.controls.visuals.additionals;
using AbstractDevelop.machines.tape;
using AbstractDevelop.machines;
using AbstractDevelop.machines.turing;
using AbstractDevelop.projects;
using System.Runtime.Serialization;
using AbstractDevelop.controls.environment.debugwindow;
using System.IO;

namespace AbstractDevelop.controls.visuals
{
    /// <summary>
    /// Представляет контрол, визуализирующий модель машины Тьюринга.
    /// </summary>
    public partial class TuringVisualizer : UserControl, IMachineVisualizer
    {
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

        private List<TapeToolUnit> _tapeUnits;
        private TuringMachine _machine;
        private VisualizerState _state;
        private AbstractProject _project;

        private int _counter;
        private int[] _breakPoints;
        private bool _stepInProgress;

        public TuringVisualizer()
        {
            InitializeComponent();

            _machine = null;
            _breakPoints = null;
            _tapeUnits = new List<TapeToolUnit>();
            _state = VisualizerState.Stopped;
            _stepInProgress = false;

            addTapeBtn.Click += addTapeBtn_Click;
            tapePanel.Resize += tapePanel_Resize;
            codeBox.TextChanged += DataChangedHandler;
            codeBox.BreakPointToggled += BreakPointToggledHandler;
            turingInfo.DefaultCharChanged += turingInfo_DefaultCharChanged;

        }
        ~TuringVisualizer()
        {
            turingInfo.DefaultCharChanged -= turingInfo_DefaultCharChanged;
            codeBox.BreakPointToggled -= BreakPointToggledHandler;
            addTapeBtn.Click -= addTapeBtn_Click;
            tapePanel.Resize -= tapePanel_Resize;
            codeBox.TextChanged -= DataChangedHandler;

            while (_tapeUnits.Count > 0)
                RemoveTapeUnit(_tapeUnits[0]);

            _tapeUnits = null;
        }

        public DebugWindow Debug { get; set; }

        /// <summary>
        /// Получает или задает текущий проект для машины Тьюринга.
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
                    if (value.Machine != MachineId.Turing)
                        throw new Exception("Указан проект, предназначенный не для машины Тьюринга");
                }
                _project = value;
                if(_project != null)
                {
                    codeBox.Text = GetProjectCode();

                    CurrentMachine = GetMachineState();
                    SyncTapePositions(true);
                    turingInfo.DefaultChar = CurrentMachine.DefaultChar;
                    turingInfo.Alphabet = CurrentMachine.Tapes.Alphabet;
                }
            }
        }

        /// <summary>
        /// Получает или задает текущую визуализируемую машину Тьюринга.
        /// </summary>
        private TuringMachine CurrentMachine
        {
            get { return _machine; }
            set
            {
                if(_machine != null)
                {
                    _machine.TapeUpdated -= _machine_TapeUpdated;
                    _machine = null;
                    while(_tapeUnits.Count > 0)
                        RemoveTapeUnit(_tapeUnits[0]);
                }
                _machine = value;
                if(_machine != null)
                {
                    int n = _machine.TapesCount;
                    for (int i = 0; i < n; i++)
                        AddTapeUnit(_machine.Tapes.Units[i].Tape);
                    _machine.TapeUpdated += _machine_TapeUpdated;
                }
            }
        }

        private void turingInfo_DefaultCharChanged(object sender, EventArgs e)
        {
            CurrentMachine.DefaultChar = turingInfo.DefaultChar;
        }

        private void _machine_TapeUpdated(object sender, EventArgs e)
        {
            turingInfo.Alphabet = _machine.Tapes.Alphabet;
        }

        private void BreakPointToggledHandler(object sender, EventArgs e)
        {
            _breakPoints = codeBox.GetCommandBreakPoints();
        }

        /// <summary>
        /// Синхронизирует позиции головок визуалиаторов лент и лент.
        /// </summary>
        /// <param name="toVisualizer">Истина - копирование производится из ленты в визуализатор, иначе -
        /// в обратном направлении.</param>
        private void SyncTapePositions(bool toVisualizer)
        {
            if (_machine == null) return;

            int n = _tapeUnits.Count;
            if(toVisualizer)
            {
                for (int i = 0; i < n; i++)
                {
                    _tapeUnits[i].TapeVisualizer.Navigate(_machine.Tapes.Units[i].Position);
                }
            }
            else
            {
                for (int i = 0; i < n; i++)
                {
                    _machine.Tapes.Units[i].Position = _tapeUnits[i].TapeVisualizer.GetCurrentCell();
                }
            }
        }

        /// <summary>
        /// Получает код проекта.
        /// </summary>
        /// <returns>Строка с загруженным кодом.</returns>
        private string GetProjectCode()
        {
            string name = _project.Name + "Code.tmc";

            if(_project.FileExists(name))
            {
                return _project.LoadString(name);
            }
            else
            {
                SaveProjectCode();
                return "";
            }
        }

        /// <summary>
        /// Сохраняет код проекта.
        /// </summary>
        private void SaveProjectCode()
        {
            _project.SaveFile(_project.Name + "Code.tmc", codeBox.Text);
        }

        /// <summary>
        /// Получает состояние машины проекта.
        /// </summary>
        /// <returns>Состояние машины.</returns>
        private TuringMachine GetMachineState()
        {
            string name = _project.Name + "Machine.tms";

            return _project.FileExists(name) ? _project.LoadObject(name) as TuringMachine : new TuringMachine();
        }

        /// <summary>
        /// Сохраняет состояние машины проекта.
        /// </summary>
        private void SaveMachineState()
        {
            _project.SaveFile(_project.Name + "Machine.tms", CurrentMachine);
        }

        private void tapePanel_Resize(object sender, EventArgs e)
        {
            int h = GetTapeHeight();
            SetTapesSizes(h, GetTapesMargin(h));
        }

        /// <summary>
        /// Вычисляет отступ между модулями ленты, основываясь на текущей высоте модуля ленты.
        /// </summary>
        /// <param name="height">Текущая высота модуля ленты.</param>
        /// <returns>Актуальный отступ между лентами.</returns>
        private int GetTapesMargin(int height)
        {
            return GetTapeHeight() >> 3;
        }

        /// <summary>
        /// Вычисляет высоту одного модуля ленты, исходя из текущих размеров контрола.
        /// </summary>
        /// <returns>Актуальный размер модулей.</returns>
        private int GetTapeHeight()
        {
            return Height >> 2;
        }

        /// <summary>
        /// Устанавливает высоту и отступ между модулями лент.
        /// </summary>
        /// <param name="height">Устанавливаемая высота.</param>
        /// <param name="padding">Устанавливаемый отступ.</param>
        private void SetTapesSizes(int height, int padding)
        {
            int n = _tapeUnits.Count;
            for(int i = 0; i < n; i++)
            {
                _tapeUnits[i].Height = height;
                _tapeUnits[i].Padding = new Padding() {Bottom = padding };
            }
        }

        /// <summary>
        /// Добавляет новый модуль ленты в элемент управления лентами.
        /// </summary>
        /// <param name="tape">Лента, сопоставленная с визуальным модулем.</param>
        private void AddTapeUnit(Tape tape)
        {
            TapeToolUnit unit = new TapeToolUnit();
            unit.TapeVisualizer.ExternalSymbolSet = _machine.Tapes.SymbolSet;
            unit.TapeVisualizer.CurrentTape = tape;

            unit.Dock = DockStyle.Top;
            unit.Height = GetTapeHeight();
            unit.Padding = new Padding() { Bottom = GetTapesMargin(unit.Height) };
            _tapeUnits.Add(unit);
            tapePanel.Controls.Add(unit);
            unit.BringToFront();
            unit.OnTapeRemovePressed += OnTapeRemovePressed;
            unit.TapeVisualizer.PositionChanged += DataChangedHandler;
        }

        private void DataChangedHandler(object sender, EventArgs e)
        {
            InvalidateData();
        }

        /// <summary>
        /// Генерирует событие изменения данных пользователем.
        /// </summary>
        private void InvalidateData()
        {
            if (DataChanged != null)
                DataChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Удаляет модуль ленты из списка лент.
        /// </summary>
        /// <param name="unit">Удаляемый модуль.</param>
        private void RemoveTapeUnit(TapeToolUnit unit)
        {
            unit.TapeVisualizer.PositionChanged -= DataChangedHandler;
            unit.OnTapeRemovePressed -= OnTapeRemovePressed;
            tapePanel.Controls.Remove(unit);
            _tapeUnits.Remove(unit);
            unit.Dispose();

            if(_machine != null)
                _machine.TapesCount--;
        }

        private void OnTapeRemovePressed(object sender, EventArgs e)
        {
            if(_tapeUnits.Count > 1)
                RemoveTapeUnit(sender as TapeToolUnit);
            InvalidateData();
        }

        private void addTapeBtn_Click(object sender, EventArgs e)
        {
            if(_machine == null)
            {
                MessageBox.Show("Невозможно добавить ленту, поскольку машина Тьюринга не определена.");
                return;
            }

            _machine.TapesCount++;
            AddTapeUnit(_machine.Tapes.Units[_machine.TapesCount - 1].Tape);
            InvalidateData();
        }

        /// <summary>
        /// Получает текущее состояние визуализатора машины Тьюринга.
        /// </summary>
        public VisualizerState State
        {
            get { return _state; }
        }

        /// <summary>
        /// Отключает у всех лент режим ручного ввода.
        /// </summary>
        private void SetExecMode()
        {
            int n = _tapeUnits.Count;
            for (int i = 0; i < n; i++ )
            {
                _tapeUnits[i].TapeVisualizer.Navigate(_tapeUnits[i].TapeVisualizer.GetCurrentCell());
                _tapeUnits[i].TapeVisualizer.InputMode = false;
            }
        }

        /// <summary>
        /// Включает у всех лент режим ручного ввода.
        /// </summary>
        private void SetInputMode()
        {
            _tapeUnits.ForEach(x => x.TapeVisualizer.InputMode = true);
        }

        /// <summary>
        /// Запускает процесс перемещения лент в указанных направлениях.
        /// </summary>
        /// <param name="directions">Массив направлений передвижения лент.</param>
        private void MoveTapes(TuringPenDir[] directions)
        {
            _counter = 0;
            int n = _tapeUnits.Count;
            for(int i = 0; i < n; i++)
            {
                _tapeUnits[i].TapeVisualizer.OnNavigationEnd += TapeVisualizer_OnNavigationEnd;
                _tapeUnits[i].TapeVisualizer.Navigate(_machine.Tapes.Units[i].Position, _tapeUnits[i].TapeVisualizer.RecommendedNavigationSpeed);
            }
        }

        private void TapeVisualizer_OnNavigationEnd(object sender, EventArgs e)
        {
            (sender as TapeVisualizer).OnNavigationEnd -= TapeVisualizer_OnNavigationEnd;
            if (_state == VisualizerState.Stopped) return;

            _counter++;
            if(_counter == _tapeUnits.Count)
            {
                if (_state != VisualizerState.Paused)
                    NextStep();
            }

            _stepInProgress = false;
        }

        /// <summary>
        /// Устанавливает курсор выполнения на текущую команду,
        /// проверяет ее на наличие точек останова и, при необходимости,
        /// переводит исполнителя в состояние паузы.
        /// </summary>
        private void CheckCurrentCommand()
        {
            codeBox.RemoveAllExecLines();
            if (_state == VisualizerState.Stopped) return;
            
            int i = _machine.GetConversionIndex();
            if (i != -1)
            {
                codeBox.SetExecutionCommand(i);
                if (Array.FindIndex<int>(_breakPoints, x => x == i) != -1)
                    ChangeState(VisualizerState.Paused);
            }
        }

        /// <summary>
        /// Запускает выполнение следующего шага алгоритма.
        /// </summary>
        private void NextStep()
        {
            _stepInProgress = true;
            _machine.Forward();

            CheckCurrentCommand();
        }

        /// <summary>
        /// Сопоставляет текстовое объяснение с причиной остановки МТ.
        /// </summary>
        /// <param name="reason">Причина остановки МТ.</param>
        /// <returns>Строка, представляющая объяснение причины останова.</returns>
        private string DecodeReason(TuringMachineStopReason reason)
        {
            switch(reason)
            {
                case TuringMachineStopReason.UserInterrupt:
                    return "Вмешательство пользователя";
                case TuringMachineStopReason.UndefinedState:
                    return "Переход к несуществующему состоянию";
                case TuringMachineStopReason.UndefinedConversion:
                    return "Неопределенная комбинация символов на лентах";
            }

            throw new ArgumentException("Неизвестная причина останова");
        }

        private void _machine_OnMachineStopped(object sender, TuringMachineStopEventArgs e)
        {
            _machine.OnMachineStateChanged -= _machine_OnMachineStateChanged;
            _machine.OnMachineStopped -= _machine_OnMachineStopped;

            SetInputMode();

            VisualizerState prev = _state;
            ChangeState(VisualizerState.Stopped);
            codeBox.RemoveAllExecLines();
            _breakPoints = null;
            codeBox.ReadOnly = false;
            turingInfo.Enabled = true;
            if (OnStateChanged != null)
                OnStateChanged(this, new MachineVisualizerStateChangedEventArgs(prev));

            if (Debug != null)
                Debug.AddMessage(_project.Name + " - Машина Тьюринга", string.Format("Работа машины завершена. Причина: \"{0}\"", DecodeReason(e.Reason)));
        }

        private void _machine_OnMachineStateChanged(object sender, TuringMachineStateChangedEventArgs e)
        {
            MoveTapes(e.Directions);
        }

        /// <summary>
        /// Выполняет смену состояния визуализатора и дипетчеризирует соответствующее событие.
        /// </summary>
        /// <param name="state">Устанавливаемое состояние.</param>
        private void ChangeState(VisualizerState state)
        {
            VisualizerState prev = _state;
            _state = state;
            if (OnStateChanged != null)
                OnStateChanged(this, new MachineVisualizerStateChangedEventArgs(prev));
        }

        public void Run()
        {
            try
            {
                if (_machine == null)
                    throw new InvalidOperationException("МТ не определена");
                if (_state == VisualizerState.Executing)
                    throw new InvalidOperationException("Работа визуализации уже запущена");

                turingInfo.Enabled = false;
                _machine.Tapes.DefaultChar = turingInfo.DefaultChar;
                _machine.StartManual(TuringTranslator.Translate(_project.LoadString(_project.Name + "Code.tmc")));

                ChangeState(VisualizerState.Executing);
                SetExecMode();
                _machine.OnMachineStateChanged += _machine_OnMachineStateChanged;
                _machine.OnMachineStopped += _machine_OnMachineStopped;
                SyncTapePositions(false);
                _breakPoints = codeBox.GetCommandBreakPoints();
                codeBox.ReadOnly = true;

                CheckCurrentCommand();
                if(_state != VisualizerState.Paused)
                    NextStep();
            }
            catch(Exception ex)
            {
                ChangeState(VisualizerState.Stopped);
                if(Debug != null) Debug.AddMessage(_project.Name + " - Машина Тьюринга", string.Format("Ошибка запуска машины: \"{0}\"", ex.Message));
            }
        }

        public void Stop()
        {
            try
            {
                if (_machine == null)
                    throw new InvalidOperationException("Невозможно остановить визуализацию работы машины Тьюринга, поскольку МТ не определена");
                if (_state == VisualizerState.Stopped)
                    throw new InvalidOperationException("Работа визуализатора итак не запущена");

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
                    throw new InvalidOperationException("Невозможно запустить визуализацию работы машины Тьюринга, поскольку МТ не определена");
                if (_state == VisualizerState.Paused)
                    throw new InvalidOperationException("Работа визуализации итак приостановлена");
                if (_state == VisualizerState.Stopped)
                    throw new InvalidOperationException("Работа визуализации полностью остановлена");

                ChangeState(VisualizerState.Paused);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно приостановить машину: \"{0}\"", ex.Message), ex);
            }
        }

        public void Continue()
        {
            try
            {
                if (_machine == null)
                    throw new InvalidOperationException("МТ не определена");
                if (_state == VisualizerState.Executing)
                    throw new InvalidOperationException("Работа визуализации итак выполняется");
                if (_state == VisualizerState.Stopped)
                    throw new InvalidOperationException("Работа визуализации полностью остановлена");

                ChangeState(VisualizerState.Executing);
                NextStep();
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно возоновить работу машины: \"{0}\"", ex.Message), ex);
            }

        }

        public void Step()
        {
            try
            {
                if (_machine == null)
                    throw new InvalidOperationException("МТ не определена");
                if (_state == VisualizerState.Executing)
                    throw new InvalidOperationException("Машина в режиме выполнения");
                if (_stepInProgress) return;

                if(_breakPoints == null)
                    _breakPoints = codeBox.GetCommandBreakPoints();

                if (!codeBox.ReadOnly) codeBox.ReadOnly = true;


                ChangeState(VisualizerState.Executing);
                NextStep();
                ChangeState(VisualizerState.Paused);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно запустить пошаговое выполнение: \"{0}\"", ex.Message), ex);
            }
        }

        public void SaveState()
        {
            try
            {
                if (_project == null)
                    throw new InvalidOperationException("Проект не определен");
                if (_state != VisualizerState.Stopped) return;

                SaveProjectCode();

                SyncTapePositions(false);
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
