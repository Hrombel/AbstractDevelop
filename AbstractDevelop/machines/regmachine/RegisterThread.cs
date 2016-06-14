using System;
using System.Numerics;
using System.Threading;
using RegisterOperation = AbstractDevelop.Machines.Operation<AbstractDevelop.machines.regmachine.RegisterOperationId, System.Numerics.BigInteger>;

namespace AbstractDevelop.machines.regmachine
{
    /// <summary>
    /// Представляет поток параллелньной машины с бесконечными регистрами, выполняющий сопоставленную
    /// с ним программу.
    /// </summary>
    public class RegisterThread : IComparable<RegisterThread>, IComparable
    {
        /// <summary>
        /// Возникает после остановки работы потока.
        /// </summary>
        public event EventHandler<RegisterThreadEventArgs> OnThreadStop;

        /// <summary>
        /// Возникает после засыпания потока.
        /// </summary>
        public event EventHandler ThreadPaused;

        private ThreadInfo _info;
        private BigInteger _out;
        private RegisterProgram _prog;
        private RegisterThreadManager _parent;
        private Thread _systemThread;

        /// <summary>
        /// Инициализирует экземпляр потока как точку входа программ для ПМБР.
        /// </summary>
        /// <param name="parent">Средство, создавшее этот поток.</param>
        /// <param name="program">Программа, выполняемая потоком.</param>
        /// <param name="id">Уникальный идентификатор потока.</param>
        public RegisterThread(RegisterThreadManager parent, RegisterProgram program, int id)
        {
            if (parent == null)
                throw new ArgumentNullException("");
            if (program == null)
                throw new ArgumentNullException("Программа не может быть неопределенной");
            if (id < 0)
                throw new ArgumentException("Уникальный идентификатор потока не может быть отрицательным");

            _parent = parent;
            _prog = program;
            _info = new ThreadInfo(id, program.Name, 0);
            _out = -1;
        }

        /// <summary>
        /// Инициализирует экземпляр потока указанными параметрами.
        /// </summary>
        /// <param name="parent">Средство, создавшее этот поток.</param>
        /// <param name="program">Программа, выполняемая потоком.</param>
        /// <param name="outRegister">Регистр-индикатор выполнения потока.</param>
        /// <param name="id">Уникальный идентификатор потока.</param>
        public RegisterThread(RegisterThreadManager parent, RegisterProgram program, BigInteger outRegister, int id)
        {
            if (parent == null)
                throw new ArgumentNullException("");
            if (program == null)
                throw new ArgumentNullException("Программа не может быть неопределенной");
            if (outRegister < 0)
                throw new ArgumentException("Регистр не может быть отрицательным");
            if (id < 0)
                throw new ArgumentException("Уникальный идентификатор потока не может быть отрицательным");

            _parent = parent;
            _prog = program;
            _out = outRegister;
            _info = new ThreadInfo(id, program.Name, 0);
        }

        /// <summary>
        /// Получает базовую информацию о текущем потоке МБР.
        /// </summary>
        public ThreadInfo Info { get { return _info; } }

        /// <summary>
        /// Получает номер регистра-индикатора выполнения потока.
        /// </summary>
        public BigInteger OutRegister { get { return _out; } }

        /// <summary>
        /// Получает ссылку на программу, выполняемую потоком.
        /// </summary>
        public RegisterProgram Program { get { return _prog; } }

        /// <summary>
        /// Выполняет указанную операцию.
        /// </summary>
        /// <param name="op">Выполняемая операция.</param>
        private void ExecuteOperation(RegisterOperation op)
        {
            _info.Command = (int)_parent.ExecuteOperation(this, op);
        }

        /// <summary>
        /// Запускает выполнение программы потоком.
        /// </summary>
        public void Start()
        {
            if (_systemThread != null)
                throw new InvalidOperationException("Поток уже запущен");

            Thread thread = new Thread(ThreadJob);
            thread.IsBackground = true;
            thread.Start();

            _systemThread = thread;
        }

        private void ThreadJob()
        {
            try
            {
                _systemThread = Thread.CurrentThread;

                if (_out != -1)
                    _parent.ExecuteOperation(this, RegisterOperation.Create(RegisterOperationId.Erase, new BigInteger[] { _out }));
                _info.Command = 1;
                int cmd;

                while (_info.Command != 0)
                {
                    if (_info.Command <= _prog.Operations.Length)
                    {
                        cmd = _info.Command - 1;
                        ExecuteOperation(_prog.Operations[cmd]);
                    }
                    else
                    {
                        _info.Command = 0;
                        EndOperation();
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine(ex.Message);
            }
            finally
            {
                _info.Command = 0;
                if (OnThreadStop != null)
                    OnThreadStop(this, new RegisterThreadEventArgs());
                _systemThread = null;
            }
        }

        /// <summary>
        /// Определяет, находится остановлен ли поток в данный момент.
        /// </summary>
        public bool Paused { get { return _systemThread.ThreadState.HasFlag(ThreadState.Suspended); } }

        /// <summary>
        /// Переводит поток в режим ожидания.
        /// </summary>
        public void Pause()
        {
            if (ThreadPaused != null)
                ThreadPaused(this, EventArgs.Empty);
            _systemThread.Suspend();
        }

        /// <summary>
        /// Возобновляет работу потока после его установки в режим ожидания.
        /// </summary>
        public void Resume()
        {
            _systemThread.Resume();
        }

        /// <summary>
        /// Выполняет завершающие операции и уведомляет фоновый процесс о необходимости завершения потока.
        /// </summary>
        private void EndOperation()
        {
            _info.Command = 0;
            if (_out != -1)
            {
                _parent.ExecuteOperation(this, RegisterOperation.Create(RegisterOperationId.Erase, new BigInteger[] { _out }));
                _parent.ExecuteOperation(this, RegisterOperation.Create(RegisterOperationId.Inc, new BigInteger[] { _out }));
            }
        }

        /// <summary>
        /// Останавливает выполнение потока.
        /// </summary>
        public void Stop()
        {
            if (_systemThread == null)
                throw new InvalidOperationException("Поток уже остановлен");

            _info.Command = 0;
            if (Paused) Resume();
            _systemThread.Abort();
            if (_systemThread != null)
                _systemThread.Join();
        }

        public int CompareTo(RegisterThread other)
        {
            return _info.Id.CompareTo(other._info.Id);
        }

        public int CompareTo(object obj)
        {
            return CompareTo(obj as RegisterThread);
        }
    }
}