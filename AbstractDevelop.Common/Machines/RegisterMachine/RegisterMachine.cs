using AbstractDevelop.machines.registers;
using AbstractDevelop.Machines;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Numerics;
using System.Runtime.Serialization;

using RegisterOperation = AbstractDevelop.Machines.Operation<AbstractDevelop.machines.regmachine.RegisterOperationId, System.Numerics.BigInteger>;

namespace AbstractDevelop.machines.regmachine
{
    /// <summary>
    /// Представляет модель параллельной машины с бесконечными регистрами.
    /// </summary>
    [Serializable]
    public class RegisterMachine : ISerializable
    {
        /// <summary>
        /// Генерируется после остановки работы МБР.
        /// </summary>
        public event EventHandler<RegisterMachineStoppedEventArgs> OnMachineStopped;

        /// <summary>
        /// Генерируется после записи значения в устройство вывода.
        /// </summary>
        public event EventHandler<RegisterMachineValueOutEventArgs> ValueOut;

        /// <summary>
        /// Происходит перед чтением очередного значения из устройства ввода.
        /// </summary>
        public event EventHandler<RegisterMachineValueInEventArgs> ValueIn;

        /// <summary>
        /// Происходит после достижения исполнителем точки останова.
        /// </summary>
        public event EventHandler BreakPointReached;

        /// <summary>
        /// Происходит после выполнения машиной одного шага в режиме отладки.
        /// </summary>
        public event EventHandler StepCompleted;

        private InfiniteRegisters _registers;

        [NonSerialized]
        private RegisterThreadManager _manager;

        private RegisterBreakPoint[] _breakPoints;

        /// <summary>
        /// Инициализирует параллельную машину с бесконечными регистрами, устанавливая значения всех регистров равными нулю.
        /// </summary>
        public RegisterMachine()
        {
            _registers = new InfiniteRegisters();
            _manager = null;
        }

        private RegisterMachine(SerializationInfo info, StreamingContext context)
        {
            _registers = info.GetValue("registers", typeof(InfiniteRegisters)) as InfiniteRegisters;
        }

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            info.AddValue("registers", _registers, typeof(InfiniteRegisters));
        }

        /// <summary>
        /// Получает текущее состояние регистров.
        /// </summary>
        public InfiniteRegisters Registers { get { return _registers; } }

        /// <summary>
        /// Получает или задает точки останова МБР.
        /// </summary>
        public RegisterBreakPoint[] BreakPoints
        {
            get { return _breakPoints; }
            set
            {
                _breakPoints = value;
                if (_manager != null)
                    _manager.BreakPoints = value;
            }
        }

        /// <summary>
        /// Получает информацию о текущих выполняемых потоках.
        /// </summary>
        /// <returns>Readonly коллекция информации о выполняемых потоках.</returns>
        public ReadOnlyCollection<ThreadInfo> GetThreadsInfo()
        {
            return _manager.GetThreadsInfo();
        }

        private void _manager_OnComplete(object sender, RegisterThreadManagerEventArgs e)
        {
            _manager.BreakPointReached -= _manager_BreakPointReached;
            _manager.OnComplete -= _manager_OnComplete;
            _manager.ValueOut -= _manager_ValueOut;
            _manager.ValueIn -= _manager_ValueIn;
            _manager.BreakPointReached -= _manager_BreakPointReached;
            Stop();

            BreakPoints = null;
            _manager = null;
            if (OnMachineStopped != null)
                OnMachineStopped(this, new RegisterMachineStoppedEventArgs(e.RegisterChanged));
        }

        private void _manager_ValueOut(object sender, EventArgs e)
        {
            int n = _manager.OutputBufferAmount;
            List<BigInteger> buffer = new List<BigInteger>(n);
            while (n > 0)
            {
                buffer.Add(_manager.ReadValue());
                n--;
            }
            if (ValueOut != null)
                ValueOut(this, new RegisterMachineValueOutEventArgs(buffer.ToArray()));
        }

        private void _manager_ValueIn(object sender, EventArgs e)
        {
            if (ValueIn != null)
            {
                RegisterMachineValueInEventArgs args = new RegisterMachineValueInEventArgs();
                ValueIn(this, args);
                _manager.WriteValue(args.Value);
            }
        }

        private void _manager_BreakPointReached(object sender, EventArgs e)
        {
            if (BreakPointReached != null)
                BreakPointReached(this, EventArgs.Empty);
        }

