using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AbstractDevelop.Machines;
using AbstractDevelop.Storage.Formats;
using AbstractDevelop.Projects;

using AbstractDevelop;
using AbstractDevelop.Translation;
using AbstractDevelop.Machines.Testing;

namespace AnotherTest
{
    class Program
    {
        static void Main(string[] args)
        {

            PlatformService.Initialize();
            PlatformService.Add(new RiscPlatform());

            var format = new JsonFormat();
            var project = AbstractProject.Load("Project.json", format);

            var machine = project.CreateMachine<RiscMachine>();
            var test = new RiscTestSystem(TestSource.Load("SampleTest.json", format), machine);

            var testString = string.Concat(Enumerable.Repeat("ror 1 [2] r5", 3));

            machine.Translator.Validate(testString, out var test1);

            machine.ReadInput = () => new ValueReference(machine.Do(v => Console.Write("Введите число: ")), 
                byte.Parse(Console.ReadLine()));
            machine.WriteOutput = (value) => Console.WriteLine($"Вывод: {value.Value}");

            if (machine.Translator.TranslateFile("risc.txt", out var instructions) &&
                instructions.Try(machine.Instructions.Load))
            {
                test.Run();
                machine.RunToEnd();
            }
                       
            Console.ReadKey();

        }
    }
}
