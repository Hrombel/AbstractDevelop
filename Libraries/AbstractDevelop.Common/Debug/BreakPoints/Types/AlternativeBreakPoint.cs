using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Debug.BreakPoints
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
    }
}
