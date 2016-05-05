using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AbstractDevelop.Machines
{
    public abstract partial class AbstractMachine<OperationType, OperationCode, ArgumentType, StorageType>
    {
        public enum StopReason
        {
            ExecutionStopped,
            Result,
            Exception,
            WrongCommand
        }

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

        internal sealed class OperationCollection :
            Collection<OperationType>, IOperationCollection
        {
            public ArgumentType Next { get; set; }

            public Func<OperationType, bool> Checker { get; set; }

            public bool Load(IEnumerable<OperationType> source)
            {
                if (Count > 0) Clear();

                foreach (var item in source)
                {
                    if (Checker?.Invoke(item) ?? false)
                        Add(item);
                    else return false;
                }
                return true;
            }

            public bool Execute(OperationType operation)
            {
                // TODO: не реализовано ни фи га
                return true;
            }
        }

        public interface IOperationCollection :
            ICollection<OperationType>
        {
            /// <summary>
            /// Определяет следующую операцию (инструкцию) для абстрактной машины
            /// </summary>
            ArgumentType Next { get; set; }
            Dictionary<OperationCode, Action<ArgumentType[]>> Definitions { get; set; }

            /// <summary>
            /// Загружает список операций из указанного источника операций
            /// </summary>
            /// <param name="source">Источник операций для загрузки</param>
            /// <returns>Возвращает true, если все оперции были успешно загружен в список</returns>
            bool Load(IEnumerable<OperationType> source);

            bool Execute(OperationType operation);
        }
    }
}
