using System;
using System.Collections.Generic;
using System.Runtime.Serialization;

namespace AbstractDevelop.Machines
{
    public abstract class AbstractMachine<OperationCode, ArgumentType, StorageType> :
        AbstractMachine<Operation<OperationCode, ArgumentType>, OperationCode, ArgumentType, StorageType> { }
   
    public abstract partial class AbstractMachine<OperationType, OperationCode, ArgumentType, StorageType> :
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

        #endregion

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

        #endregion

        #region [Свойства]

        protected virtual Dictionary<OperationCode, Action<ArgumentType[]>> OperationHandlers { get; }

        public virtual ICollection<BreakPoint> BreakPoints { get; } = new BreakPointCollection();

        public virtual IOperationCollection Operations { get; } = new OperationCollection();
        
        #region [Состояниие]

        public abstract string Name { get; }

        public virtual bool IsActive { get; protected set; }

        #endregion

        /// <summary>
        /// Поддерживаются ли данной абстрактной машиной точки останова
        /// </summary>
        public virtual bool SupportsBreakpoints { get { return true; } }

       

        #region [Вспомогательные свойства]

        protected virtual List<StorageType> Storages { get; } // TODO: реализовать хранилище 

        protected virtual bool IsDisposed { get; set; }
       
        #endregion

        #endregion

        #region [Методы]

        public virtual void Start(bool byStepMode = false)
        {
            if (byStepMode)
                Step();
            else
                while (Step()) ; //TODO: добавить автообработку перехода на следующую операцию
        }

        public virtual bool Step(bool shouldGenerateEvents = true)
        {
            var isSucceded = false;
            var operation = default(OperationType); //TODO: реализовать выборку операций

            if (IsActive)
                try
                {
                    // вызов преобработки выполняемой операции
                    OperationPreprocess?.Invoke(operation);
                    // проверка на возможность выполнения
                    if (operation == default(OperationType))
                        Stop(StopReason.WrongCommand, "Неизвестная операция (null-ref)");
                    else
                        isSucceded = Operations.Execute(operation);
                    // вызов постобработки выполненной операции
                    OperationPostprocess?.Invoke(operation);
                }
                // во время выполнения возникло исключение, порожденное частью абстрактной машины
                catch (AbstractMachineExcepton ex) { Stop(StopReason.Exception, ex.Message); }

            if (shouldGenerateEvents) {
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
            else { }

        }

        #region [Реализация интерфейсов]

        public virtual void Dispose() => IsDisposed = true;
       
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
        }

        #endregion

        public override string ToString() => $"{Name}: {(IsActive? "Отладки" : "Редактирование")}";
       
        #endregion

    }
}
