using AbstractDevelop.machines.registers;
using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Linq;
using System.Numerics;
using System.Threading;

namespace AbstractDevelop.machines.regmachine
{
    /// <summary>
    /// Представляет средство работы с потоками параллельной машины с бесконечными регистрами.
    /// </summary>
    public class RegisterThreadManager
    {
        private class LockedRegister : IComparable<LockedRegister>, IComparable
        {
            public BigInteger Index;
            public RegisterThread Thread;
            public List<RegisterThread> Awaiters;

            public LockedRegister()
            {
                Awaiters = new List<RegisterThread>();
            }

            public int CompareTo(LockedRegister other)
            {
                return Index.CompareTo(other.Index);
            }

            public int CompareTo(object obj)
            {
                return CompareTo(obj as LockedRegister);
            }
        }

        /// <summary>
        /// Происходит после завершения выполнения всех потоков.
        /// </summary>
        public event EventHandler<RegisterThreadManagerEventArgs> OnComplete;
        /// <summary>
        /// Происходит после записи значения на устройство вывода.
        /// </summary>
        public event EventHandler ValueOut;
        /// <summary>
        /// Происходит перед чтением очередного значения из устройства ввода.
        /// </summary>
        public event EventHandler ValueIn;
        /// <summary>
        /// Происходит после достижения точки останова и приостановки всех потоков.
        /// </summary>
        public event EventHandler BreakPointReached;
        /// <summary>
        /// Происходит после выполнения одного шага всеми в отладочном режиме.
        /// </summary>
        public event EventHandler StepCompleted;

        private InfiniteRegisters _registers;
        private RegisterProgramCollection _programs;
        private List<RegisterThread> _threads;
        private List<LockedRegister> _locked;
        private List<RegisterThread> _pausedThreads;

        private bool _debugMode;
        private bool _registersChanged;
        private bool _breakPointReached;
        private bool _isParallel;

        private Queue<BigInteger> _inputBuffer;
        private Queue<BigInteger> _outputBuffer;

        private BackgroundWorker _mainLoop;
        private AutoResetEvent _sync;
        private AutoResetEvent _inputSync;
        private bool _stoppingProcess;

        /// <summary>
        /// Определяет сигнал, отправленный потоком.
        /// 0 - все потоки заснули, 1 - поток завершил работу, 2 - поток принимает данные из буфера ввода, 3 - поток отсправляет данные в буфер вывода.
        /// </summary>
        private byte _lastSyncSignal;

        /// <summary>
        /// Инициализирует экземпляр класса указанными общими для всех создаваемых потоков регистрами.
        /// </summary>
        /// <param name="registers">Регитстры, используемые создаваемыми потоками.</param>
        /// <param name="programs">Коллекция программ, доступных для выполнения.</param>
        public RegisterThreadManager(InfiniteRegisters registers, RegisterProgramCollection programs)
        {
            if (registers == null)
                throw new ArgumentNullException("Регистры не могут быть неопределенными");
            if (programs == null)
                throw new ArgumentNullException("Коллекция программ не может быть неопределенной");

            _registers = registers;
            _programs = programs;
            _threads = new List<RegisterThread>();
            _locked = new List<LockedRegister>();
            _pausedThreads = new List<RegisterThread>();
            _debugMode = false;
            _breakPointReached = false;

            _inputBuffer = new Queue<BigInteger>();
            _outputBuffer = new Queue<BigInteger>();

            _mainLoop = new BackgroundWorker();
            _mainLoop.WorkerReportsProgress = true;
            _sync = new AutoResetEvent(false);
            _inputSync = new AutoResetEvent(false);

            _isParallel = _programs.Count > 1;
        }

        /// <summary>
        /// Определяет, находятся ли потоки в режиме отладки.
        /// </summary>
        public bool DebugMode
        {
            get { return _debugMode; }
            set
            {
                if (_debugMode == value) return;

                _debugMode = value;
                _breakPointReached = false;

                if(!_debugMode)
                    ReleasePausedThreads();
            }
        }

