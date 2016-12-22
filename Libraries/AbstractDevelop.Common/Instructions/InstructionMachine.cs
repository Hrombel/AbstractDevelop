using System;
using System.Collections.Generic;
using System.Linq;

using AbstractDevelop.Translation;
using System.Collections.ObjectModel;
using System.Collections;

namespace AbstractDevelop.Machines
{
    public abstract partial class InstructionMachine<InstructionCode, ArgumentType> : AbstractMachine
        where InstructionCode : struct, IComparable
    {
        #region [Интерфейсы]

        /// <summary>
        /// Определение формата аргумента инструкции
        /// </summary>
        public interface IArgumentDefinition
        {

            #region [Свойства и Поля]

            /// <summary>
            /// Значение по умолчанию
            /// </summary>
            ArgumentType DefaultValue { get; }

            /// <summary>
            /// Являтся ли аргумент опциональным
            /// </summary>
            bool IsOptional { get; set; }

            /// <summary>
            /// Функция преобразования из строкового представления данных в объектоное
            /// </summary>
            Func<string, TranslationState, IArgumentDefinition, ArgumentType> Parser { get; }

            /// <summary>
            /// Функция проверки значения аргумента на допустимость
            /// </summary>
            Func<ArgumentType, TranslationState, bool> Validator { get; }

            #endregion

        }

        /// <summary>
        /// Определение набора инструкций абстрактной машины
        /// </summary>
        public interface IInstructionCollection :
            ICollection<Instruction>
        {

            #region [События]

            /// <summary>
            /// Событие, возникающее при обработке инструкции
            /// </summary>
            event Action<Instruction> OnExecution;

            /// <summary>
            /// Событие, возникающее при вынужденном переходе на инструкцию
            /// </summary>
            event Action<int> OnGoto;

            #endregion

            #region [Свойства и Поля]

            /// <summary>
            /// Инструкция, выполняемая в данный момент
            /// </summary>
            Instruction Current { get; }

            /// <summary>
            /// Индекс инструкции, выполняемой в данный момент
            /// </summary>
            int CurrentIndex { get; }

            /// <summary>
            /// Набор определений инструкций, используемый для их выполнения
            /// </summary>
            InstructionDefinitions Definitions { get; set; }

            /// <summary>
            /// Индекс инструкции, которая будет выполнена на следующем шаге,
            /// либо null, если задано поведение по умолчанию
            /// </summary>
            int? NextIndex { get; }

            #endregion

            #region [Методы]

            /// <summary>
            /// Запускает действия, связанные с указанной инструкцией
            /// </summary>
            /// <param name="instruction">Инструкция для запуска</param>
            /// <returns></returns>
            bool Execute(Instruction operation);

            /// <summary>
            /// Переходит на инструкцию с указанным индексом
            /// </summary>
            /// <param name="operationIndex">Индекс инструкции для перехода</param>
            void Goto(int operationIndex);

            /// <summary>
            /// Переходит ко следующей по порядку или заданию инструкции
            /// </summary>
            /// <returns></returns>
            Instruction GotoNext();

            /// <summary>
            /// Загружает список операций из указанного источника операций
            /// </summary>
            /// <param name="source">Источник операций для загрузки</param>
            /// <returns>Возвращает true, если все оперции были успешно загружен в список</returns>
            bool Load(IEnumerable<Instruction> source);

            /// <summary>
            /// Загружает набор инструкций в данную коллекцию
            /// </summary>
            /// <param name="source">Набор инструкций для загрузки</param>
            /// <returns></returns>
            void Reset();

            #endregion
        }

        #endregion

        #region [Классы и структуры]

        /// <summary>
        /// Представляет инструкцию абстрактной машины
        /// </summary>
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

            #region [Методы]

            public override string ToString()
                => Translate.Key("InstructionDescription", format: Translate.Format(Type, Arguments?.Length ?? 0));

            #endregion

            #region [Конструкторы и деструкторы]

            /// <summary>
            /// Создает экземпляр инструкции указанного типа с заданным набором аргументов
            /// </summary>
            /// <param name="code">Тип инструкции</param>
            /// <param name="args">Значения аргументов инструкции</param>
            public Instruction(InstructionCode code = default(InstructionCode), params ArgumentType[] args)
            {
                Type = code;
                Arguments = args;
            }

            /// <summary>
            /// Создает экземпляр инструкции указанного типа с заданным набором аргументов
            /// </summary>
            /// <param name="code">Тип инструкции</param>
            /// <param name="args">Значения аргументов инструкции</param>
            public Instruction(InstructionCode code = default(InstructionCode), IEnumerable<ArgumentType> args = null) :
                this(code, args?.ToArray() ?? new ArgumentType[0])
            { }
           
            #endregion

        }

