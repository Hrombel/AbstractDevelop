using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Debug.BreakPoints
{
    public class BreakPointEventArgs :
        EventArgs
    {
        public IBreakPoint BreakPoint { get; set; }

        public BreakPointEventArgs(IBreakPoint breakPoint)
        {
            BreakPoint = breakPoint;
        }
    }
}
