using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

using Newtonsoft.Json.Schema;
using Newtonsoft.Json;
using System.IO;
using Newtonsoft.Json.Linq;


using System.ComponentModel.Composition.Hosting;
using System.Reflection;

using AbstractDevelop.Storage.Formats;
using AbstractDevelop.Projects;
using AbstractDevelop;
using AbstractDevelop.Machines;
using System.ComponentModel.Composition.Primitives;

namespace SchemaTests
{
    class Program
    {
        static void Main(string[] args)
        {
            var catalogList = new List<ComposablePartCatalog>(Assembly.GetExecutingAssembly().GetReferencedAssemblies().
                    Select(assemblyName => Assembly.Load(assemblyName)).
                    Select(assembly => new AssemblyCatalog(assembly)));

            catalogList.Add(new DirectoryCatalog(Environment.CurrentDirectory, "*.plg"));


            var priorityCatalog = new AggregateCatalog(catalogList);
            var priorityProvider = new CatalogExportProvider(priorityCatalog);
            var container = new CompositionContainer(priorityProvider);
            var batch = new CompositionBatch();

            priorityProvider.SourceProvider = container;
            container.Compose(batch);

            // инициализация списка доступных платформ
            PlatformService.Initialize(container);

            // получение вспомогательных объектов
            var format = container.GetExportedValue<IBinaryDataFormatProvider>();
            var stopwatch = System.Diagnostics.Stopwatch.StartNew();

            var project = AbstractProject.Load(File.OpenRead("Project.json"), format);

            var machine = project.Platform.CreateMachine(project);
            var result = machine.Translator.Translate(File.ReadAllLines("risc.txt"));

            Console.WriteLine($"Parsing: {stopwatch.ElapsedMilliseconds}"); stopwatch.Restart();
            Console.ReadKey();
 
            // открытие из "чистого" формата
            Console.WriteLine($"Load: {stopwatch.ElapsedMilliseconds}"); stopwatch.Restart();

            //project.Settings.
            //    Set("memorySize", risc.Memory.Count).
            //    Set("registerCount", risc.Registers.Count);

            var file = (project[@"Resources\resource1.res"] as ProjectFile);

            //using (var rdr = file.CreateStream().CreateReader())
            //    Console.WriteLine(rdr.ReadToEnd());

            using (var fileStream = file.CreateStream())
            {
                using (var writer = fileStream.CreateWriter())
                {
                    writer.Write("Just a test string here");
                    file.CopyFrom(fileStream);
                }
            }

            //file = (project[$@"Resources\images\image1.png"]) as ProjectFile;

            //file = (project[$@"Resources\images\image1.png"] = new ProjectFile()) as ProjectFile;
            //file.Serialize(project, format);

            //using (var reader = file.CreateStream().CreateReader())

            //    Console.WriteLine(reader.ReadToEnd());

            // добавлние нового файла в проект
            // project[$@"Resources\images\image1.png"] = new ProjectFile();
            // либо
            // project.Root.GetDirectory(@"Resources\images").Add(new ProjectFile("image1.png"));

            //dynamic resfile = project[@"Resources\resource1.res", generateExceptions: true];
            //resfile.CopyFrom(File.OpenRead("SchemaTests.pdb"));

            //Console.WriteLine(resfile.Type.ToString());
            //Console.WriteLine($"Access: {stopwatch.ElapsedMilliseconds}"); stopwatch.Restart();
            
            // запись в "чистом" формате
            project.Save(File.OpenWrite("Project_plain.json"));
            Console.WriteLine($"Write plain: {stopwatch.ElapsedMilliseconds}"); stopwatch.Restart();

            // запись в сжатом формате
            format.PreferBinary = true;
            project.Save(File.OpenWrite("Project.adp"));
            Console.WriteLine($"Write compressed: {stopwatch.ElapsedMilliseconds}");

            Console.ReadKey();
        }
    }
}
