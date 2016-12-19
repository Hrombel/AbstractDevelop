using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AbstractDevelop.Machines.Properties;

namespace AbstractDevelop.Machines.Testing
{
    public class RiscTestSystem
    {
        public List<RiscTest> Tests { get; set; }

        public RiscMachine Machine { get; set; }

        public RiscMachine.InstructionBase InstructionSet { get; }

        public RiscTestSystem(TestSource source, RiscMachine machine)
        {
            Machine = machine;
            Tests = new List<RiscTest>(source.Tests.Select(test => new RiscTest(test, this)));

            InstructionSet = source.InstructionSet != null ? new RiscMachine.InstructionBase(machine.Instructions.Definitions, source.InstructionSet) : null ;
        }

        public Dictionary<RiscTest, Exception> Run()
        {
            var preservedInput = Machine.ReadInput;
            var preservedOutput = Machine.WriteOutput;

            var result = new Dictionary<RiscTest, Exception>(Tests.Count);

            foreach (var test in Tests)
            {
                var instructionBase = (test.InstructionSet ?? InstructionSet);

                if (!(instructionBase?.Check(Machine.Instructions) ?? true))
                {
                    result.Add(test, new Exception(Translate.Key("TestInvalidInstructionBase", Resources.ResourceManager, format: instructionBase)));
                    continue;
                }

                // TODO: проверка набора инструкций

                int inputIndex = 0, outputIndex = 0;
                try
                {
                    Machine.ReadInput = () => new ValueReference(Machine, test.Input[inputIndex++]);
                    Machine.WriteOutput = (value) =>
                    {
                        if (value.Value != test.Output[outputIndex++])
                            throw new ArgumentException(Translate.Key("TestInvalidOutputValue", Resources.ResourceManager, outputIndex, test.Output[outputIndex - 1]));
                    };

                    Machine.RunToEnd();
                    result.Add(test, CheckIndexes(false));
                }
                catch (IndexOutOfRangeException) { result.Add(test, CheckIndexes(true)); }
                catch (Exception ex) { result.Add(test, ex); }

                Exception CheckIndexes(bool shouldBeEqual)
                {
                    if ((inputIndex == test.Input.Length) == shouldBeEqual)
                        return new Exception(Translate.Key("TestInvalidInput", Resources.ResourceManager, format: test.Input.Length));
                    if ((outputIndex == test.Output.Length) == shouldBeEqual)
                        return new Exception(Translate.Key("TestInvalidOutput", Resources.ResourceManager, format: test.Output.Length));

                    // индексы прошли проверку
                    return null;
                }
            }

            Machine.ReadInput = preservedInput;
            Machine.WriteOutput = preservedOutput;

            return result;
        }
    }
}
