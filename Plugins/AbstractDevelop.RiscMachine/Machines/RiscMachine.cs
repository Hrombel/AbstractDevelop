using System;
using System.Collections.Generic;
using System.Text.RegularExpressions;

using AbstractDevelop.Translation;

using DataType = System.Byte;
using MemoryStorage = AbstractDevelop.Machines.Tape<byte>;
using System.Linq;

namespace AbstractDevelop.Machines
{
    public partial class RiscMachine :
        InstructionMachine<RiscInstructionCode, DataReference>
    {
        #region [Шаблоны команд]

        static readonly Regex argumentExpression = new Regex(@"^((\d+)|r(\d)|\[(\d+)\])$", RegexOptions.Singleline);

        static Argument
           // точка отправки данных
           source = new Argument(),
           // точка назначения данных (не может быть указателем на значение)
           destination = new Argument() { Validator = (arg, state) => arg.Type != DataReferenceType.Value },
           // метка (указатель должен ссылаться на существующую инструкцию)
           label = new Argument()
           {
               Validator = (arg, state) =>
                   (arg.Type == DataReferenceType.Label &&
                   (state as RiscTranslationState).Labels.ContainsValue(arg))
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
        /// Функция для преобразования значений DataReference
        /// </summary>
        /// <param name="token"></param>
        /// <returns></returns>
        DataReference ConvertToReference(string token, RiscTranslationState state)
        {
            var match = default(Match);
            if ((match = argumentExpression.Match(token)).Success)
            {
                DataReferenceType type = default(DataReferenceType);

                if (!string.IsNullOrEmpty(match.Groups[2].Value))
                    type = DataReferenceType.Value;
                else if (!string.IsNullOrEmpty(match.Groups[3].Value))
                    type = DataReferenceType.Register;
                else if (!string.IsNullOrEmpty(match.Groups[4].Value))
                    type = DataReferenceType.Memory;

                return new DataReference(this, int.Parse(match.Groups[(int)type + 1].Value), type);
            }
            // поиск ссылки
            else if (state.Labels.TryGetValue(token, out var reference))
                return new DataReference(this, reference, DataReferenceType.Label);
            else
                return DataReference.Empty(this);
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
            Memory = new MemoryStorage(false, memorySize);
            Registers = new List<IRegister>(Enumerable.Range(0, registerCount).Select(id => new RiscRegister(id)));
            // список доступных операций (реализация по техническому документу)
            instructionsBase = new InstructionDefinitions()
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
            };

            // стандартный транслятор инструкций 
            Translator = new RiscTranslator() { Convert = ConvertToReference };
        }

        #endregion
    }
}