        /// <summary>
        /// Набор инструкций абстрактной машины, основанной на механизме инструкций
        /// </summary>
        public class InstructionCollection :
            Collection<Instruction>, IInstructionCollection
        {
            #region [События]

            /// <summary>
            /// Событие, возникающее при обработке инструкции
            /// </summary>
            public event Action<Instruction> OnExecution;

            /// <summary>
            /// Событие, возникающее при вынужденном переходе на инструкцию
            /// </summary>
            public event Action<int> OnGoto;

            #endregion

            #region [Свойства и Поля]

            /// <summary>
            /// Инструкция, выполняемая в данный момент
            /// </summary>
            public Instruction Current =>
                CurrentIndex.IsInRange(end: Count - 1) ?
                this[CurrentIndex] : default(Instruction);

            /// <summary>
            /// Индекс инструкции, выполняемой в данный момент
            /// </summary>
            public int CurrentIndex { get; protected set; }

            /// <summary>
            /// Набор определений инструкций, используемый для их выполнения
            /// </summary>
            public InstructionDefinitions Definitions { get; set; }

            /// <summary>
            /// Индекс инструкции, которая будет выполнена на следующем шаге,
            /// либо null, если задано поведение по умолчанию
            /// </summary>
            public int? NextIndex { get; set; }

            #endregion

            #region [Методы]

            /// <summary>
            /// Запускает действия, связанные с указанной инструкцией
            /// </summary>
            /// <param name="instruction">Инструкция для запуска</param>
            /// <returns></returns>
            public bool Execute(Instruction instruction)
            {
                if (Definitions.ByCode.TryGetValue(instruction?.Type ?? default(InstructionCode), out var definition) && definition.Handler != null)
                {
                    OnExecution(instruction);
                    definition.Handler.Invoke(instruction.Arguments);

                    return true;
                }
                return false;
            }

            /// <summary>
            /// Переход на инструкцию с указанным индексом
            /// </summary>
            /// <param name="operationIndex">Индекс инструкции для перехода</param>
            public void Goto(int operationIndex)
                => NextIndex = operationIndex != -1 ? operationIndex.Do(v => OnGoto?.Invoke(operationIndex)) : null as int?;

            /// <summary>
            /// Переходит ко следующей по порядку или заданию инструкции
            /// </summary>
            /// <returns></returns>
            public Instruction GotoNext()
            {
                // выбор следующего индекса исходя из заданного значения NextIndex
                CurrentIndex = NextIndex ?? CurrentIndex + 1;
                // сброс на поведение по умолчанию
                NextIndex = null;

                return Current;
            }

            /// <summary>
            /// Загружает набор инструкций в данную коллекцию
            /// </summary>
            /// <param name="source">Набор инструкций для загрузки</param>
            /// <returns></returns>
            public bool Load(IEnumerable<Instruction> source)
            {
                try
                {
                    if (Count > 0) Clear();

                    // добавление инструкций в коллекцию
                    foreach (var instruction in source)
                        Add(instruction);

                    Reset();
                    return true;
                }
                catch { return false; }
            }

            /// <summary>
            /// Сбрасывает настройки позиций на значения по умолчанию
            /// </summary>
            public void Reset()
            {
                CurrentIndex = -1;
                NextIndex = null;
            }

            #endregion

        }

