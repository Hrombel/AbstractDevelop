using System;
using System.Collections.Generic;
using System.Linq;
using AbstractDevelop.Translation;

using DataType = System.Byte;
using MemoryStorage = System.Collections.ObjectModel.ObservableCollection<byte>;

namespace AbstractDevelop.Machines
{
    public partial class RiscMachine :
        InstructionMachine<RiscInstructionCode, DataReference>
    {
        #region [Шаблоны команд]

        static Argument
           // точка отправки данных
           source = new Argument(),
           // точка назначения данных (не может быть указателем на значение)
           destination = new Argument() { Validator = (arg, state) => !(arg is ValueReference || arg is LabelReference) },
           // метка (указатель должен ссылаться на существующую инструкцию)
           label = new Argument()
           {
               Validator = (arg, state) =>
                   arg?.Type == DataReferenceType.Label &&
                   (state as RiscTranslationState).Labels.ContainsValue(arg) 
           },
           // количество
           count = source;

        // наборы аргументов для операций определенного типа
        static Argument[]
            labelOperation = { source, label },
            shiftOperation = { source, count, destination },
            binaryOperation = { source, source, destination },
            unaryOperation = { source, destination },
            inputOperation = { destination },
            outputOperation = { source },
            subprocedureOperation = { label },
            noArgOperation = { };

        /// <summary>
        /// Определеня доступных для использования команд
        /// </summary>
        static InstructionDefinitions instructionsBase;

        /// <summary>
        /// Список префиксов форматов данных
        /// </summary>
        static Dictionary<char, NumberFormat> formatPrefixMapping = new Dictionary<char, NumberFormat>()
        {
            ['x'] = NumberFormat.Hex,
            ['o'] = NumberFormat.Octal,
            ['b'] = NumberFormat.Binary
        };

        public static DataType GetNumberValue(string token)
        {
            // если не удастся прочитать, то поиск по префиксам
            if (!int.TryParse(token, out var value))
            {
                value = -1;
                if (token[0] == '0' && token.Length > 1)
                {
                    // формат с префиксом
                    if (formatPrefixMapping.TryGetValue(token[1], out var format))
                        value = Convert.ToInt32(token.Remove(0, 2), (int)format);
                    // восьмеричный формат
                    else value = Convert.ToInt32(token, 8);
                }
            }

            if (!value.IsInRange(DataType.MinValue, DataType.MaxValue))
                throw new ArgumentException(nameof(token));

            return (DataType)value;
        }

        /// <summary>
        /// Функция для преобразования значений DataReference
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        DataReference ConvertToReference(string token, RiscTranslationState state, IArgumentDefinition argument)
        {
            if (string.IsNullOrEmpty(token))
                throw new ArgumentNullException(nameof(token));

            var firstChar = token.First();
            var lastChar = token.Last();

            // если первый символ - цифра, то преобразование в значение
            if (char.IsDigit(firstChar))
            {
                try { return new ValueReference(this, GetNumberValue(token)); }
                catch { throw new Exceptions.InvalidArgumentException(argument, token, state.LineNumber); }
            }
            // иначе ссылка
            else
            {
                // ссылка на регистр
                if (firstChar == 'r' && byte.TryParse(token.Substring(1, token.Length - 1), out var value))
                    return new DataReference(this, new ValueReference(this, value), DataReferenceType.Register);
                // ссылка на память
                else if (firstChar == '[' && lastChar == ']')
                    return new DataReference(this, ConvertToReference(token.Substring(1, token.Length - 2), state, argument), DataReferenceType.Memory);
                // метка
                else if (state.Labels.TryGetValue(token, out var reference))
                    return new LabelReference(this, (DataType)reference);
                // неизвестный формат
                else throw new Exceptions.InvalidArgumentException(argument, token, state.LineNumber);
            }
        }

        #endregion

        #region [Свойства и Поля]

        /// <summary>
        /// Внутренний стек вызовов для подпроцедур
        /// </summary>
        public Stack<int> ExecutionStack { get; }
            = new Stack<int>();

        /// <summary>
        /// Набор ячеек памяти <see cref="RiscMachine"/>
        /// </summary>
        public MemoryStorage Memory { get; }

        /// <summary>
        /// Набор регистров <see cref="RiscRegister"/>
        /// </summary>
        public List<IRegister> Registers { get; }

        /// <summary>
        /// Транслятор инструкций для <see cref="RiscRegister"/>
        /// </summary>
        public override ISourceTranslator Translator { get; }

        /// <summary>
        /// Таймер времени доступа в виртуальных тактах
        /// </summary>
        public int AccessTimer;

        #endregion

        #region [Методы]

        /// <summary>
        /// Производит сдвиг операнда на определенное количество позиций
        /// </summary>
        /// <param name="input">Операнд для сдвига</param>
        /// <param name="count">Количество позиций и направление сдвига</param>
        /// <returns></returns>
        DataType Shift(DataType input, int count)
        {
            const int dataTypeSize = 8;
            var countMod = Math.Abs(count % dataTypeSize);

            if (count > 0) // сдвиг вправо
                return (DataType)((input << countMod) | (input >> (dataTypeSize - countMod)));
            else if (count < 0) // сдвиг влево
                return (DataType)((input >> countMod) | (input << (dataTypeSize - countMod)));
            else // без сдвига
                return input;
        }

        /// <summary>
        /// Обрабатывает переход в подпроедуру с записью текщего места выполнения в стек вызовов
        /// </summary>
        /// <param name="instuctionIndex"></param>
        void Subprocedure(int instuctionIndex)
        {
            ExecutionStack.Push(Instructions.CurrentIndex);
            Instructions.Goto(instuctionIndex);
        }

        #endregion

        #region [Конструкторы и деструкторы]

        /// <summary>
        /// Конструктор по умолчанию для <see cref="RiscMachine"/>
        /// </summary>
        /// <param name="memorySize">Размер внешней памяти</param>
        /// <param name="registerCount">Количество регистров</param>
        public RiscMachine(int memorySize, int registerCount) :
            base()
        {
            // выделение необходимых ресурсов
            Memory = new MemoryStorage(new DataType[memorySize]);
            Registers = new List<IRegister>(Enumerable.Range(0, registerCount).Select(id => new RiscRegister(id)));
            // список доступных операций (реализация по техническому документу)
            Instructions.Definitions = instructionsBase = instructionsBase ?? new InstructionDefinitions()
            {
                ["in"] = (RiscInstructionCode.ReadInput, args => args[0].Value = ReadInput(), inputOperation),
                ["out"] = (RiscInstructionCode.WriteOutput, args => WriteOutput(args[0]), outputOperation),

                ["ror"] = (RiscInstructionCode.ShiftRight, args => args[2].Value = Shift(args[0], +args[1]), shiftOperation),
                ["rol"] = (RiscInstructionCode.ShiftLeft, args => args[2].Value = Shift(args[0], -args[1]), shiftOperation),

                ["nor"] = (RiscInstructionCode.NotOr, args => args[2].Value = (DataType)~(args[0] | args[1]), binaryOperation),
                ["xor"] = (RiscInstructionCode.Xor, args => args[2].Value = (DataType)(args[0] ^ args[1]), binaryOperation),
                ["or"] = (RiscInstructionCode.Or, args => args[2].Value = (DataType)(args[0] | args[1]), binaryOperation),
                ["and"] = (RiscInstructionCode.And, args => args[2].Value = (DataType)(args[0] & args[1]), binaryOperation),
                ["nand"] = (RiscInstructionCode.NotAnd, args => args[2].Value = (DataType)~(args[0] & args[1]), binaryOperation),
                ["add"] = (RiscInstructionCode.Addition, args => args[2].Value = (DataType)(args[0] + args[1]), binaryOperation),
                ["sub"] = (RiscInstructionCode.Subtraction, args => args[2].Value = (DataType)(args[0] - args[1]), binaryOperation),

                ["not"] = (RiscInstructionCode.Not, args => args[1].Value = (DataType)~args[0], unaryOperation),

                ["jo"] = (RiscInstructionCode.JumpIfTrue, args => Instructions.Goto(args[0] == 0xFF ? args[1] : -1), labelOperation),
                ["jz"] = (RiscInstructionCode.JumpIfFalse, args => Instructions.Goto(args[0] == 0x00 ? args[1] : -1), labelOperation),

                ["call"] = (RiscInstructionCode.Call, args => Subprocedure(args[0]), subprocedureOperation),
                ["ret"] = (RiscInstructionCode.Return, args => Instructions.Goto(ExecutionStack.Pop()), noArgOperation)
            }; Instructions.Definitions.Rebuild();

            // увеличение счетчика времени доступа\работы
            Instructions.OnExecution += (instruction) => AccessTimer++;
            Instructions.OnGoto += (index) => AccessTimer += 8;

            // стандартный транслятор инструкций 
            Translator = new RiscTranslator() { Convert = ConvertToReference };
        }

        #endregion
    }
}
