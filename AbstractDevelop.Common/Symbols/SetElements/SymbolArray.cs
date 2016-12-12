using System;
using System.Collections;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop.Symbols
{
    public struct SymbolArray :
        ISymbolSetElement
    {
        public char[] Array;

        public int Count { get { return Array.Length; } }

        public bool Contains(char symbol)
            => Array.Contains(symbol);

        public IEnumerator<char> GetEnumerator()
            => GetEnumerator() as IEnumerator<char>;

        IEnumerator IEnumerable.GetEnumerator()
            => Array.GetEnumerator();

        public SymbolArray(char[] symbols)
        {
            Array = symbols ?? new char[0];
        }

        public SymbolArray(string source) :
            this(source.ToCharArray()) { }

        public static implicit operator SymbolArray(char[] value)
            => new SymbolArray(value);

        public static implicit operator SymbolArray(string value)
           => new SymbolArray(value);
    }
}