        /// <summary>
        /// Получает или задает точки останова МБР.
        /// </summary>
        public RegisterBreakPoint[] BreakPoints { get; set; }

        /// <summary>
        /// Определяет, выполняется ли в данный момент хотя бы один поток.
        /// </summary>
        public bool IsBusy { get { return _threads.Count > 0; } }

        /// <summary>
        /// Получает общее количество выполняемых потоков.
        /// </summary>
        public int ThreadsCount { get { return _threads.Count; } }

        /// <summary>
        /// Получает количество потоков, находящихся на паузе.
        /// </summary>
        public int ThreadsPaused { get { return _pausedThreads.Count; } }

        /// <summary>
        /// Записывает указанное значение в буфер устройства ввода ПМБР.
        /// </summary>
        /// <param name="value">Записываемое значение.</param>
        public void WriteValue(BigInteger value)
        {
            if (value < 0)
                throw new ArgumentException("Записываемое значение не может быть отрицательным");

            _inputBuffer.Enqueue(value);
        }

        /// <summary>
        /// Читает значение из буфера устройства вывода ПМБР.
        /// </summary>
        /// <returns>Полученное из буфера значение.</returns>
        public BigInteger ReadValue()
        {
            BigInteger result;
            try
            {
                result = _outputBuffer.Dequeue();
            }
            catch
            {
                throw new InvalidOperationException("Невозможно прочитать значение из буфера");
            }

            return result;
        }

        /// <summary>
        /// Получает количество данных, находящихся в выходном буфере.
        /// </summary>
        public int OutputBufferAmount
        {
            get { return _outputBuffer.Count; }
        }

        /// <summary>
        /// Возвращает информацию о текущих выполняемых потоках.
        /// </summary>
        /// <returns>ReadOnly коллекция информации о выполняемых потоках.</returns>
        public ReadOnlyCollection<ThreadInfo> GetThreadsInfo()
        {
            return _threads.ConvertAll<ThreadInfo>(x => x.Info).AsReadOnly();
        }

        /// <summary>
        /// Определяет, является ли указанная операция точкой останова МБР.
        /// </summary>
        /// <param name="program">Название программы.</param>
        /// <param name="cmd">Номер операции.</param>
        /// <returns>Истина, если команда является точкой останова, иначе - ложь.</returns>
        private bool IsBreakPoint(string program, int cmd)
        {
            if (BreakPoints == null) return false;

            if(_isParallel)
                return Array.FindIndex<RegisterBreakPoint>(BreakPoints, x => x.Command == cmd && x.Program == program ) != -1;
            else
                return Array.FindIndex<RegisterBreakPoint>(BreakPoints, x => x.Command == cmd) != -1;
        }

        /// <summary>
        /// Приостанавливает указанный поток и добавляет его в список приостановленных потоков.
        /// </summary>
        /// <param name="thread">Приостанавливаемый поток.</param>
        private void PauseThread(RegisterThread thread)
        {
            lock (_pausedThreads)
                _pausedThreads.Add(thread);

            thread.Pause();
        }

        /// <summary>
        /// Возоновляет работу всех потоков, работа которых приостановлена.
        /// </summary>
        private void ReleasePausedThreads()
        {
            List<RegisterThread> copy = _pausedThreads.ToList();
            _pausedThreads.Clear();
            copy.ForEach(x => x.Resume());
        }

