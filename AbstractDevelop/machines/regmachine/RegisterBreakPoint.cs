using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.machines.regmachine
{
    /// <summary>
    /// Представляет точку останова ПМБР.
    /// </summary>
    public struct RegisterBreakPoint
    {
        /// <summary>
        /// Полное название программы, в котором находится точка останова.
        /// </summary>
        public string Program;
        /// <summary>
        /// Номер команды, на которой установлена точка останова.
        /// </summary>
        public int Command;
    }
}
