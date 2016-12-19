using AbstractDevelop.Storage.Formats;
using System;
using System.Collections.Generic;
using System.IO;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Machines.Testing
{
    public class TestSource
    {
        public string[] InstructionSet { get; set; }

        public TestEntry[] Tests { get; set; }

        public class TestEntry
        {
            public string Title { get; set; }

            public string[] Input { get; set; }

            public string[] Output { get; set; }

            public string[] InstructionSet { get; set; }
        }

        public static TestSource Load(Stream source, IDataFormatProvider format)
            => format.Deserialize<TestSource>(source);

        public static TestSource Load(string path, IDataFormatProvider format)
           => File.OpenRead(path).Using(format.Deserialize<TestSource>);
    }
}