        /// <summary>
        /// Потокобезопасно выполняет операцию указанным потоком.
        /// </summary>
        /// <param name="thread">Поток, выполняющий операцию.</param>
        /// <param name="op">Выполняемая операция.</param>
        /// <returns>Номер команды, к которой необходимо перейти потоку после выполнения операции.</returns>
        public BigInteger ExecuteOperation(RegisterThread thread, RegisterOperation op)
        {
            BigInteger res;

            try
            {
                if (IsBreakPoint(thread.Program.Name, thread.Info.Command))
                {
                    _breakPointReached = true;
                    _debugMode = true;
                }

                if (_debugMode)
                {
                    PauseThread(thread);
                }

                res = -1;

                Monitor.Enter(_registers);
                int i;
                switch (op.Id)
                {
                    case RegisterOperationId.Erase:
                        {
                            i = GetRegisterLocker(op.Arguments[0]);
                            if (i >= 0 && i != thread.Info.Id)
                            {
                                AddAwaiter(thread, op.Arguments[0]);
                                Monitor.Exit(_registers);
                                thread.Pause();
                                Monitor.Enter(_registers);
                            }
                            _registers.SetValue(0, op.Arguments[0]);
                            res = thread.Info.Command + 1;
                            if (!_registersChanged)
                                _registersChanged = true;
                            break;
                        }
                    case RegisterOperationId.Inc:
                        {
                            i = GetRegisterLocker(op.Arguments[0]);
                            if (i >= 0 && i != thread.Info.Id)
                            {
                                AddAwaiter(thread, op.Arguments[0]);
                                Monitor.Exit(_registers);
                                thread.Pause();
                                Monitor.Enter(_registers);
                            }

                            _registers.Increment(op.Arguments[0]);
                            res = thread.Info.Command + 1;
                            if (!_registersChanged)
                                _registersChanged = true;
                            break;
                        }
                    case RegisterOperationId.Copy:
                        {
                            i = GetRegisterLocker(op.Arguments[0]);
                            if (i >= 0 && i != thread.Info.Id)
                            {
                                AddAwaiter(thread, op.Arguments[0]);
                                Monitor.Exit(_registers);
                                thread.Pause();
                                Monitor.Enter(_registers);
                            }
                            i = GetRegisterLocker(op.Arguments[1]);
                            if (i >= 0 && i != thread.Info.Id)
                            {
                                AddAwaiter(thread, op.Arguments[1]);
                                Monitor.Exit(_registers);
                                thread.Pause();
                                Monitor.Enter(_registers);
                            }

                            _registers.SetValue(_registers.GetValue(op.Arguments[0]), op.Arguments[1]);
                            res = thread.Info.Command + 1;
                            if (!_registersChanged)
                                _registersChanged = true;
                            break;
                        }
                    case RegisterOperationId.Decision:
                        {
                            i = GetRegisterLocker(op.Arguments[0]);
                            if (i >= 0 && i != thread.Info.Id)
                            {
                                AddAwaiter(thread, op.Arguments[0]);
                                Monitor.Exit(_registers);
                                thread.Pause();
                                Monitor.Enter(_registers);
                            }
                            i = GetRegisterLocker(op.Arguments[1]);
                            if (i >= 0 && i != thread.Info.Id)
                            {
                                AddAwaiter(thread, op.Arguments[1]);
                                Monitor.Exit(_registers);
                                thread.Pause();
                                Monitor.Enter(_registers);
                            }

                            if (_registers.GetValue(op.Arguments[0]) == _registers.GetValue(op.Arguments[1]))
                                res = op.Arguments[2];
                            else
                                res = thread.Info.Command + 1;
                            break;
                        }
                    case RegisterOperationId.Start:
                        {
                            if (_stoppingProcess) break;

                            try
                            {
                                RunProgram((int)op.Arguments[0], op.Arguments[1]);
                                res = thread.Info.Command + 1;
                            }
                            catch(ThreadAbortException ex)
                            {
                                throw ex;
                            }
                            catch (Exception ex)
                            {
                                throw new Exception(string.Format("Невозможно запустить программу с id = {0}. \"{1}\"", op.Arguments[0], ex.Message), ex);
                            }
                            break;
                        }
                    case RegisterOperationId.Lock:
                        {
                            i = GetRegisterLocker(op.Arguments[0]);
                            if (i >= 0 && i != thread.Info.Id)
                            {
                                AddAwaiter(thread, op.Arguments[0]);
                                Monitor.Exit(_registers);
                                thread.Pause();
                                Monitor.Enter(_registers);
                            }
                            LockRegister(op.Arguments[0], thread);
                            res = thread.Info.Command + 1;
                            break;
                        }
                    case RegisterOperationId.Unlock:
                        {
                            i = GetRegisterLocker(op.Arguments[0]);
                            if (i >= 0 && i != thread.Info.Id)
                            {
                                AddAwaiter(thread, op.Arguments[0]);
                                Monitor.Exit(_registers);
                                thread.Pause();
                                Monitor.Enter(_registers);
                            }
                            UnlockRegister(op.Arguments[0]);
                            res = thread.Info.Command + 1;
                            break;
                        }
                    case RegisterOperationId.Read:
                        {
                            i = GetRegisterLocker(op.Arguments[0]);
                            if (i >= 0 && i != thread.Info.Id)
                            {
                                AddAwaiter(thread, op.Arguments[0]);
                                Monitor.Exit(_registers);
                                thread.Pause();
                                Monitor.Enter(_registers);
                            }

                            _registers.SetValue(Input(), op.Arguments[0]);
                            res = thread.Info.Command + 1;
                            if (!_registersChanged)
                                _registersChanged = true;
                            break;
                        }
                    case RegisterOperationId.Write:
                        {
                            i = GetRegisterLocker(op.Arguments[0]);
                            if (i >= 0 && i != thread.Info.Id)
                            {
                                AddAwaiter(thread, op.Arguments[0]);
                                Monitor.Exit(_registers);
                                thread.Pause();
                                Monitor.Enter(_registers);
                            }

                            Output(_registers.GetValue(op.Arguments[0]));
                            res = thread.Info.Command + 1;
                            break;
                        }
                }
            }
            catch(ThreadAbortException ex)
            {
                throw ex;
            }
            finally
            {
                if(Monitor.IsEntered(_registers))
                    Monitor.Exit(_registers);
            }

            return res;
        }

