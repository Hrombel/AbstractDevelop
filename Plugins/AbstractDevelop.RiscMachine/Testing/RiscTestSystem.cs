using System;
using System.Collections.Generic;
using System.Linq;

using AbstractDevelop.Machines.Properties;
using System.Collections.ObjectModel;

namespace AbstractDevelop.Machines.Testing
{
    public class RiscTestSystem
    {
        public ObservableCollection<RiscTest> Tests { get; set; }

        public RiscMachine Machine { get; set; }

        public RiscMachine.InstructionBase InstructionSet { get; }

        public RiscTestSystem(TestSource source, RiscMachine machine)
        {
            Machine = machine;
            Tests = new ObservableCollection<RiscTest>(source.Tests.Select(test => new RiscTest(test, this)));

            InstructionSet = source.InstructionSet != null ? new RiscMachine.InstructionBase(machine.Instructions.Definitions, source.InstructionSet) : null ;
        }

        public void Run()
        {
            var preservedInput = Machine.ReadInput;
            var preservedOutput = Machine.WriteOutput;

            var memorySnapshot = Machine.Memory.GetSnapshot();
            var registerSnapshot = Machine.Registers.GetSnapshot();

            foreach (var test in Tests)
            {
                var instructionBase = (test.InstructionSet ?? InstructionSet);
                test.LastError = null;

                int inputIndex = 0, outputIndex = 0;
                try
                {
                    if (!(instructionBase?.Check(Machine.Instructions) ?? true))
                        throw new Exception(Translate.Key("TestInvalidInstructionBase", Resources.ResourceManager, format: instructionBase));
          
                    Machine.ReadInput = () => new ValueReference(Machine, test.Input[inputIndex++]);
                    Machine.WriteOutput = (value) =>
                    {
                        if (value.Value != test.Output[outputIndex++])
                            throw new ArgumentException(Translate.Key("TestInvalidOutputValue", Resources.ResourceManager, outputIndex, test.Output[outputIndex - 1]));
                    };

                    Machine.RunToEnd();
                    test.LastError = CheckIndexes(false);
                }
                catch (IndexOutOfRangeException) { test.LastError = CheckIndexes(true); }
                catch (Exception ex) { test.LastError =  ex; }

                test.State = test.LastError == null? TestState.Passed : TestState.Failed;

                Machine.Memory.RestoreValues(memorySnapshot);
                Machine.Registers.RestoreValues(registerSnapshot);

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
        }
    }
}
