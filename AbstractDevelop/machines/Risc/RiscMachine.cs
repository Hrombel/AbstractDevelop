using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

using DataType = System.Int32;
using MemoryStorage = AbstractDevelop.Machines.Tape<int>;
using RiscOp = AbstractDevelop.Machines.Operation<AbstractDevelop.Machines.Risc.RiscOperation, AbstractDevelop.Machines.Risc.DataReference>;

namespace AbstractDevelop.Machines.Risc
{
    public enum RiscOperation
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

    public class RiscTranslator :
        ISourceTranslator<RiscOp, RiscOperation, DataReference>
    {
        static readonly string
            reg3 = $@"r\d+|\[\d+\]",
            reg1 = $@"{reg3}|[\d-]+",
            reg2 = $@"{reg1}|[A-Za-z_\d]+",
            list = "in|out|ror|rol|not|or|and|nor|nand|xor|add|sub|jz|jo",
            reg = $@"\s*({list})\s+({reg1})+\s*({reg2})*\s*({reg3})*";
            

        RiscOperation GetOperation(string op)
        {
            switch (op)
            {
                case "in": return RiscOperation.ReadInput;
                case "out": return RiscOperation.WriteOutput;
                case "ror": return RiscOperation.ShiftRight;
                case "rol": return RiscOperation.ShiftLeft;
                case "not": return RiscOperation.Not;
                case "or": return RiscOperation.Or;
                case "and": return RiscOperation.And;
                case "nor": return RiscOperation.NotOr;
                case "nand": return RiscOperation.NotAnd;
                case "xor": return RiscOperation.Xor;
                case "add": return RiscOperation.Addition;
                case "sub": return RiscOperation.Subtraction;
                case "jz": return RiscOperation.JumpIfFalse;
                case "jo": return RiscOperation.JumpIfTrue;
                default: return RiscOperation.Unknown;
            }
        }

        public IEnumerable<RiscOp> Translate(IEnumerable<string> input)
        {
            var commands = new List<RiscOp>();
            var regex = new Regex(reg, RegexOptions.Singleline);

            foreach (var line in input)
            {
                if (string.IsNullOrEmpty(line)) continue;

                var match = regex.Match(line);
                var count = match.Groups.Count;
                // если есть достаточное кол-во совпадений
                if (match.Success && count > 2)
                {
                    var list = new List<DataReference>(match.Captures.Count - 1);
                    DataType value = 0;
                    for (int i = 2; i < count; i++)
                    {
                        var str = match.Groups[i].Value.ToLower();
                        if (string.IsNullOrEmpty(str)) continue;

                        if (int.TryParse(str, out value))
                            list.Add(value);
                        else if (str.StartsWith("r"))
                            list.Add(new DataReference()
                            {
                                // сначала указывается тип, потому что он учитывается при задании значения свойства Value
                                Type = DataLinkType.Register,
                                Reference = DataType.Parse(str.Substring(1, str.Length - 1))
                            });
                        else if (str.StartsWith("[") && str.EndsWith("]") && str.Length > 2)
                            list.Add(new DataReference()
                            {
                                Type = DataLinkType.Memory,
                                Reference = DataType.Parse(str.Substring(1, str.Length - 2))
                            });
                        else
                            throw new NotSupportedException($"Неверный параметр в команде {match.Groups[0].Value}: {str}");
                    }
     
                    commands.Add(RiscOp.Create(GetOperation(match.Groups[1].Value), list.ToArray()));
                }
                else
                    throw new ArgumentException($"Неизвестный шаблон команды в строке: {line}");
            }
            return commands;
        }
    }

    [Serializable]
    public class RiscMachine :
        AbstractMachine<RiscOperation, DataReference, MemoryStorage>
    {
        public delegate void RegisterEventHandler(int register);
        public event RegisterEventHandler RegisterUpdated;

        #region [Свойства]

        public MemoryStorage Memory { get; set; }
        public override string Name => nameof(RiscMachine);
        public List<DataType> Registers { get; }

        #endregion

        internal void SetRegisterValue(int index, int value)
        {
            Registers[index] = value;
            RegisterUpdated?.Invoke(index);
        }

        #region [Конструкторы]
        const int ByteMask = 0x000000FF;

        public RiscMachine(int memorySize = 256, int registerCount = 8) :
            base()
        {
            // выделение необходимого кол-ва памяти
            Memory = new MemoryStorage();
            Memory.IsInfinite = false;
            Memory.AddBlock(new MemoryStorage.TapeBlock(0, memorySize));
            // задание массива регистров
            Registers = new List<DataType>(Enumerable.Repeat(0, registerCount));
            // список доступных операций (реализация по техническому документу)
            Operations.Definitions.
                AddChain(RiscOperation.ReadInput,   (args) => args[0].Value = ReadInput()).
                AddChain(RiscOperation.WriteOutput, (args) => WriteOutput(args[0] & ByteMask)).
                AddChain(RiscOperation.ShiftRight,  (args) => args[2].Value = ((byte)args[0] >> (args[1] % 8)) | ((byte)args[0] << (8 - args[1] % 8)) & ByteMask).
                AddChain(RiscOperation.ShiftLeft,   (args) => args[2].Value = ((byte)args[0] << (args[1] % 8)) | ((byte)args[0] >> (8 - args[1] % 8)) & ByteMask).
                AddChain(RiscOperation.Not,         (args) => args[1].Value = ~args[0] & ByteMask).
                AddChain(RiscOperation.Or,          (args) => args[2].Value = (args[0] | args[1]) & ByteMask).
                AddChain(RiscOperation.And,         (args) => args[2].Value = (args[0] & args[1]) & ByteMask).
                AddChain(RiscOperation.NotOr,       (args) => args[2].Value = ~(args[0] | args[1]) & ByteMask).
                AddChain(RiscOperation.NotAnd,      (args) => args[2].Value = ~(args[0] & args[1]) & ByteMask).
                AddChain(RiscOperation.Xor,         (args) => args[2].Value = (args[0] ^ args[1]) & ByteMask).
                AddChain(RiscOperation.Addition,    (args) => args[2].Value = (args[0] + args[1]) & ByteMask).
                AddChain(RiscOperation.Subtraction, (args) => args[2].Value = (args[0] - args[1]) & ByteMask).
                AddChain(RiscOperation.JumpIfTrue,  (args) => Operations.Goto((args[0] & ByteMask) == 0xFF? (int)args[1] : -1)).
                AddChain(RiscOperation.JumpIfFalse, (args) => Operations.Goto((args[0] & ByteMask) == 0x00? (int)args[1] : -1));

            // задание необходимой машины на каждом этапе обработки ссылок
            OperationPreprocess     += (op) => DataReference.machine = this;
            OperationPostprocess    += (op) => DataReference.machine = null;

            SourceTranslator = new RiscTranslator();
        }

        #endregion
    }
}
