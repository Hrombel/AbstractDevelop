using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.tools
{
    /// <summary>
    /// Предоставляет набор методов для работы с иходными текстами программ.
    /// </summary>
    public sealed class SourceTools
    {
        /// <summary>
        /// Удаляет все комментарии из исходного кода.
        /// </summary>
        /// <param name="src">Исходный текст программы, комментарии из которого удаляются.</param>
        /// <returns>Строка с исходным текстом без комментариев.</returns>
        public static string RemoveComments(string src)
        {
            if (src == null)
                throw new ArgumentNullException("Исходный текст программы не может быть неопределен");

            while (RemoveFirst(ref src)) ;

            return src;
        }

        /// <summary>
        /// Удаляет первое вхождение комментариев.
        /// </summary>
        /// <param name="src">Строка, содержащая комментарии.</param>
        /// <returns>Истина, если комментарий найден, иначе - не найден.</returns>
        private static bool RemoveFirst(ref string src)
        {
            bool line = true;

            int s = src.IndexOf("//");
            if (s == -1)
            {
                line = false;
                s = src.IndexOf("/*");
                if (s == -1) return false;
            }
            int e;
            
            if(line)
            {
                e = src.IndexOf('\n', s);
                if (e == -1) e = src.Length;
                e--;
            }
            else
            {
                e = src.IndexOf("*/", s);
                if (e == -1)
                    throw new Exception("Незакрытый блочный комментарий");
                e++;
            }

            src = src.Remove(s, e - s + 1);
            return true;
        }
    }
}