        /// <summary>
        /// Получает свободный уникальный идентификатор потока.
        /// </summary>
        /// <returns>Уникальный идентификатор потока.</returns>
        private int GetThreadId()
        {
            int i = 0;
            int n = _threads.Count;
            while(i < n)
            {
                if (i == _threads[i].Info.Id) i++;
                else break;
            }

            return i;
        }

        /// <summary>
        /// Выполняет следующий шаг для всех потоков, если система находится в режиме отладки.
        /// </summary>
        /// <returns>Истина, если выполнен не последний шаг алгоритма, иначе - ложь.</returns>
        public bool Step()
        {
            if (!_debugMode)
                throw new InvalidOperationException("Система должна находиться в режиме отладки");

            bool canStep = _threads.TrueForAll(x => x.Paused);
            if (!canStep) return true;

            ReleasePausedThreads();

            return _threads.Count > 0;
        }

        /// <summary>
        /// Останавливает работу всех выполняемых потоков.
        /// </summary>
        public void StopAllThreads()
        {
            if (_threads.Count == 0)
                throw new InvalidOperationException("Ни одного потока не запущено");

            _stoppingProcess = true;
            if (_debugMode)
                DebugMode = false;

            while(_threads.Count > 0)
                _threads[0].Stop();
        }

        /// <summary>
        /// Запускает выполнение входной точки программ для ПМБР.
        /// </summary>
        /// <param name="program">Программа, определяющая точку входа.</param>
        public void RunEntryPoint(int program)
        {
            if (program < 0)
                throw new ArgumentException("Уникальный идентификатор программы не может быть отрицательным");

            _registersChanged = false;
            _stoppingProcess = false;
            RunProgram(program, -1);
        }

