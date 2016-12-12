using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AbstractDevelop.Machines
{
    public abstract class AbstractMachine<OperationCode, ArgumentType> :
        AbstractMachine<Operation<OperationCode, ArgumentType>, OperationCode, ArgumentType>
    { }
    
    public abstract partial class AbstractMachine<OperationType, OperationCode, ArgumentType> :
        ISerializable, IDisposable
        where OperationType : Operation<OperationCode, ArgumentType>
    {
        #region [События]

        #region [Список делегатов]

        public delegate void OperationEventHandler(object source, OperationType operation);

        /// <summary>
        /// Делегат, описывающий действие над операцией в абстрактной машине
        /// </summary>
        /// <param name="operation"></param>
        public delegate void OperationProcess(OperationType operation);

        #endregion [Список делегатов]

        public class ValueEventArgs
        {
            public ArgumentType Value { get; set; }
        }

        public delegate void ValueEventHandler(object sender, ValueEventArgs e);

        public event ValueEventHandler ValueIn, ValueOut;

        /// <summary>
        /// Происходит перед запуском абстрактной машины
        /// </summary>
        public event EventHandler OnStart;

        /// <summary>
        /// Происходит после запуска абстрактной машины
        /// </summary>
        public event EventHandler OnStarted;

        /// <summary>
        /// Происходит перед остановом абстрактной машины
        /// </summary>
        public event EventHandler OnStop;

        /// <summary>
        /// Происходит после останова авбстрактной машины
        /// </summary>
        public event EventHandler OnStopped;

        /// <summary>
        /// Происходит после успешного шага в пошаговом режиме исполнения команд
        /// </summary>
        public event OperationEventHandler OnStepCompleted;

        /// <summary>
        /// Происходит после неудачного (приведшего к ошибке или завершению) шага в пошаговом режиме исполнения команд
        /// </summary>
        public event OperationEventHandler OnStepFailed;

        /// <summary>
        /// Изменилось состояние абстрактной машины
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Во время выполнения команд была достигнута точка останова
        /// </summary>
        public event EventHandler BreakPointReached;

        /// <summary>
        /// Действия, происходящие до\после обработки операции
        /// </summary>
        protected event OperationProcess OperationPreprocess, OperationPostprocess;

        #endregion [События]

        public static AbstractMachineContext CurrentContext
        {
            get
            {
                // TODO
                return null;
            }

            protected set
            {

            }
        }

        public static AbstractMachineContext ForcedContext { set { } }

        public static Stack<AbstractMachineContext> PreviousContextes { get; } = new Stack<AbstractMachineContext>();

        #region [Свойства]

        public virtual ICollection<BreakPoint> BreakPoints { get; } = new BreakPointCollection();

        public virtual IOperationCollection Operations { get; } = new OperationCollection();

        #region [Состояниие]

        public abstract string Name { get; }

        public virtual bool IsActive { get; protected set; }

        #endregion [Состояниие]

        public virtual ISourceTranslator<OperationType, OperationCode, ArgumentType> SourceTranslator { get; protected set; }

        /// <summary>
        /// Поддерживаются ли данной абстрактной машиной точки останова
        /// </summary>
        public virtual bool SupportsBreakpoints { get { return true; } }

        #region [Вспомогательные свойства]

        protected virtual bool IsDisposed { get; set; }

        #endregion [Вспомогательные свойства]

        #endregion [Свойства]

        #region [Методы]

        public virtual void Start(bool byStepMode = false)
        {
            IsActive = true;
            if (byStepMode)
                Step();
            else
                while (Step()) ; //TODO: добавить автообработку перехода на следующую операцию

        }

        public virtual void Pause() 
        {
        }

        public virtual void Continue()
        {
        }

        public virtual bool Step(bool shouldGenerateEvents = true)
        {
            Operations.MoveNext();

            var isSucceded = false;
            var operation = Operations.Current; //TODO: реализовать выборку операций

            if (IsActive)
                try
                {
                    // проверка на возможность выполнения
                    if (operation == default(OperationType))
                        Stop(StopReason.WrongCommand, "Команды выполнены");
                    else
                    {
                        // вызов преобработки выполняемой операции
                        OperationPreprocess?.Invoke(operation);
                        isSucceded = Operations.Execute(operation);
                        // вызов постобработки выполненной операции
                        OperationPostprocess?.Invoke(operation);
                    }
                }
                // во время выполнения возникло исключение, порожденное частью абстрактной машины
                catch (AbstractMachineExcepton ex) { Stop(StopReason.Exception, ex.Message); }

            if (shouldGenerateEvents)
            {
                // генерация событий в зависимости от результата
                if (isSucceded)
                    OnStepCompleted?.Invoke(this, operation);
                else
                    OnStepFailed?.Invoke(this, operation);
            }
            return isSucceded;
        }

        public virtual void Stop(StopReason reason = StopReason.ExecutionStopped, string debugMessage = default(string))
        {
            if (IsActive)
            {
                OnStop?.Invoke(this, default(EventArgs));
                // TODO: доделать операцию останова
                OnStopped?.Invoke(this, default(EventArgs));
            }
            else {
                // TODO: случай, когда машина уже остановлена
            }
        }

        public ArgumentType ReadInput()
        {
            var reference = new ValueEventArgs();
            ValueIn?.Invoke(this, reference);

            return reference.Value;
        }

        public void WriteOutput(ArgumentType value)
        {
            var reference = new ValueEventArgs() { Value = value };
            ValueOut?.Invoke(this, reference);
        }

        #region [Реализация интерфейсов]

        public virtual void Dispose() 
            => IsDisposed = true;

        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
        }

        #endregion [Реализация интерфейсов]

        public override string ToString() => $"{Name}: {(IsActive ? "Отладка" : "Редактирование")}";

        #endregion [Методы]
    }
}