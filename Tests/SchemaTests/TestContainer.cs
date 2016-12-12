using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace SchemaTests
{
    public class Test
    {
        public string Title { get; set; }

        public object[] Input { get; set; }

        public object[] Output { get; set; }

        public string[] InstructionSet { get; set; }
    }

    public class TestContainer
    {
        public string[] InstructionSet { get; set; }

        public Test[] Tests { get; set; }
    }
}
