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
        public int ActionIndex { get; }

        public override bool IsReached =>
              MasterCollection.Owner.Context.CurrentIndex == ActionIndex;

        protected ActionBreakPoint(int actionIndex, IBreakPointCollection master) : 
            base(master)
        {
            ActionIndex = actionIndex;
        }
    }
}
