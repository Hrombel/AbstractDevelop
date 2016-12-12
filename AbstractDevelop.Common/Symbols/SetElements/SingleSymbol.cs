using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Symbols
{
    public struct SingleSymbol :
        ISymbolSetElement
    {
        public char Symbol;

        public int Count { get { return 1; } }

        public bool Contains(char symbol)
            => symbol == Symbol;
    
        public IEnumerator<char> GetEnumerator() { yield return Symbol; }

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator() as IEnumerator;

        public SingleSymbol(char value)
        {
            Symbol = value;
        }

        public static implicit operator SingleSymbol(char value)
            => new SingleSymbol(value);
    }
}
