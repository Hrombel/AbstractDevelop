using System;
using System.Collections;
using System.Collections.Generic;
namespace AbstractDevelop.Symbols
{
    public struct SymbolRange :
        ISymbolSetElement
    {
        /// <summary>
        /// Стандартный перечислитель промежутка символов
        /// </summary>
        class SymbolRangeEnumerator :
            IEnumerator<char>
        {
            public SymbolRange Range { get; private set; }

            public char Current { get; set; }
            object IEnumerator.Current { get { return Current; } }

            public void Dispose() { }
          
            public bool MoveNext()
                => ++Current <= Range.EndChar;

            public void Reset()
              => Current = Range.StartChar;

            public SymbolRangeEnumerator(SymbolRange range, char startChar = default(char))
            {
                if (startChar == default(char))
                    startChar = range.StartChar;

                Range = range;
                Current = startChar;
            }
        }

        public char StartChar, EndChar;

        public int Count { get { return (EndChar - StartChar); } }
       
        public IEnumerator<char> GetEnumerator()
            => new SymbolRangeEnumerator(this);

        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator() as IEnumerator;

        public bool Contains(char symbol)
            => symbol.IsInRange(StartChar, EndChar, false);

        // TODO: реализовать интерпретацию проверки в IL коде
    }
}