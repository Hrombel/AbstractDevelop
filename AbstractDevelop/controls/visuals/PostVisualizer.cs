using System;
using System.Collections.Generic;
using System.ComponentModel;
using System.Drawing;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using System.Windows.Forms;
using AbstractDevelop.machines.post;
using AbstractDevelop.machines;
using AbstractDevelop.errors.dev;
using System.Numerics;
using AbstractDevelop.projects;
using System.IO;
using System.Runtime.Serialization.Formatters.Binary;
using AbstractDevelop.controls.environment.debugwindow;

namespace AbstractDevelop.controls.visuals
{
    /// <summary>
    /// Представляет средство визуализации машины Поста.
    /// </summary>
    public partial class PostVisualizer : UserControl, IMachineVisualizer
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

        private PostMachine _currentMachine;
        private VisualizerState _state;
        private AbstractProject _project;

        private int[] _breakPoints;
        private bool _stepInProgress;

        public PostVisualizer()
        {
            InitializeComponent();

            _stepInProgress = false;
            _currentMachine = null;
            _breakPoints = null;
            _state = VisualizerState.Stopped;
            tapeVisualizer.TapeUpdated += DataChangedHandler;
            tapeVisualizer.PositionChanged += DataChangedHandler;
            codeBox.TextChanged += DataChangedHandler;
            codeBox.BreakPointToggled += BreakPointToggled;
        }
        ~PostVisualizer()
        {
            tapeVisualizer.TapeUpdated -= DataChangedHandler;
            tapeVisualizer.PositionChanged -= DataChangedHandler;
            codeBox.TextChanged -= DataChangedHandler;
            codeBox.BreakPointToggled -= BreakPointToggled;
        }

        public DebugWindow Debug { get; set; }

        /// <summary>
        /// Получает или задает текущую визуализируемую машину Поста.
        /// </summary>
        public PostMachine CurrentMachine
        {
            get
            {
                return _currentMachine;
            }
            set
            {
                if(_currentMachine != null)
                {
                    tapeVisualizer.CurrentTape = null;
                    _currentMachine = null;
                }

                if (value != null)
                {
                    tapeVisualizer.CurrentTape = value.Tape;
                    _currentMachine = value;
                }
            }
        }

