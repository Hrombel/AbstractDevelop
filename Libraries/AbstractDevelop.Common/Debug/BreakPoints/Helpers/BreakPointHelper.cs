using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Debug.BreakPoints
{
    public static class BreakPointHelper
    {
        /// <summary>
        /// Проверяет тип точки останова
        /// </summary>
        /// <param name="target">Точка останова для проверки</param>
        /// <param name="type">Целевой тип</param>
        /// <returns></returns>
        public static bool OfType(this IBreakPoint target, BreakPointType type)
            => (target.Type & type) == type;

    }
}
