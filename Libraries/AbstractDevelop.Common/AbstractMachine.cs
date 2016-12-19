using System;
using System.Diagnostics;
using System.Runtime.Serialization;

using AbstractDevelop.Debug.BreakPoints;
using AbstractDevelop.Translation;

namespace AbstractDevelop.Machines
{
    /// <summary>
    ///  Представляет оболочку взаимодействия абстрактной машины (вычислительной архитектуры)
    /// </summary>
    public abstract class AbstractMachine :
        IDisposable
    {
        #region [Классы и структуры]

        public class ExecutionContext
        {
            public int CurrentIndex { get; internal set; }
        }

        public abstract class MachineState
        {

            #region [Свойства и Поля]

            public virtual bool Active { get; set; }

            #endregion

        }

        #endregion

        #region [Перечисления]

        /// <summary>
        /// Описывает возможные действия при обработке шага
        /// </summary>
        public enum StepAction
        {
            Stop,
            Continue
        }

        /// <summary>
        /// Описывает причину останнова машины
        /// </summary>
        public enum StopReason
        {
            ExecutionStopped,
            Result,
            Exception,
            WrongCommand,
            BreakPoint
        }

        #endregion

        #region [Делегаты]

        /// <summary>
        /// Делегат изменения конкретной величины, связанной с абстрактной машиной
        /// </summary>
        /// <typeparam name="ArgsType"></typeparam>
        /// <param name="source">Источник события</param>
        /// <param name="prev">Преды</param>
        /// <param name="next"></param>
        public delegate void ChangedEventHandler<ArgsType>(AbstractMachine source, ArgsType prev, ArgsType next);

        /// <summary>
        /// Автозамещающий делегат обработчика события, связанного с абстрактной машины
        /// </summary>
        /// <param name="source">Источник события</param>
        /// <param name="args">Аргументы события</param>
        /// <remarks></remarks>
        public delegate void EventHandler(AbstractMachine source, EventArgs args);

        #endregion

        #region [События]

        /// <summary>
        /// События, происходящие непосредственно перед выполнением шага обработки
        /// и после выполнения шага абстрактной машины
        /// </summary>
        public event EventHandler BeforeStep, AfterStep;

        /// <summary>
        /// Событие достижения абстрактной машиной точки останова
        /// </summary>
        public event EventHandler BreakPointReached;

        /// <summary>
        /// События, происходящие непосредственно перед запуском и после запуска абстрактной машины
        /// </summary>
        public event EventHandler Starting, Started;

        /// <summary>
        /// События, происходящие непосредственно перед остановом и после останова абстрактной машины
        /// </summary>
        public event EventHandler Stopping, Stopped;

        #endregion

        #region [Свойства и Поля]

        /// <summary>
        /// Коллекция точек останова, связанных с данной машиной
        /// </summary>
        public virtual IBreakPointCollection BreakPoints { get; }

        /// <summary>
        /// Контекст работы данной машины
        /// </summary>
        public ExecutionContext Context { get; set; }

        /// <summary>
        /// Вспомогательнный объект для отслеживания событий отладки
        /// </summary>
        public TraceListener DebugTrace { get; protected set; }

        /// <summary>
        /// Запущено ли выполнение работы машины
        /// </summary>
        public virtual bool IsActive { get; protected set; }

        /// <summary>
        /// Транслятор команд, связанный с данной машиной
        /// </summary>
        public abstract ISourceTranslator Translator { get; }

        protected MachineState state;

        #endregion

        #region [Методы]

        /// <summary>
        /// Освобождает ресурсы, связанные с данной машиной
        /// </summary>
        public virtual void Dispose()
        {
            // TODO: определить процесс освобождения ресурсов
        }

        /// <summary>
        /// Запускает работу машины в пошаговом или обычном режиме
        /// </summary>
        public virtual void Start()
        {
            OnStarting(EventArgs.Empty);
            IsActive = PrepareToStart();

            if (IsActive)
            {
                // подготовка успешна, производится выполнение
                OnStarted(EventArgs.Empty);
                // выполнять шаги до конца
                while (IsActive && OnBeforeStep(EventArgs.Empty) == StepAction.Continue && Step())
                    OnAfterStep(EventArgs.Empty);
            }
        }

        /// <summary>
        /// Выполняет шаги данной машины до остановки
        /// </summary>
        public virtual void RunToEnd()
        {
            Activate();
            while (IsActive && Step()) ;

            IsActive = false;
        }

        /// <summary>
        /// Выполняет шаг работы
        /// </summary>
        /// <returns></returns>
        public abstract bool Step();

        /// <summary>
        /// Останавливает выполнение машины
        /// </summary>
        /// <param name="reason">Причина останова</param>
        public virtual void Stop(StopReason reason = StopReason.ExecutionStopped, string debugMessage = default(string))
        {
            if (IsActive)
            {
                OnStopping(EventArgs.Empty);
                // подготовка машины к остановке
                if (PrepareToStop(reason))
                {
                    IsActive = false;
                    OnStopped(EventArgs.Empty);
                    // запись отладочного сообщения
                    DebugTrace?.Write(debugMessage);
                }
            }
            else throw new InvalidOperationException("Остановка невозможна: машина не была запущена");
        }

        /// <summary>
        /// Переводит машину в активное состояние
        /// </summary>
        protected virtual void Activate()
            =>  IsActive = true;
        /// <summary>
        /// Вызывает обработку события <see cref="AfterStep"/>
        /// </summary>
        /// <param name="args">Аргументы для обработки</param>
        protected virtual void OnAfterStep(EventArgs args, bool isSucceded = true)
            => AfterStep?.Invoke(this, args);

        /// <summary>
        /// Вызывает обработку события <see cref="BeforeStep"/>
        /// </summary>
        /// <param name="args">Аргументы для обработки</param>
        protected virtual StepAction OnBeforeStep(EventArgs args)
        {
            BeforeStep?.Invoke(this, args);

            // TODO: выполнить проверку на наличие точки останова
            foreach (var point in BreakPoints)
                if (point.IsReached)
                    return StepAction.Stop;

            return StepAction.Continue;
        }

        /// <summary>
        /// Вызывает обработку события <see cref="BreakPointReached"/>
        /// </summary>
        /// <param name="args">Аргументы для обработки</param>
        protected virtual void OnBreakPointReached(EventArgs args)
            => BreakPointReached?.Invoke(this, args);

        /// <summary>
        /// Вызывает обработку события <see cref="Started"/>
        /// </summary>
        /// <param name="args">Аргументы для обработки</param>
        protected virtual void OnStarted(EventArgs args)
            => Started?.Invoke(this, args);

        /// <summary>
        /// Вызывает обработку события <see cref="Starting"/>
        /// </summary>
        /// <param name="args">Аргументы для обработки</param>
        protected virtual void OnStarting(EventArgs args)
            => Starting?.Invoke(this, args);

        /// <summary>
        /// Вызывает обработку события <see cref="Stopped"/>
        /// </summary>
        /// <param name="args">Аргументы для обработки</param>
        protected virtual void OnStopped(EventArgs args)
            => Stopped?.Invoke(this, args);

        /// <summary>
        /// Вызывает обработку события <see cref="Stopping"/>
        /// </summary>
        /// <param name="args">Аргументы для обработки</param>
        protected virtual void OnStopping(EventArgs args)
            => Stopping?.Invoke(this, args);

        /// <summary>
        /// Производит подготовку машины к запуску
        /// </summary>
        /// <returns></returns>
        protected virtual bool PrepareToStart() => true;

        /// <summary>
        /// Производит подготовку машины к останову
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        protected virtual bool PrepareToStop(StopReason reason) => true;

        #endregion

        protected AbstractMachine()
        {
            BreakPoints = new BreakPointCollection(this);
        }
    }
}