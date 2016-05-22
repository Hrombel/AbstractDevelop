using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using DataType = System.Byte;
using RiscDataCell = AbstractDevelop.Machines.Risc.DataCell<byte>;
using MemoryStorage = AbstractDevelop.machines.tape.Tape<byte>;

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

    public class RiscMachine :
        AbstractMachine<RiscOperation, DataLink<DataType>, MemoryStorage>
    {
        public override string Name => nameof(RiscMachine);

        public MemoryStorage Memory { get { return Storages[0]; } }

        public DataType[] Registers { get; }

        public RiscMachine(int memorySize = 256, int registerCount = 8) :
            base()
        {
            // выделение необходимого кол-ва памяти
            Memory.IsInfinite = false;
            Memory.Clear(memorySize);
            // задание массива регистров
            Registers = new DataType[registerCount];
            // список доступных операций (реализация по техническому документу)
            Operations.Definitions.
                AddChain(RiscOperation.ReadInput, (args) => { (args[0] as RiscDataCell).Value = ReadInput(); }).
                AddChain(RiscOperation.WriteOutput, (args) => { Write(args[0].Value); }).
                AddChain(RiscOperation.ShiftRight, (args) => { }).
                AddChain(RiscOperation.ShiftLeft, (args) => { }).
                AddChain(RiscOperation.Not, (args) => { }).
                AddChain(RiscOperation.Or, (args) => { }).
                AddChain(RiscOperation.And, (args) => { }).
                AddChain(RiscOperation.NotOr, (args) => { }).
                AddChain(RiscOperation.NotAnd, (args) => { }).
                AddChain(RiscOperation.Xor, (args) => { }).
                AddChain(RiscOperation.Addition, (args) => { }).
                AddChain(RiscOperation.Subtraction, (args) => { }).
                AddChain(RiscOperation.JumpIfTrue, (args) => { }).
                AddChain(RiscOperation.JumpIfFalse, (args) => { });
        }
    }
}
