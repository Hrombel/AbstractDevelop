using System.Collections;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;

namespace AbstractDevelop.Symbols
{
    public class SymbolGroup :
        ISymbolSetElement
    {
        #region [Свойства и Поля]

        /// <summary>
        /// Категория символа Юникода
        /// </summary>
        public UnicodeCategory Category { get; }

        /// <summary>
        /// Количество элементов в наборе
        /// </summary>
        public int Count => this.Count();

        #endregion

        #region [Методы]

        /// <summary>
        /// Проверяет присутствие символа в данном наборе
        /// </summary>
        /// <param name="symbol">Символ для проверки</param>
        /// <returns></returns>
        public bool Contains(char symbol)
            => char.GetUnicodeCategory(symbol) == Category;

        /// <summary>
        /// Возравщает перечислитель элементов данного набора
        /// </summary>
        /// <returns></returns>
        public IEnumerator<char> GetEnumerator()
        {
            for (char c = char.MinValue; c < char.MaxValue; c++)
            {
                if (Contains(c))
                    yield return c;
            }
        }

        /// <summary>
        /// Получает перечислитель объектов
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator();

        #endregion

        #region [Конструкторы и деструкторы]

        public SymbolGroup(UnicodeCategory category)
        {
            Category = category;
        }

        #endregion
    }
}