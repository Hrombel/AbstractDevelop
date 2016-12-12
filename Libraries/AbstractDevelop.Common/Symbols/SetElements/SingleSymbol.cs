using System.Collections;
using System.Collections.Generic;

namespace AbstractDevelop.Symbols
{
    public struct SingleSymbol :
        ISymbolSetElement
    {

        #region [Свойства и Поля]

        /// <summary>
        /// Количество элементов в наборе
        /// </summary>
        public int Count { get { return 1; } }

        /// <summary>
        /// Символ, представленный данным элементом
        /// </summary>
        public char Symbol;

        #endregion [Свойства и Поля]

        #region [Методы]

        public static implicit operator SingleSymbol(char value)
                    => new SingleSymbol(value);

        /// <summary>
        /// Проверяет присутствие символа в данном наборе
        /// </summary>
        /// <param name="symbol">Символ для проверки</param>
        /// <returns></returns>
        public bool Contains(char symbol)
            => symbol == Symbol;

        /// <summary>
        /// Возравщает перечислитель элементов данного набора
        /// </summary>
        /// <returns></returns>
        public IEnumerator<char> GetEnumerator()
        {
            yield return Symbol;
            yield break;
        }

        /// <summary>
        /// Получает перечислитель объектов
        /// </summary>
        /// <returns></returns>
        IEnumerator IEnumerable.GetEnumerator()
            => GetEnumerator() as IEnumerator;

        #endregion [Методы]

        #region [Конструкторы и деструкторы]

        /// <summary>
        /// 
        /// </summary>
        /// <param name="value"></param>
        public SingleSymbol(char value)
        {
            Symbol = value;
        }

        #endregion [Конструкторы и деструкторы]

    }
}