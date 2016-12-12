using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Machines.BreakPoints
{
    class AlternativeBreakPoint :
        BreakPoint
    {
        public override BreakPointType Type
            => BreakPointType.PostAction;
               
        public override bool IsReached
        {
            get
            {
                throw new NotImplementedException();
            }
        }

        public AlternativeBreakPoint(IBreakPointCollection master) : base(master)
        {
        }
    }
}