        /// <summary>
        /// Запускает выполнение программы по указанному уникальному идентификатору в новом потоке.
        /// </summary>
        /// <param name="id">Уникальный идентификатор программы.</param>
        /// <param name="indicator">Номер регистра-индикатора выполнения потока. Если -1, то регистр с потоком не сопоставляется и поток запускается как точка входа.</param>
        private void RunProgram(int id, BigInteger indicator)
        {
            if (id < 0)
                throw new ArgumentException("Уникальный идентификатор программы не может быть отрицательным");
            if (indicator < -1)
                throw new ArgumentException("Номер регистра не может быть отрицательным");

            try
            {
                RegisterProgram program = _programs.Get(id);
                RegisterThread thread;
                if (indicator >= 0)
                    thread = new RegisterThread(this, program, indicator, GetThreadId());
                else
                    thread = new RegisterThread(this, program, GetThreadId());
                thread.OnThreadStop += ThreadStopHandler;
                thread.ThreadPaused += ThreadPausedHandler;
                lock(_threads)
                    _threads.Add(thread);

                if (_threads.Count == 1)
                {
                    _mainLoop.DoWork += MainLoop;
                    _mainLoop.ProgressChanged += MainLoopProgress;
                    _mainLoop.RunWorkerCompleted += _mainLoop_RunWorkerCompleted;
                    _mainLoop.RunWorkerAsync();
                }

                thread.Start();
            }
            catch(ThreadAbortException ex)
            {
                throw ex;
            }
            catch(Exception ex)
            {
                throw new Exception(string.Format("Ошибка запуска программы: \"{0}\"", ex.Message), ex);
            }
        }

        private void _mainLoop_RunWorkerCompleted(object sender, RunWorkerCompletedEventArgs e)
        {
            _mainLoop.ProgressChanged -= MainLoopProgress;
            _mainLoop.RunWorkerCompleted -= _mainLoop_RunWorkerCompleted;

            if (OnComplete != null)
                OnComplete(this, new RegisterThreadManagerEventArgs(_registersChanged));
        }

        private void MainLoopProgress(object sender, ProgressChangedEventArgs e)
        {
            switch(e.ProgressPercentage)
            {
                case 0:
                    if (_breakPointReached)
                    {
                        _breakPointReached = false;
                        if (BreakPointReached != null)
                            BreakPointReached(this, EventArgs.Empty);
                    }
                    if(_debugMode)
                    {
                        if (StepCompleted != null)
                            StepCompleted(this, EventArgs.Empty);
                    }
                    break;
                case 1:
                    if (OnComplete != null)
                        OnComplete(this, new RegisterThreadManagerEventArgs(_registersChanged));
                    break;
                case 2:
                    if (ValueIn != null)
                        ValueIn(this, EventArgs.Empty);
                    _inputSync.Set(); // Уведомляем поток, запросивший ввод, о том, что данные приняты.
                    break;
                case 3:
                    if (ValueOut != null)
                        ValueOut(this, EventArgs.Empty);
                    break;
            }
        }

        /// <summary>
        /// Процесс, выполняемый потоком, следящим за работой потоков ПМБР.
        /// </summary>
        private void MainLoop(object sender, DoWorkEventArgs e)
        {
            _mainLoop.DoWork -= MainLoop;

            while (_threads.Count > 0)
            {
                _sync.WaitOne(); // Ожидаем, пока не подаст сигнал какой-либо поток ПМБР.
                _mainLoop.ReportProgress(_lastSyncSignal);
            }
        }

        private void ThreadPausedHandler(object sender, EventArgs e)
        {
            if (_pausedThreads.Count + CountAwaiters() == _threads.Count)
            {
                lock (_sync)
                {
                    _lastSyncSignal = 0;
                    _sync.Set();
                }
            }
        }

        private void ThreadStopHandler(object sender, RegisterThreadEventArgs e)
        {
            RegisterThread thread = sender as RegisterThread;

            thread.OnThreadStop -= ThreadStopHandler;
            thread.ThreadPaused -= ThreadPausedHandler;

            lock (_threads)
            {
                int i = _threads.FindIndex(x => x.Info.Id == thread.Info.Id);
                _threads.RemoveAt(i);
            }
            if (_threads.Count == 0)
            {
                _locked.Clear();
                _pausedThreads.Clear();
                
                _programs = null;
                BreakPoints = null;

                lock (_sync)
                {
                    _lastSyncSignal = 1;
                    _sync.Set();
                }
            }
        }

