using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop
{
    public static class LinkTools
    {
        public static bool IsValid(this object value, bool shouldThrowException = true)
        {
            if (value == default(object))
            {
                if (shouldThrowException) throw new NullReferenceException($"");
                else return false;
            }
            return false;
        }
    }
}
