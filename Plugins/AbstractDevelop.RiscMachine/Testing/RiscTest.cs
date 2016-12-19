using System.Diagnostics;
using System.Linq;

using static AbstractDevelop.Machines.Testing.TestSource;

namespace AbstractDevelop.Machines.Testing
{
    [DebuggerDisplay("{Title}")]
    public  class RiscTest
    {
        #region [Свойства и Поля]

        public byte[] Input { get; set; }
        public RiscMachine.InstructionBase InstructionSet { get; set; }
        public byte[] Output { get; set; }
        public string Title { get; set; }

        #endregion

        #region [Конструкторы и деструкторы]

        public RiscTest(TestEntry test, RiscTestSystem system)
        {
            Title = test.Title;

            Input = test.Input.Select(RiscMachine.GetNumberValue).ToArray();
            Output = test.Output.Select(RiscMachine.GetNumberValue).ToArray();

            InstructionSet = test.InstructionSet != null ? new RiscMachine.InstructionBase(system.Machine.Instructions.Definitions, test.InstructionSet) : null;
        }

        public override string ToString()
            => Title;

        #endregion
    }
}
