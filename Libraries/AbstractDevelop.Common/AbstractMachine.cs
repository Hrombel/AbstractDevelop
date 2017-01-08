using System;
using System.Diagnostics;
using System.Linq;

using AbstractDevelop.Debug.BreakPoints;
using AbstractDevelop.Translation;

namespace AbstractDevelop.Machines
{
    #region [Перечисления]

    /// <summary>
    /// Описывает возможные действия при обработке шага
    /// </summary>
    public enum ExecutionAction
    {
        Stop,
        Continue,
        BreakPoint
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

    /// <summary>
    ///  Представляет оболочку взаимодействия абстрактной машины (вычислительной архитектуры)
    /// </summary>
    public abstract class AbstractMachine :
        IDisposable
    {

        #region [Классы и структуры]

        public class ExecutionContext
        {
            #region [Свойства и Поля]

            public int CurrentIndex { get; internal set; }

            #endregion
        }

        public abstract class MachineState
        {

            #region [Свойства и Поля]

            public virtual bool Active { get; set; }

            #endregion

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

        // TODO
        public delegate void StopEventHandler(AbstractMachine source, StopReason reason);

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
        public event StopEventHandler Stopping, Stopped;

        #endregion

        #region [Свойства и Поля]

        /// <summary>
        /// Коллекция точек останова, связанных с данной машиной
        /// </summary>
        public virtual IBreakPointCollection BreakPoints { get; } =
            new BreakPointCollection();

        /// <summary>
        /// Контекст работы данной машины
        /// </summary>
        public ExecutionContext Context { get; set; }

        /// <summary>
        /// Вспомогательнный объект для отслеживания событий отладки
        /// </summary>
        public TraceListener DebugTrace { get; protected set; }

        public Action<AbstractMachine> RefreshAction { get; set; }

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
        /// Переводит машину в активное состояние
        /// </summary>
        public virtual void Activate()
            => IsActive = true;

        /// <summary>
        /// Освобождает ресурсы, связанные с данной машиной
        /// </summary>
        public virtual void Dispose()
        {
            // TODO: определить процесс освобождения ресурсов
        }

        /// <summary>
        /// Выполняет шаги данной машины до остановки
        /// </summary>
        public virtual void RunToEnd(bool breakpointsActive = true)
        {
            while (IsActive && Step(breakpointsActive))
                RefreshAction?.Invoke(this);
        }

        /// <summary>
        /// Запускает работу машины в пошаговом или обычном режиме
        /// </summary>
        public virtual void Start()
        {
            OnStarting(EventArgs.Empty);

            if (IsActive = PrepareToStart())
            {
                // подготовка успешна, производится выполнение
                OnStarted(EventArgs.Empty);
                // выполнять шаги до конца
                RunToEnd();
            }
        }
        /// <summary>
        /// Выполняет шаг работы
        /// </summary>
        /// <returns>Значение, указывающее возможность продолжение выполнения работы машины</returns>
        public virtual bool Step(bool breakpointsActive)
        {
            try
            {
                return OnBeforeStep(EventArgs.Empty, breakpointsActive).Check(CanContinue) && 
                    PerformStep().Do(result => OnAfterStep(EventArgs.Empty, result, breakpointsActive).Check(CanContinue)).
                    Decision(@false: () => Stop(StopReason.Result));
            }
            // во время выполнения возникло исключение, порожденное частью абстрактной машины
            catch (AbstractMachineException ex)
            {
                return Stop(StopReason.Exception, ex.Message).Check(CanContinue);
            }
        }

        /// <summary>
        /// Останавливает выполнение машины
        /// </summary>
        /// <param name="reason">Причина останова</param>
        public virtual ExecutionAction Stop(StopReason reason = StopReason.ExecutionStopped, string debugMessage = default(string))
        {
            if (reason == StopReason.BreakPoint)
                return ExecutionAction.BreakPoint;

            if (IsActive)
            {
                OnStopping(reason);
                // подготовка машины к остановке
                if (PrepareToStop(reason))
                {
                    IsActive = false;
                    OnStopped(reason);
                    // запись отладочного сообщения
                    DebugTrace?.Write(debugMessage);
                    return ExecutionAction.Stop;
                }
            }

            return ExecutionAction.Continue;
        }

        /// <summary>
        /// Определяет, возможно ли дальнейшее функционирование абстрактной машины,
        /// вернувшей следующий код действия
        /// </summary>
        /// <param name="actionCode">Код действия для проверки</param>
        /// <returns></returns>
        protected virtual bool CanContinue(ExecutionAction actionCode)
            => actionCode == ExecutionAction.Continue;

        /// <summary>
        /// Определяет, была ли достигнута хоть одна из точек останова
        /// </summary>
        /// <param name="breakpointsActive">Активны ли точки останова</param>
        /// <param name="type">Тип точек останова для проверки</param>
        /// <returns></returns>
        protected virtual bool IsBreakPointReached(bool breakpointsActive, BreakPointType type = BreakPointType.All)
            => breakpointsActive && BreakPoints.Any(bp => bp.OfType(type) && bp.IsReached, out var breakPoint).
                Decision(() => OnBreakPointReached(new BreakPointEventArgs(breakPoint)));

        /// <summary>
        /// Вызывает обработку события <see cref="AfterStep"/>
        /// </summary>
        /// <param name="args">Аргументы для обработки</param>
        protected virtual ExecutionAction OnAfterStep(EventArgs args, bool breakpointsActive, bool isSucceded = true)
        {
            AfterStep?.Invoke(this, args);

            return IsBreakPointReached(breakpointsActive, BreakPointType.PostAction) ?
                ExecutionAction.BreakPoint : ExecutionAction.Continue;
        }

        /// <summary>
        /// Вызывает обработку события <see cref="BeforeStep"/>
        /// </summary>
        /// <param name="args">Аргументы для обработки</param>
        protected virtual ExecutionAction OnBeforeStep(EventArgs args, bool breakpointsActive)
        {
            BeforeStep?.Invoke(this, args);

            return IsBreakPointReached(breakpointsActive, BreakPointType.PreAction) ? 
                ExecutionAction.BreakPoint : ExecutionAction.Continue;
        }

        /// <summary>
        /// Вызывает обработку события <see cref="BreakPointReached"/>
        /// </summary>
        /// <param name="args">Аргументы для обработки</param>
        protected virtual bool OnBreakPointReached(BreakPointEventArgs args)
            => !Stop(StopReason.BreakPoint).Check(CanContinue) && BreakPointReached.Try(onError: null, args: new object[] { this, args });

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
        protected virtual void OnStopped(StopReason reason)
            => Stopped?.Invoke(this, reason);

        /// <summary>
        /// Вызывает обработку события <see cref="Stopping"/>
        /// </summary>
        /// <param name="args">Аргументы для обработки</param>
        protected virtual void OnStopping(StopReason reason)
            => Stopping?.Invoke(this, reason);

        /// <summary>
        /// Выполняет шаг работы машины
        /// </summary>
        /// <returns></returns>
        protected abstract bool PerformStep();

        /// <summary>
        /// Производит подготовку машины к запуску
        /// </summary>
        /// <returns></returns>
        protected virtual bool PrepareToStart() => !IsActive || Step(false);

        /// <summary>
        /// Производит подготовку машины к останову
        /// </summary>
        /// <param name="reason"></param>
        /// <returns></returns>
        protected virtual bool PrepareToStop(StopReason reason) => true;

        #endregion
    }
}