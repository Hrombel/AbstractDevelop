using System.Collections;
using System.Collections.Generic;
using System.Linq;

namespace AbstractDevelop.Symbols
{
    public struct SymbolArray :
        ISymbolSetElement
    {
        #region [Свойства и Поля]

        public int Count { get { return Array.Length; } }
        public char[] Array;

        #endregion

        #region [Методы]

        public static implicit operator SymbolArray(char[] value)
            => new SymbolArray(value);

        public static implicit operator SymbolArray(string value)
           => new SymbolArray(value);

        public bool Contains(char symbol)
                            => Array.Contains(symbol);

        public IEnumerator<char> GetEnumerator()
            => GetEnumerator() as IEnumerator<char>;

        IEnumerator IEnumerable.GetEnumerator()
            => Array.GetEnumerator();

        #endregion

        #region [Конструкторы и деструкторы]

        public SymbolArray(char[] symbols)
        {
            Array = symbols ?? new char[0];
        }

        public SymbolArray(string source) :
            this(source.ToCharArray())
        { }

        #endregion
    }
}