using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;
using System.Runtime.Serialization;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Machines
{
    public abstract class AbstractMachine<OperationType, OperationCode, ArgumentType> :
        ISerializable, IDisposable
        where OperationType : Operation<OperationCode, ArgumentType>
    {
        internal sealed class BreakPointCollection :
            Collection<BreakPoint>
        {
            public EventHandler<BreakPoint> Added, Removed;

            public new void Add(BreakPoint breakPoint)
            {
                if (breakPoint.IsValid(false) && !Contains(breakPoint))
                {
                    base.Add(breakPoint);
                    Added?.Invoke(this, breakPoint);
                }
            }

            public new void Remove(BreakPoint breakPoint)
            {
                if (breakPoint.IsValid(false) && Contains(breakPoint))
                {
                    base.Remove(breakPoint);
                    Removed?.Invoke(this, breakPoint);
                }
            }
        }


        #region [События]

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
        public event EventHandler OnStepCompleted;
        /// <summary>
        /// Происходит после неудачного (приведшего к ошибке или завершению) шага в пошаговом режиме исполнения команд
        /// </summary>
        public event EventHandler OnStepFailed;

        /// <summary>
        /// Изменилось состояние абстрактной машины
        /// </summary>
        public event EventHandler StateChanged;

        /// <summary>
        /// Во время выполнения команд была достигнута точка останова
        /// </summary>
        public event EventHandler BreakPointReached;
        #endregion

        #region [Свойства]

        public virtual ICollection<BreakPoint> BreakPoints { get; } = new BreakPointCollection();

        /// <summary>
        /// Поддерживаются ли данной абстрактной машиной точки останова
        /// </summary>
        public virtual bool SupportsBreakpoints { get { return true; } }

        protected virtual bool IsDisposed { get; set; }
        #endregion

        #region [Методы]

        public virtual void Dispose() => IsDisposed = true;
       
        public void GetObjectData(SerializationInfo info, StreamingContext context)
        {
            
        }

        #endregion

    }
}