        private void _manager_StepCompleted(object sender, EventArgs e)
        {
            if (StepCompleted != null)
                StepCompleted(this, EventArgs.Empty);
        }

        /// <summary>
        /// Запускает программы с возможностью параллельного выполнения.
        /// </summary>
        /// <param name="programms">Программы для ПМБР.</param>
        public void StartParallel(RegisterProgramCollection programs)
        {
            if (programs == null)
                throw new ArgumentNullException("Коллекция программ не может быть неопределенной");
            if (programs.Count == 0)
                throw new ArgumentException("Коллекция программ не может быть пустой");
            if (_manager != null)
                throw new Exception("ПМБР уже запущена");

            _manager = new RegisterThreadManager(_registers, programs);
            _manager.BreakPoints = _breakPoints;
            _manager.OnComplete += _manager_OnComplete;
            _manager.ValueOut += _manager_ValueOut;
            _manager.ValueIn += _manager_ValueIn;
            _manager.StepCompleted += _manager_StepCompleted;
            _manager.BreakPointReached += _manager_BreakPointReached;
            _manager.RunEntryPoint(programs.GetEntry().Id);
        }

        /// <summary>
        /// Запускает выполнение операций без использования паралеллизма.
        /// </summary>
        /// <param name="ops">Список операций для ПМБР.</param>
        public void Start(List<Operation<object, object>> ops)
        {
            if (ops == null)
                throw new ArgumentNullException("Список операций не может быть неопределенным");
            if (_manager != null)
                throw new Exception("ПМБР уже запущена");

            RegisterProgram mainProg = new RegisterProgram(ops.Cast<RegisterOperation>().ToList().ToArray(), 0, "classic", true);
            RegisterProgramCollection programs = new RegisterProgramCollection();
            programs.Add(mainProg);
            _manager = new RegisterThreadManager(_registers, programs);
            _manager.BreakPoints = _breakPoints;
            _manager.OnComplete += _manager_OnComplete;
            _manager.BreakPointReached += _manager_BreakPointReached;
            _manager.RunEntryPoint(0);
        }

        /// <summary>
        /// Запускает классическую МБР в режиме пошагового выполнения.
        /// </summary>
        /// <param name="ops">Список операций для выполнения.</param>
        public void StartManual(List<Operation<object, object>> ops)
        {
            if (ops == null)
                throw new ArgumentNullException("Коллекция операций не может быть неопределенной");
            if (ops.Count == 0)
                throw new ArgumentException("Коллекция операций не может быть пустой");
            if (_manager != null)
                throw new Exception("МБР уже запущена");

            RegisterProgram mainProg = new RegisterProgram(ops.Cast<RegisterOperation>().ToList().ToArray(), 0, "classic", true);
            RegisterProgramCollection programs = new RegisterProgramCollection();
            programs.Add(mainProg);
            _manager = new RegisterThreadManager(_registers, programs);
            _manager.BreakPoints = _breakPoints;
            _manager.OnComplete += _manager_OnComplete;
            _manager.BreakPointReached += _manager_BreakPointReached;
            _manager.RunEntryPoint(0);
        }

        /// <summary>
        /// Приостанавливает работу машины.
        /// </summary>
        public void Pause()
        {
            if (_manager == null)
                throw new InvalidOperationException("МБР не запущена");
            if (_manager.DebugMode)
                throw new InvalidOperationException("Работа машины уже приостановлена");

            _manager.DebugMode = true;
        }

        /// <summary>
        /// Возобновляет работу машины.
        /// </summary>
        public void Continue()
        {
            if (_manager == null)
                throw new InvalidOperationException("МБР не запущена");
            if (!_manager.DebugMode)
                throw new InvalidOperationException("Машина уже работает");

            _manager.DebugMode = false;
        }

        /// <summary>
        /// Выполняет следующую операцию, если машина находится в режиме пошагового выполнения.
        /// </summary>
        /// <returns>Истина, если выполнен не последний шаг алгоритма, иначе - ложь.</returns>
        public bool Forward()
        {
            if (_manager == null)
                throw new InvalidOperationException("МБР не запущена");
            if (!_manager.DebugMode)
                throw new InvalidOperationException("Машина не находится в режиме пошагового выполнения");

            return _manager.Step();
        }

        /// <summary>
        /// Останавливает работу машины.
        /// </summary>
        public void Stop()
        {
            if (_manager == null)
                throw new InvalidOperationException("МБР итак остановлена");

            if (_manager.IsBusy)
                _manager.StopAllThreads();
        }
    }
}