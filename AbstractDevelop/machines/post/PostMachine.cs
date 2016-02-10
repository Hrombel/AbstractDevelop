using AbstractDevelop.errors.runtime;
using AbstractDevelop.machines.tape;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Numerics;
using System.Text;
using System.Threading.Tasks;
using System.Timers;

namespace AbstractDevelop.machines.post
{
    /// <summary>
    /// Представляет модель машины Поста.
    /// </summary>
    [Serializable]
    public class PostMachine : IAbstractMachine
    {
        /// <summary>
        /// Возникает после выполнения операции.
        /// </summary>
        public event EventHandler<PostOperationExecutedEventArgs> OnOperationExecuted;
        /// <summary>
        /// Возникает после останова машины Поста.
        /// </summary>
        public event EventHandler<PostMachineStopEventArgs> OnMachineStopped;

        private Tape _tape;
        private BigInteger _pos;
        [NonSerialized]
        private List<Operation> _ops;
        [NonSerialized]
        private int _opNum;

        /// <summary>
        /// Инициализирует машину Поста пустой лентой.
        /// </summary>
        public PostMachine()
        {
            _tape = new Tape(TapeType.TwoStated);
            _pos = 0;
            _ops = null;
            _opNum = 0;
        }

        /// <summary>
        /// Получает номер выполняемой операции.
        /// </summary>
        public int CurrentOperation { get { return _opNum; } }

        /// <summary>
        /// Получает текущее состояние ленты.
        /// </summary>
        public Tape Tape { get { return _tape; } }

        /// <summary>
        /// Получает или задает текущее положение каретки.
        /// </summary>
        public BigInteger Position
        {
            get
            {
                return _pos;
            }
            set
            {
                _pos = value;
            }
        }

        /// <summary>
        /// Выполняет операцию машины Поста.
        /// </summary>
        /// <param name="operation">Операция машины Поста.</param>
        private void ExecuteOperation(Operation operation)
        {
            if (operation == null)
                throw new ArgumentNullException("Выполняемая операция не может быть неопределенной");
            if (!(operation is PostOperation))
                throw new ArgumentException("Выполняемая операция должна быть предназначена для конкретного исполнителя");

            int n = (operation as PostOperation).Arguments.Length;
            for (int i = 0; i < n; i++ )
            {
                if ((operation as PostOperation).Arguments[i] < 1)
                {
                    ExecutionAbstractException outNumEx = new ExecutionAbstractException("Попытка перехода к операции с отрицательным номером");
                    outNumEx.Data.Add(0, PostMachineStopReason.OUT_OF_OPERATION_NUMBER);
                    throw outNumEx;
                }
            }

            switch ((operation as PostOperation).Id)
            {
                case PostOperationId.Left:
                    {
                        _pos--;
                        break;
                    }
                case PostOperationId.Right:
                    {
                        _pos++;
                        break;
                    }
                case PostOperationId.Place:
                    {
                        if (_tape.GetValue(_pos) != 0)
                        {
                            ExecutionAbstractException setLblEx = new ExecutionAbstractException("Попытка установки метки в отмеченную ячейку");
                            setLblEx.Data.Add(0, PostMachineStopReason.SET_EXISTING_LABEL);
                            throw setLblEx;
                        }
                        _tape.SetValue(_pos, 1);
                        break;
                    }
                case PostOperationId.Erase:
                    {
                        if (_tape.GetValue(_pos) == 0)
                        {
                            ExecutionAbstractException remLblEx = new ExecutionAbstractException("Попытка стирания несуществующей метки");
                            remLblEx.Data.Add(0, PostMachineStopReason.REMOVE_NULL_LABEL);
                            throw remLblEx;
                        }
                        _tape.SetValue(_pos, 0);
                        break;
                    }
                case PostOperationId.Stop:
                    {
                        Stop(PostMachineStopReason.STOP_OPERATION);
                        break;
                    }
            }

            if((operation as PostOperation).Id == PostOperationId.Decision)
            {
                if (_tape.GetValue(_pos) != 0)
                    _opNum = (operation as PostOperation).Arguments[0];
                else
                    _opNum = (operation as PostOperation).Arguments[1];
            }
            else if ((operation as PostOperation).Id != PostOperationId.Stop)
            {
                if ((operation as PostOperation).Arguments.Length == 0)
                    _opNum++;
                else
                    _opNum = (operation as PostOperation).Arguments[0];
            }

            if (OnOperationExecuted != null)
                OnOperationExecuted(this, new PostOperationExecutedEventArgs((operation as PostOperation).Id));
        }

        /// <summary>
        /// Определяет, существует ли операция под указанным номером.
        /// </summary>
        /// <param name="num">Номер(отсчитывая от единицы) проверяемой операции.</param>
        /// <returns>Истина - операция под указанным номером существует, иначе - не существует.</returns>
        private bool OperationExists(int num)
        {
            if (num < 1) throw new ArgumentException("Номер операции не может быть меньше единицы");
            if (_ops == null) throw new Exception("Невозможно проверить наличие операции, поскольку не задан их список");

            return num <= _ops.Count;
        }

        /// <summary>
        /// Переводит машину Поста в режим пошагового выполнения операций.
        /// </summary>
        /// <param name="ops">Список операций для машины Поста.</param>
        public void StartManual(List<Operation> ops)
        {
            if (ops == null)
                throw new ArgumentNullException("Список выполняемых команд не может быть неопределенным");
            if (_opNum != 0)
                throw new Exception("Невозможно запустить машину Поста, поскольку она уже запущена");

            _ops = ops;
            _opNum = 1;
        }

        /// <summary>
        /// Выполняет следующую операцию из текущего списка операций.
        /// </summary>
        /// <returns>Истина, если операция выполнена успешно и не вызвала остановку машины, иначе - возникла ошибка, которая остановила машину, либо достигнут конец программы.</returns>
        public bool Forward()
        {
            if (_ops == null)
                throw new Exception("Невозможно выполнить следующую операцию, поскольку машина не запущена");

            bool res = true;

            if (OperationExists(_opNum))
            {
                try
                {
                    ExecuteOperation(_ops[_opNum - 1]);
                }
                catch (ExecutionAbstractException ex)
                {
                    res = false;
                    Stop((PostMachineStopReason)ex.Data[0]);
                }
            }
            else
            {
                res = false;
                Stop(PostMachineStopReason.OUT_OF_OPERATION_NUMBER);
            }

            return res;
        }

        /// <summary>
        /// Запускает выполнение операций без задержек между ними.
        /// </summary>
        /// <param name="ops">Список операций для машины Поста.</param>
        public void Start(List<Operation> ops)
        {
            if (ops == null)
                throw new ArgumentNullException("Список выполняемых команд не может быть неопределенным");
            if (_opNum != 0)
                throw new Exception("Невозможно запустить машину Поста, поскольку она уже запущена");

            _ops = ops;
            _opNum = 1;
            

            while(_opNum != 0) Forward();
        }

        /// <summary>
        /// Останавливает работу машины Поста.
        /// </summary>
        public void Stop()
        {
            Stop(PostMachineStopReason.USER_INTERRUPT);
        }

        private void Stop(PostMachineStopReason reason)
        {
            if (_ops == null)
                throw new Exception("Невозможно остановить работу машины Поста. Машина итак не запущена");

            _ops = null;
            _opNum = 0;

            if (OnMachineStopped != null)
                OnMachineStopped(this, new PostMachineStopEventArgs(reason));
        }
    }
}
