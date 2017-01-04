using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Symbols
{
    public static class SymbolHelper
    {
        public static ISymbolSet ToSymbolSet(this IEnumerable<ISymbolSetElement> source)
            => new SymbolSet(source.ToList());

    }
}