        /// <summary>
        /// Блокирует указанный регистр указанным потоком.
        /// </summary>
        /// <param name="register">Номер блокируемого регистра.</param>
        /// <param name="thread">Блокирующий поток.</param>
        /// <returns>Истина, если регистр успешно заблокирован, иначе - ложь.</returns>
        private bool LockRegister(BigInteger register, RegisterThread thread)
        {
            bool result = true;

            lock(_threads)
            {
                int i = GetRegisterLocker(register);
                if (i >= 0) result = false;
                else
                {
                    LockedRegister reg = new LockedRegister() { Index = register, Thread = thread };
                    _locked.Insert(~i, reg);
                }
            }

            return result;
        }

        private void UnlockRegister(BigInteger register)
        {
            lock (_locked)
            {
                int i = GetRegisterLocker(register);
                if (i < 0) throw new ArgumentException("Указанный регистр не заблокирован");

                _locked[i].Awaiters.ForEach(x => x.Resume());

                _locked.RemoveAt(i);
            }
        }
        
        /// <summary>
        /// Возвращает общее количество потоков, ожидающих разблокировки регистров.
        /// </summary>
        private int CountAwaiters()
        {
            int res = 0;
            _locked.ForEach(x => res += x.Awaiters.Count);
            return res;
        }

        /// <summary>
        /// Возвращает идентификатор потока-блокиратора указанного регистра.
        /// </summary>
        /// <param name="register">Номер проверяемого регистра.</param>
        /// <returns>Неотрицательное число, обозначающее индекс потока, если регистр заблокирован, иначе - -1.</returns>
        private int GetRegisterLocker(BigInteger register)
        {
            int i = _locked.BinarySearch(new LockedRegister() { Index = register });

            return i >= 0 ? _locked[i].Thread.Info.Id : -1;
        }

        /// <summary>
        /// Добавляет поток, ожидающий разблокировки указанного регистра.
        /// </summary>
        /// <param name="thread">Поток, ожидающий разблокировки.</param>
        /// <param name="register">Регистр, разблокировка которого ожидается.</param>
        private void AddAwaiter(RegisterThread thread, BigInteger register)
        {
            int i;
            lock(_locked)
            {
                i = _locked.BinarySearch(new LockedRegister() { Index = register });
                if (i < 0) throw new ArgumentException("Регистр не заблокирован");

                int j = _locked[i].Awaiters.BinarySearch(thread);
                if (j >= 0) throw new ArgumentException("Указанный поток уже ожидает разблокировки регистра");
                _locked[i].Awaiters.Insert(~j, thread);
            }

            lock(_pausedThreads)
            {
                i = _pausedThreads.IndexOf(thread);
                if (i >= 0)
                    _pausedThreads.RemoveAt(i);
            }
        }

        /// <summary>
        /// Производит чтение числового значения с устройства ввода. 
        /// </summary>
        /// <returns>Прочитанное значение. Если с устройства ввода было
        /// подано значение, возвращает это значение, иначе - возвращает нуль.</returns>
        private BigInteger Input()
        {
            _lastSyncSignal = 2;
            _sync.Set();
            _inputSync.WaitOne();

            return _inputBuffer.Count != 0 ? _inputBuffer.Dequeue() : 0;
        }

        /// <summary>
        /// Производит запись числового значения на устройство вывода.
        /// </summary>
        /// <param name="value">Записываемое значение.</param>
        private void Output(BigInteger value)
        {
            try
            {
                _outputBuffer.Enqueue(value);
            }
            catch(Exception ex)
            {
                throw new Exception("Ошибка записи выходных данных в буфер", ex);
            }
            _lastSyncSignal = 3;
            _sync.Set();
        }
    }
}
