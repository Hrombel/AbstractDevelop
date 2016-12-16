using System;

namespace AbstractDevelop
{
    /// <summary>
    /// Описывает объект поставщика расширений
    /// </summary>
    public interface IExtensibilityProvider
    {
        /// <summary>
        /// Производит импорт объекта указанного типа
        /// </summary>
        /// <typeparam name="T">Тип импортируемого объекта</typeparam>
        T Import<T>();

        /// <summary>
        /// Производит импорт объекта указнного типа с параметрами контракта
        /// </summary>
        /// <typeparam name="T">Тип импортируемого объекта</typeparam>
        /// <param name="contractName">Имя контракта для импорта</param>
        /// <param name="contractType">Тип контракта для импорта</param>
        /// <returns></returns>
        T Import<T>(string contractName, Type contractType);

        /// <summary>
        /// Производит экспорт объекта заданного типа
        /// </summary>
        /// <typeparam name="T">Тип экспортируемого объекта</typeparam>
        /// <param name="value">Объект для экспорта</param>
        void Export<T>(T value);

        /// <summary>
        /// Производит экспорт объекта заданного типа с параметрами контракта
        /// </summary>
        /// <typeparam name="T">Тип экспортируемого объекта</typeparam>
        /// <param name="value">Объект для экспорта</param>
        /// <param name="contractName">Имя контракта для экспорта</param>
        /// <param name="contractType">Тип контракта для экспорта</param>
        void Export<T>(T value, string contractName, Type contractType);
    }
}
