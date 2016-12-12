using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Machines.BreakPoints
{
    public class ActionBreakPoint :
        BreakPoint
    {


        protected ActionBreakPoint(IBreakPointCollection master) : base(master)
        {
        }

        public override bool IsReached
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
