using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop
{
    public enum DebugEntryType
    {
        Message, Warning, Error, Output
    }

    public class DebugEntry
    {
        public DebugEntryType Type { get; set; }

        public int? Line { get; set; }

        public string Content { get; set; }


        public DebugEntry(string content, int? line = null, DebugEntryType type = DebugEntryType.Output)
        {
            Type = type;
            Content = content;
            Line = line;
        }
    }
}
