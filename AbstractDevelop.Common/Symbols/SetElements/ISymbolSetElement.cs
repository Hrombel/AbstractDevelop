using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Symbols
{
    /// <summary>
    /// Представляет определение элемента набора символов
    /// </summary>
    /// <remarks>Может являться как промежутком, так и единичным символом</remarks>
    public interface ISymbolSetElement :
        IEnumerable<char>
    {
        int Count { get; }

        bool Contains(char symbol);
    }
}
