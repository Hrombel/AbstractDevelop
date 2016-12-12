using System.Collections;
using System.Collections.Generic;

namespace AbstractDevelop.Symbols
{
    public struct SymbolRange :
        ISymbolSetElement
    {

        #region [Классы и структуры]

        /// <summary>
        /// Стандартный перечислитель промежутка символов
        /// </summary>
        class SymbolRangeEnumerator :
            IEnumerator<char>
        {

            #region [Свойства и Поля]

            public char Current { get; set; }

            object IEnumerator.Current { get { return Current; } }

            public SymbolRange Range { get; private set; }

            #endregion

            #region [Методы]

            public void Dispose()
            {
            }

            public bool MoveNext()
                => ++Current <= Range.EndChar;

            public void Reset()
              => Current = Range.StartChar;

            #endregion

            #region [Конструкторы и деструкторы]

            public SymbolRangeEnumerator(SymbolRange range, char startChar = default(char))
            {
                if (startChar == default(char))
                    startChar = range.StartChar;

                Range = range;
                Current = startChar;
            }

            #endregion

        }

        #endregion

        #region [Свойства и Поля]

        /// <summary>
        /// Количество элементов в наборе
        /// </summary>
        public int Count { get { return (EndChar - StartChar); } }

        /// <summary>
        /// Начальный и конечный элементы набора 
        /// </summary>
        public char StartChar, EndChar;

        #endregion

        #region [Методы]

        /// <summary>
        /// Проверяет присутствие символа в данном наборе
        /// </summary>
        /// <param name="symbol">Символ для проверки</param>
        /// <returns></returns>
        public bool Contains(char symbol)
            => symbol.IsInRange(StartChar, EndChar, false);

        /// <summary>
        /// Возравщает перечислитель элементов данного набора
        /// </summary>
        /// <returns></returns>
        public IEnumerator<char> GetEnumerator()
                    => new SymbolRangeEnumerator(this);

        /// <summary>
        /// Получает перечислитель объектов
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator() as IEnumerator;

        #endregion

        /// <summary>
        /// Создает набор по начальному и конечному элементу
        /// </summary>
        /// <param name="start">Начальный элемент</param>
        /// <param name="end">Конечный элемент</param>
        public SymbolRange(char start, char end)
        {
            StartChar = start;
            EndChar = end;
        }

        // TODO: реализовать интерпретацию проверки в IL коде
    }
}