using System.Collections.Generic;

namespace AbstractDevelop.Symbols
{

    /// <summary>
    /// Представляет определение элемента набора символов
    /// </summary>
    /// <remarks>Может являться как промежутком, так и единичным символом</remarks>
    public interface ISymbolSetElement :
        IEnumerable<char>
    {
        /// <summary>
        /// Количество элементов в наборе
        /// </summary>
        int Count { get; }

        /// <summary>
        /// Проверяет присутствие символа в данном наборе
        /// </summary>
        /// <param name="symbol">Символ для проверки</param>
        /// <returns></returns>
        bool Contains(char symbol);
    }
}