        /// <summary>
        /// Получает или задает текущий проект для машины Поста.
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
                    if (value.Machine != MachineId.Post)
                        throw new Exception("Указан проект, предназначенный не для машины Поста");
                }
                _project = value;
                if(_project != null)
                {
                    codeBox.Text = GetProjectCode();
                    CurrentMachine = GetMachineState();
                    tapeVisualizer.Navigate(CurrentMachine.Position);
                }
            }
        }

        /// <summary>
        /// Получает текущее состояние визуализатора.
        /// </summary>
        public VisualizerState State
        {
            get { return _state; }
        }

        private void BreakPointToggled(object sender, EventArgs e)
        {
            if (_breakPoints != null)
                _breakPoints = codeBox.GetCommandBreakPoints();
        }

        private void DataChangedHandler(object sender, EventArgs e)
        {
            InvalidateData();
        }

        /// <summary>
        /// Генерирует событие о том, что данные компонента были изменены.
        /// </summary>
        private void InvalidateData()
        {
            if (DataChanged != null)
                DataChanged(this, EventArgs.Empty);
        }

        /// <summary>
        /// Сохраняет код проекта.
        /// </summary>
        private void SaveProjectCode()
        {
            _project.SaveFile(_project.Name + "Code.pmc", codeBox.Text);
        }

        /// <summary>
        /// Получает последний сохраненный код проекта.
        /// </summary>
        /// <returns>Строка, представляющая код проекта.</returns>
        private string GetProjectCode()
        {
            string name = _project.Name + "Code.pmc";

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
        /// Сохраняет текущее состояние машины Поста.
        /// </summary>
        private void SaveMachineState()
        {
            _project.SaveFile(_project.Name + "State.pms", CurrentMachine);
        }

        /// <summary>
        /// Получает состояние машины проекта.
        /// </summary>
        /// <returns>Загруженное состояние.</returns>
        private PostMachine GetMachineState()
        {
            string name = _project.Name + "State.pms";

            return _project.FileExists(name) ? _project.LoadObject(name) as PostMachine : new PostMachine();
        }

        /// <summary>
        /// Запускает выполнение следующей команды, если машина не в состоянии паузы.
        /// </summary>
        /// <param name="ignoreBreakPoint">Определяет, нужно ли игнорировать точку останова, если она присутствует.</param>
        private void ExecuteNext(bool ignoreBreakPoint = false)
        {
            if (_state == VisualizerState.Executing && !ignoreBreakPoint)
            {
                if (Array.FindIndex<int>(_breakPoints, x => x == _currentMachine.CurrentOperation) != -1)
                {
                    ChangeState(VisualizerState.Paused);
                    return;
                }
            }

            _stepInProgress = true;
            _currentMachine.Forward();

            codeBox.RemoveAllExecLines();
            if (_currentMachine.CurrentOperation != -1)
                codeBox.SetExecutionCommand(_currentMachine.CurrentOperation);
        }

        private void _currentMachine_OnOperationExecuted(object sender, PostOperationExecutedEventArgs e)
        {
            switch(e.Operation)
            {
                case PostOperationId.Right:
                case PostOperationId.Left:
                    tapeVisualizer.Navigate(_currentMachine.Position, tapeVisualizer.RecommendedNavigationSpeed);
                    break;
                case PostOperationId.Decision:
                case PostOperationId.Erase:
                case PostOperationId.Place:
                    _stepInProgress = false;
                    if (_state == VisualizerState.Paused) return;
                    ExecuteNext();
                    break;
            }
        }

        private void tapeVisualizer_OnNavigationEnd(object sender, EventArgs e)
        {
            _stepInProgress = false;
            if (_state == VisualizerState.Paused) return;

            ExecuteNext();
        }

        private void _currentMachine_OnMachineStopped(object sender, PostMachineStopEventArgs e)
        {
            _currentMachine.OnOperationExecuted -= _currentMachine_OnOperationExecuted;
            _currentMachine.OnMachineStopped -= _currentMachine_OnMachineStopped;
            tapeVisualizer.OnNavigationEnd -= tapeVisualizer_OnNavigationEnd;
            tapeVisualizer.InputMode = true;
            codeBox.RemoveAllExecLines();
            _breakPoints = null;
            codeBox.ReadOnly = false;
            ChangeState(VisualizerState.Stopped);
            if (Debug != null)
                Debug.AddMessage(_project.Name + " - Машина Поста", string.Format("Работа машины остановлена. Причина: \"{0}\"", DecodeReason(e.Reason)));
        }

        /// <summary>
        /// Изменяет текущее состояние визуализатора и генерирует соответствующее событие.
        /// </summary>
        /// <param name="newState">Новое состояние.</param>
        private void ChangeState(VisualizerState newState)
        {
            VisualizerState old = _state;
            _state = newState;
            if (OnStateChanged != null)
                OnStateChanged(this, new MachineVisualizerStateChangedEventArgs(old));
        }

        /// <summary>
        /// Возвращает строковую интерпретацию кода причины останова машины Поста.
        /// </summary>
        /// <param name="reason">Код причины останова машины Поста.</param>
        private string DecodeReason(PostMachineStopReason reason)
        {
            string res;

            switch (reason)
            {
                case PostMachineStopReason.OUT_OF_OPERATION_NUMBER:
                    res = "Переход к несуществующей операции";
                    break;
                case PostMachineStopReason.REMOVE_NULL_LABEL:
                    res = "Попытка стирания несуществующей метки";
                    break;
                case PostMachineStopReason.SET_EXISTING_LABEL:
                    res = "Попытка установки метки в ячейку с меткой";
                    break;
                case PostMachineStopReason.STOP_OPERATION:
                    res = "Вызвана команда останова";
                    break;
                case PostMachineStopReason.USER_INTERRUPT:
                    res = "Вмешательство пользователя";
                    break;
                default:
                    res = "Причина неизвестна";
                    break;
            }

            return res;
        }

        /// <summary>
        /// Производит попытку запуска работы абстрактного вычислителя.
        /// </summary>
        public void Run()
        {
            try
            {
                if (_state == VisualizerState.Executing)
                    throw new Exception("Машина уже запущена");

                List<Operation> ops = PostTranslator.Translate(codeBox.Text);
                _currentMachine.StartManual(ops);
                ChangeState(VisualizerState.Executing);
                BigInteger cell = tapeVisualizer.GetCurrentCell();
                tapeVisualizer.Navigate(cell);
                tapeVisualizer.InputMode = false;
                _currentMachine.Position = cell;
                _breakPoints = codeBox.GetCommandBreakPoints();
                codeBox.ReadOnly = true;
                _currentMachine.OnOperationExecuted += _currentMachine_OnOperationExecuted;
                _currentMachine.OnMachineStopped += _currentMachine_OnMachineStopped;
                tapeVisualizer.OnNavigationEnd += tapeVisualizer_OnNavigationEnd;
                if (ops.Count != 0) codeBox.SetExecutionCommand(1);
                ExecuteNext();
            }
            catch (InvalidOperationTextException ex)
            {
                ChangeState(VisualizerState.Stopped);
                if(Debug != null)
                {
                    Debug.AddMessage(_project.Name + " - Машина Поста", "При запуске машины возникли ошибки");
                    int n = ex.Data.Count;
                    for (int i = 0; i < n; i++)
                        Debug.AddMessage(_project.Name + " - Машина Поста", Convert.ToString(ex.Data[i]));
                }
            }
        }

        public void Stop()
        {
            try
            {
                if (_state == VisualizerState.Stopped)
                    throw new Exception("Машина не запущена");

                _currentMachine.Stop();
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
                if (_state == VisualizerState.Paused || _state == VisualizerState.Stopped)
                    throw new Exception("Машина не запущена");

                ChangeState(VisualizerState.Paused);

            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно приостановить работу машины: \"{0}\"", ex.Message), ex);
            }
        }

        public void Continue()
        {
            try
            {
                if (_state != VisualizerState.Paused)
                    throw new InvalidOperationException("Машина не находится в состояниий паузы");

                ChangeState(VisualizerState.Executing);
                ExecuteNext(true);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно возобновить работу машины: \"{0}\"", ex.Message), ex);
            }
        }

        public void Step()
        {
            try
            {
                if (_state == VisualizerState.Executing)
                    throw new InvalidOperationException("Машина находится в режиме выполнения");
                if (_stepInProgress) return;

                //ChangeState(VisualizerState.Executing);
                ExecuteNext();
                if (!codeBox.ReadOnly) codeBox.ReadOnly = true;
                /*if(_state != VisualizerState.Stopped)
                    ChangeState(VisualizerState.Paused);*/
            }
            catch (Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно выполнить пошаговую работу машины: \"{0}\"", ex.Message), ex);
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
                _currentMachine.Position = tapeVisualizer.GetCurrentCell();
                SaveMachineState();
                if (DataSaved != null)
                    DataSaved(this, EventArgs.Empty);
            }
            catch(Exception ex)
            {
                throw new InvalidOperationException(string.Format("Невозможно сохранить состояние: \"{0}\"", ex.Message), ex);
            }
            
        }
    }
}
