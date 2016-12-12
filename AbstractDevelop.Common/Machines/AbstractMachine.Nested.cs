using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;

namespace AbstractDevelop.Machines
{
    public abstract partial class AbstractMachine<OperationType, OperationCode, ArgumentType>
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

        public abstract class OperationCollectionBase :
            Collection<OperationType>, IOperationCollection
        {
            public abstract Dictionary<string, OperationCode> Aliases { get; }
            public abstract OperationType Current { get; }
            public abstract Dictionary<OperationCode, Action<ArgumentType[]>> Definitions { get; }
            public abstract OperationType Next { get; set; }

            public abstract IOperationCollection AddDefinition(OperationCode code, string alias, int maxArgumentCount, Action<ArgumentType[]> handler);

            public abstract bool Execute(OperationType operation);

            public abstract void Goto(int operationIndex);

            public abstract bool Load(IEnumerable<OperationType> source);

            public abstract void MoveNext();
        }

        internal sealed class OperationCollection :
            Collection<OperationType>, IOperationCollection
        {
            /// <summary>
            /// Функция, проверяющая корректность переданной операции
            /// </summary>
            public Func<OperationType, bool> Checker { get; set; }

            public Dictionary<OperationCode, Action<ArgumentType[]>> Definitions { get; } =
                new Dictionary<OperationCode, Action<ArgumentType[]>>();

            public OperationType Current { get { return current; } }

            public OperationType Next
            {
                get { return position < Count ? this[position] : default(OperationType); }
                set { position = IndexOf(value); }
            }

            private OperationType current;
            private int position;

            public bool Load(IEnumerable<OperationType> source)
            {
                if (Count > 0) Clear();

                foreach (var item in source)
                {
                    if (Checker?.Invoke(item) ?? true)
                        Add(item);
                    else return false;
                }

                Goto(0);
                return true;
            }

            public bool Execute(OperationType operation)
            {
                if (operation == default(OperationType))
                    return false;
                else
                {
                    var action = default(Action<ArgumentType[]>);
                    if (Definitions.TryGetValue(operation.Id, out action))
                        try
                        {
                            action(operation.Args);
                            return true;
                        }
                        // TODO: добавить вывод отладочной информации
                        catch (Exception ex) { return false; }
                    else
                        return false;
                }
            }

            public void Goto(int operationIndex)
            {
                if (operationIndex != -1)
                    position = operationIndex.CheckIndex(max: Count);
            }

            public void MoveNext()
            {
                current = Next;
                position++;
            }
        }

        public interface IOperationDefinition
        {
            string Alias { get; }

            OperationCode Code { get; }

            Action<ArgumentType[]> Handler { get; }

            int ArgumentCount { get; }
        }

        public interface IDefinitionCollection :
            ICollection<IOperationDefinition>
        {
            IOperationDefinition this[OperationCode code] { get; }

            /// <summary>
            /// Псевдонимы команд
            /// </summary>
            IDictionary<string, OperationCode> Aliases { get; }

            /// <summary>
            /// Процедуры-обработчики команд
            /// </summary>
            IDictionary<OperationCode, Action<ArgumentType[]>> Handlers { get; }

            /// <summary>
            /// Ожидаемое количество аргументов у команды определенного типа
            /// </summary>
            IDictionary<OperationCode, int> ArgumentCount { get; }

            IDefinitionCollection Add(OperationCode code, string alias, int maxArgumentCount, Action<ArgumentType[]> handler);
        }

        public interface IOperationCollection :
            ICollection<OperationType>
        {
            /// <summary>
            /// Определяет следующую операцию (инструкцию) для абстрактной машины
            /// </summary>
            OperationType Next { get; set; }

            OperationType Current { get; }

            IDefinitionCollection Definitions { get; }
           
            /// <summary>
            /// Загружает список операций из указанного источника операций
            /// </summary>
            /// <param name="source">Источник операций для загрузки</param>
            /// <returns>Возвращает true, если все оперции были успешно загружен в список</returns>
            bool Load(IEnumerable<OperationType> source);

            bool Execute(OperationType operation);

            void Goto(int operationIndex);

            void MoveNext();
        }
    }
}