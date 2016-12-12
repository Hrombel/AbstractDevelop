using System;
using System.Collections.Generic;
using System.Linq;

namespace AbstractDevelop.Machines
{
    public abstract partial class InstructionMachine<InstructionCode, ArgumentType> : AbstractMachine
        where InstructionCode : struct, IComparable
    {

        #region [Интерфейсы]

        public interface IArgumentDefinition
        {

            #region [Свойства и Поля]

            ArgumentType DefaultValue { get; }
            bool IsOptional { get; set; }
            Func<string, ArgumentType> Parser { get; }

            Func<ArgumentType, bool> Validator { get; }

            #endregion

        }

        public interface IDefinitionCollection :
            ICollection<IInstructionDefinition>
        {

            #region [Свойства и Поля]

            /// <summary>
            /// Псевдонимы команд
            /// </summary>
            IDictionary<string, InstructionCode> Aliases { get; }

            /// <summary>
            /// Ожидаемое количество аргументов у команды определенного типа
            /// </summary>
            IDictionary<InstructionCode, int> ArgumentCount { get; }

            /// <summary>
            /// Процедуры-обработчики команд
            /// </summary>
            IDictionary<InstructionCode, Action<ArgumentType[]>> Handlers { get; }

            #endregion

            #region [Методы]

            IDefinitionCollection Add(InstructionCode code, string alias, int maxArgumentCount, Action<ArgumentType[]> handler);

            #endregion

            #region [Индексаторы]

            IInstructionDefinition this[InstructionCode code] { get; }

            #endregion

        }

        public interface IInstructionCollection :
            ICollection<Instruction>
        {

            #region [Свойства и Поля]

            Instruction Current { get; }

            IDefinitionCollection Definitions { get; }

            /// <summary>
            /// Определяет следующую операцию (инструкцию) для абстрактной машины
            /// </summary>
            Instruction Next { get; set; }

            #endregion

            #region [Методы]

            bool Execute(Instruction operation);

            void Goto(int operationIndex);

            Instruction GotoNext();

            /// <summary>
            /// Загружает список операций из указанного источника операций
            /// </summary>
            /// <param name="source">Источник операций для загрузки</param>
            /// <returns>Возвращает true, если все оперции были успешно загружен в список</returns>
            bool Load(IEnumerable<Instruction> source);

            #endregion
        }

        public interface IInstructionDefinition
        {

            #region [Свойства и Поля]

            string Alias { get; }

            IList<IArgumentDefinition> Arguments { get; }
            Action<ArgumentType[]> Handler { get; }
            InstructionCode Type { get; }

            #endregion

        }

        #endregion

        #region [Классы и структуры]

        public class Instruction
        {

            #region [Свойства и Поля]

            /// <summary>
            /// Аргументы данной операции
            /// </summary>
            public virtual ArgumentType[] Arguments { get; protected set; }

            /// <summary>
            /// Идентификатор типа операции
            /// </summary>
            public virtual InstructionCode Type { get; protected set; }

            #endregion

            // TODO: переработать список методов класса <Instruction>

            #region [Методы]

            public override string ToString()
                => $"Operation of {typeof(InstructionCode).Name} with args of {typeof(ArgumentType).Name} (count = {Arguments?.Length ?? 0})";

            #endregion

            #region [Конструкторы и деструкторы]

            /// <summary>
            /// Создает экземпляр инструкции указанного типа с заданным набором аргументов
            /// </summary>
            /// <param name="code"></param>
            /// <param name="args"></param>
            public Instruction(InstructionCode code = default(InstructionCode), params ArgumentType[] args)
            {
                Type = code;
                Arguments = args;
            }

            public Instruction(InstructionCode code = default(InstructionCode), IEnumerable<ArgumentType> args = null) :
                this(code, args?.ToArray() ?? new ArgumentType[0])
            {

            }

            #endregion

        }

        /// <summary>
        /// Аргументы события, основой которого являются данные об инструкции
        /// </summary>
        public class InstructionEventArgs : EventArgs
        {
            #region [Свойства и Поля]

            public Instruction Instruction { get; set; }

            #endregion

            #region [Конструкторы и деструкторы]

            public InstructionEventArgs(Instruction instruction)
            {
                Instruction = instruction;
            }

            #endregion
        }

        #endregion

        #region [Делегаты]

        public delegate void InstructionEventHandler(AbstractMachine source, Instruction instrucion);

        #endregion

        #region [События]

        /// <summary>
        /// Действия, происходящие до\после обработки операции
        /// </summary>
        protected event InstructionEventHandler InstructionPreprocess, InstructionPostprocess;

        #endregion

        #region [Свойства и Поля]

        /// <summary>
        /// Набор инструкций и их определений, загруженный в память данной машины
        /// </summary>
        public virtual IInstructionCollection Instructions { get; }

        #endregion

        #region [Методы]

        /// <summary>
        /// Выполняет шаг (единичную инструкцию) работы машины
        /// </summary>
        /// <returns></returns>
        public override bool Step()
        {
            Activate();

            // переход ко следующей инструкции
            var currentInstruction = Instructions.GotoNext();
            var args = new InstructionEventArgs(currentInstruction);
            var isSucceded = false;
            // проверка на возможность выполнения
            if (currentInstruction == default(Instruction))
                Stop(StopReason.WrongCommand, "Команды выполнены"); // LANG
            try
            {
                OnBeforeStep(args);
                isSucceded = Instructions.Execute(currentInstruction);
                OnAfterStep(args, isSucceded);
            }
            // во время выполнения возникло исключение, порожденное частью абстрактной машины
            catch (AbstractMachineException ex) { Stop(StopReason.Exception, ex.Message); }
            catch (Exception ex) { DebugTrace.Write(ex, "error"); }

            return isSucceded;
        }

        #endregion

        // предыдущая реализация коллекции инструкций
        //internal sealed class OperationCollection :
        //   Collection<OperationType>, IOperationCollection
        //{
        //    /// <summary>
        //    /// Функция, проверяющая корректность переданной операции
        //    /// </summary>
        //    public Func<OperationType, bool> Checker { get; set; }

        //    public Dictionary<OperationCode, Action<ArgumentType[]>> Definitions { get; } =
        //        new Dictionary<OperationCode, Action<ArgumentType[]>>();

        //    public OperationType Current { get { return current; } }

        //    public OperationType Next
        //    {
        //        get { return position < Count ? this[position] : default(OperationType); }
        //        set { position = IndexOf(value); }
        //    }

        //    private OperationType current;
        //    private int position;

        //    public bool Load(IEnumerable<OperationType> source)
        //    {
        //        if (Count > 0) Clear();

        //        foreach (var item in source)
        //        {
        //            if (Checker?.Invoke(item) ?? true)
        //                Add(item);
        //            else return false;
        //        }

        //        Goto(0);
        //        return true;
        //    }

        //    public bool Execute(OperationType operation)
        //    {
        //        if (operation == default(OperationType))
        //            return false;
        //        else
        //        {
        //            var action = default(Action<ArgumentType[]>);
        //            if (Definitions.TryGetValue(operation.Id, out action))
        //                try
        //                {
        //                    action(operation.Args);
        //                    return true;
        //                }
        //                // TODO: добавить вывод отладочной информации
        //                catch (Exception ex) { return false; }
        //            else
        //                return false;
        //        }
        //    }

        //    public void Goto(int operationIndex)
        //    {
        //        if (operationIndex != -1)
        //            position = operationIndex.CheckIndex(max: Count);
        //    }

        //    public void MoveNext()
        //    {
        //        current = Next;
        //        position++;
        //    }
        //}
    }
}