﻿using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using AbstractDevelop.Machines;
using AbstractDevelop.Storage.Formats;
using AbstractDevelop.Projects;
using AbstractDevelop;
using System.IO;

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

            var machine = project.Platform.CreateMachine(project) as RiscMachine;

            machine.ReadInput = () => new ValueReference(machine.Do(v => Console.Write("Введите число: ")), byte.Parse(Console.ReadLine()));
            machine.WriteOutput = (value) => Console.WriteLine($"Вывод: {value.Value}");

            // var result = (machine.Translator as RiscMachine.RiscTranslator).Translate(File.ReadAllLines("risc.txt"), null);

            foreach (var instr in (machine.Translator as RiscMachine.RiscTranslator).Translate(File.ReadAllLines("risc.txt"), null))
                machine.Instructions.Add(instr);

            machine.Instructions.Reset();
            machine.RunToEnd();

            Console.ReadKey();

        }
    }
}
