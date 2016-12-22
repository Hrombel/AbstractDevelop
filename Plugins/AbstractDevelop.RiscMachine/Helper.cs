using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Machines
{
    public static class Helper
    {

        public static void RestoreValues(this IEnumerable<IDataCell> target, byte[] values)
        {
            int index = 0;
            foreach (var cell in target)
            {
                if (index == values.Length)
                    break;
                else
                    cell.Value = values[index++];
            }
        }


        public static byte[] GetSnapshot(this IEnumerable<IDataCell> source)
             => source.Select(cell => cell.Value).ToArray();
    }
}
