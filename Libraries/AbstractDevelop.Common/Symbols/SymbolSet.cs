using System;
using System.Collections.Generic;
using System.Collections.ObjectModel;
using System.Linq;

namespace AbstractDevelop.Symbols
{
    /// <summary>
    /// Представляет набор символов, составленный из различных элементов
    /// </summary>
    public interface ISymbolSet :
       ICollection<ISymbolSetElement>, IDisposable, ICloneable
    {
        /// <summary>
        /// Добавляет ряд символьных элементов в данный набор
        /// </summary>
        /// <param name="elements">Ряд символьных элементов для добавления</param>
        void AddRange(IEnumerable<ISymbolSetElement> elements);

        /// <summary>
        /// Проверяет начилие конкретного символа в данном наборе
        /// </summary>
        /// <param name="symbol">Символ для проверки</param>
        /// <returns></returns>
        bool Contains(char symbol);
    }
    
    /// <summary>
    /// Реализует набор символов, составленный из различных элементов
    /// </summary>
    [Serializable]
    public class SymbolSet :
        Collection<ISymbolSetElement>, ISymbolSet
    {
        /// <summary>
        /// Добавляет ряд символьных элементов в данный набор
        /// </summary>
        /// <param name="elements">Ряд символьных элементов для добавления</param>
        public void AddRange(IEnumerable<ISymbolSetElement> elements)
            => elements?.WhereNot(Contains).Apply(Add);

        /// <summary>
        /// Создает точную копию данного набора символов
        /// </summary>
        /// <returns></returns>
        public object Clone()
            => new SymbolSet(this);

        /// <summary>
        /// Проверяет начилие конкретного символа в данном наборе
        /// </summary>
        /// <param name="symbol">Символ для проверки</param>
        /// <returns></returns>
        public bool Contains(char symbol)
            => this.Any(e => e.Contains(symbol));

        /// <summary>
        /// Освобождает ресурсы, связанные с данным набором символов
        /// </summary>
        public void Dispose()
        {
            this.DisposeElements();
            this.Clear();
        }

        #region [Конструкторы]

        /// <summary>
        /// Создает набор символов и добавляет в него указанный элемент
        /// </summary>
        /// <param name="element"></param>
        public SymbolSet(ISymbolSetElement element = default(ISymbolSetElement)) :
            base()
        {
            if (element != default(ISymbolSetElement))
                Add(element);
        }

        /// <summary>
        /// Создает набор символов из списка элементов 
        /// </summary>
        /// <param name="list">Список элементов для создания набора</param>
        public SymbolSet(IList<ISymbolSetElement> list) : 
            base(list) { }
              
        /// <summary>
        /// Создает набор символов из массива символов
        /// </summary>
        /// <param name="chars">Массив символов для создания набора</param>
        public SymbolSet(params char[] chars) :
            this((SymbolArray)chars) { }
      
        #endregion
    }
}