        /// <summary>
        /// Класс, позволяющий записывать наборы определений инструкций
        /// </summary>
        public class InstructionDefinitions :
            Dictionary<string, (InstructionCode Code, Action<ArgumentType[]> Handler, IArgumentDefinition[] Arguments)>
        {
            #region [Свойства и Поля]

            public Dictionary<InstructionCode, (InstructionCode Code, Action<ArgumentType[]> Handler, IArgumentDefinition[] Arguments)> ByCode { get; } =
                new Dictionary<InstructionCode, (InstructionCode Code, Action<ArgumentType[]> Handler, IArgumentDefinition[] Arguments)>();

            #endregion

            #region [Методы]

            public new void Add(string key, (InstructionCode Code, Action<ArgumentType[]> Handler, IArgumentDefinition[] Arguments) value)
            {
                ByCode.Add(value.Code, value);
                base.Add(key, value);
            }

            public new void Clear()
            {
                ByCode.Clear();
                base.Clear();
            }

            public new void Remove(string key)
            {
                ByCode.Remove(this[key].Code);
                base.Remove(key);
            }

            #endregion

            public void Rebuild()
            {
                ByCode.Clear();
                foreach (var value in Values)
                    ByCode.Add(value.Code, value);
            }
        }

        public class InstructionBase :
            Collection<InstructionCode>
        {
            public InstructionDefinitions Base { get; set; }

            /// <summary>
            /// Проверяет предоставленный набор инструкций на соответствие данной базе
            /// </summary>
            /// <param name="source">Набор для проверки</param>
            /// <returns></returns>
            public bool Check(IEnumerable<Instruction> source)
                => source?.All(i => Contains(i.Type)) ?? false;

            public InstructionBase(InstructionDefinitions definitions, params string[] data)
            {
                Base = definitions;

                foreach (var instruction in data)
                    if (definitions.TryGetValue(instruction, out var def))
                        Add(def.Code);
            }

            public override string ToString()
                => $"[{string.Join(", ", this.Select(val => Base.First(pair => pair.Value.Code.Equals(val)).Key))}]";
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

        /// <summary>
        /// Предстваляет аргумент инструкции
        /// </summary>
        public class Argument :
            IArgumentDefinition
        {
            #region [Свойства и Поля]

            /// <summary>
            /// Значение по умолчанию
            /// </summary>
            public ArgumentType DefaultValue { get; set; }

            /// <summary>
            /// Являтся ли аргумент опциональным
            /// </summary>
            public bool IsOptional { get; set; }

            /// <summary>
            /// Функция преобразования из строкового представления данных в объектоное
            /// </summary>
            public Func<string, TranslationState, IArgumentDefinition, ArgumentType> Parser { get; set; }

            /// <summary>
            /// Функция проверки значения аргумента на допустимость
            /// </summary>
            public Func<ArgumentType, TranslationState, bool> Validator { get; set; }

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
        public virtual IInstructionCollection Instructions { get; } =
            new InstructionCollection();

        /// <summary>
        /// Функция чтения пользовательского ввода
        /// </summary>
        public virtual Func<ArgumentType> ReadInput { get; set; }

        /// <summary>
        /// Функция вывода значения в порт вывода
        /// </summary>
        public virtual Action<ArgumentType> WriteOutput { get; set; }

        #endregion

        #region [Методы]

        protected override void OnStarting(EventArgs args)
        {
            Instructions.Reset();
            base.OnStarting(args);
        }

        /// <summary>
        /// Выполняет шаг (единичную инструкцию) работы машины
        /// </summary>
        /// <returns></returns>
        public override bool Step()
        {
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

            return isSucceded;
        }

        protected override void Activate()
        {
            Instructions.Reset();
            base.Activate();
        }

        #endregion
    }
}