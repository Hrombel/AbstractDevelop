using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Debug.BreakPoints
{
    public class ConditionBreakPoint :
         BreakPoint
    {
        public override bool IsReached
        {
            get
            {
                throw new NotImplementedException();
            }
        }
    }
}
