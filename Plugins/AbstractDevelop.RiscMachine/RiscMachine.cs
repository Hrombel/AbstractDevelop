using System;
using System.Collections.Generic;
using System.Linq;

using DataType = System.Byte;
using MemoryStorage = AbstractDevelop.Machines.Tape<byte>;

namespace AbstractDevelop.Machines
{
    public enum RiscInstructionType
    {
        /// <summary>
        /// Неизвестная команда
        /// </summary>
        Unknown,

        /// <summary>
        /// Чтение ввода
        /// </summary>
        /// <remarks>Соотвествует команде 'in'</remarks>
        ReadInput,
        /// <summary>
        /// Вывод байта в порт вывода
        /// </summary>
        /// <remarks>Соотвествует команде 'out'</remarks>
        WriteOutput,
        ShiftRight, ShiftLeft,
        Not, Or, And,
        NotOr, NotAnd, Xor,
        Addition, Subtraction,
        JumpIfTrue, JumpIfFalse
    }

    public enum DataLinkType : byte { Value, Register, Memory }

    public class DataReference
    {
        internal static RiscMachine machine = default(RiscMachine);

        public DataLinkType Type { get; set; }

        public DataType Value
        {
            get
            {
                // TODO: реализовать подстчет времени доступа к ресурсам
                switch (Type)
                {
                    case DataLinkType.Memory: return machine.Memory[Reference];
                    case DataLinkType.Register: return machine.Registers[Reference];
                    default: return val;
                }
            }
            set
            {
                switch (Type)
                {
                    case DataLinkType.Memory: machine.Memory[Reference] = value; break;
                    case DataLinkType.Register: machine.SetRegisterValue(Reference, value); break;
                    default: val = value; break;
                }
            }
        }

        public DataType Reference { get; set; }

        DataType val;

        public override string ToString() => Value.ToString();

        public static implicit operator DataReference(DataType value)
            => new DataReference()
            {
                Type = DataLinkType.Value,
                Value = value
            };

        public static implicit operator DataType(DataReference link) => link.Value;
    }

    [Serializable]
    public partial class RiscMachine :
        InstructionMachine<RiscInstructionType, DataReference>
    {
        //public delegate void RegisterEventHandler(int register);
        //public event RegisterEventHandler RegisterUpdated;

        #region [Свойства]

        public MemoryStorage Memory { get; set; }

        public List<DataType> Registers { get; }

        public override ISourceTranslator Translator { get; }
          = new RiscTranslator();

        #endregion

        #region [Конструкторы]

        DataType Shift(DataType input, int count)
        {
            // (args[0] >> (args[1] % 8)) | (args[0] << (8 - args[1] % 8)) left
            // (args[0] << (args[1] % 8)) | (args[0] >> (8 - args[1] % 8)) right
            return input;
        }

        public RiscMachine(int memorySize, int registerCount) :
            base()
        {
            // выделение необходимого кол-ва памяти
            Memory = new MemoryStorage();
            Memory.IsInfinite = false;
            Memory.AddBlock(new MemoryStorage.TapeBlock(0, memorySize));
            // задание массива регистров
            Registers = new List<DataType>(new DataType[registerCount]);
            // список доступных операций (реализация по техническому документу)
            Instructions.Definitions.
                Add(RiscInstructionType.ReadInput, "in", 1, (args) => args[0].Value = ReadInput()).
                Add(RiscInstructionType.WriteOutput, "out", 1, (args) => WriteOutput(args[0])).
                Add(RiscInstructionType.Not, "not", 2, (args) => args[1].Value = (DataType)~args[0]).

                Add(RiscInstructionType.ShiftRight, "rol", 3, (args) => args[2].Value = Shift(args[0], +args[1])).
                Add(RiscInstructionType.ShiftLeft, "ror", 3, (args) => args[2].Value = Shift(args[0], -args[1])).

                Add(RiscInstructionType.Or, "or", 3, (args) => args[2].Value = (DataType)(args[0] | args[1])).
                Add(RiscInstructionType.And, "and", 3, (args) => args[2].Value = (DataType)(args[0] & args[1])).

                Add(RiscInstructionType.NotOr, "nor", 3, (args) => args[2].Value = (DataType)~(args[0] | args[1])).
                Add(RiscInstructionType.NotAnd, "nand", 3, (args) => args[2].Value = (DataType)~(args[0] & args[1])).

                Add(RiscInstructionType.Xor, "xor", 3, (args) => args[2].Value = (DataType)(args[0] ^ args[1])).
                Add(RiscInstructionType.Addition, "add", 3, (args) => args[2].Value = (DataType)(args[0] + args[1])).
                Add(RiscInstructionType.Subtraction, "sub", 3, (args) => args[2].Value = (DataType)(args[0] - args[1])).

                Add(RiscInstructionType.JumpIfTrue, "jo", 2, (args) => Operations.Goto(args[0] == 0xFF ? args[1] : -1)).
                Add(RiscInstructionType.JumpIfFalse, "jz", 2, (args) => Operations.Goto(args[0] == 0x00 ? args[1] : -1));

            /*/////////////////////////////////////////////////////////////////////////
            // задание необходимой машины на каждом этапе обработки ссылок
            OperationPreprocess     += (op) => DataReference.machine = this;
            OperationPostprocess    += (op) => DataReference.machine = null;

            Нет необходимости делать это на каждом шаге: информацию о запущенной машине
            можно получить через контекст (AbstractMachineContext)
            ////////////////////////////////////////////////////////////////////////*/

            // стандартный транслятор исхдодного текста
            //SourceTranslator = new RiscTranslator();
        }

        #endregion
    }
}
