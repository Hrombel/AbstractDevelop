using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Debug.BreakPoints
{
    public class ActionBreakPoint :
        BreakPoint
    {
        private int index;
        private Func<int> p;

        public int ActionIndex { get; }

        public Func<int> Source { get; }

        public override bool IsReached =>
              Source() == ActionIndex;

        public ActionBreakPoint(int index, Func<int> source)
        {
            ActionIndex = index;
            Source = source;
        }
    }
}
