using System;
using System.Collections;
using System.Collections.Generic;
using System.IO;
using System.Linq;

namespace AbstractDevelop
{
    /// <summary>
    /// Представляет методы расширения для работы с разными типами данных
    /// </summary>
    public static class Tools
    {
        #region [Строковые]

        /// <summary>
        /// Строка, содержащая все недопустимые для пути файла символы.
        /// </summary>
        public static string InvalidPathChars = string.Concat(Path.GetInvalidPathChars());

        /// <summary>
        /// Проверяет путь на наличие недопустимых символов, а также опционально на его существование.
        /// </summary>
        /// <param name="targetPath">Путь для проверки.</param>
        /// <param name="checkExistance">Следуется ли проверять на существование.</param>
        /// <returns>Путь для дальнейшего использования.</returns>
        public static string CheckPath(this string targetPath, bool checkExistance = true)
        {
            if (string.IsNullOrEmpty(targetPath) || targetPath.Contains(InvalidPathChars) || (checkExistance && !(File.Exists(targetPath) || Directory.Exists(targetPath))))
                throw new FileNotFoundException(targetPath);
            else return targetPath;
        }

        #endregion

        #region [Численные]

        /// <summary>
        /// Проверяет индекс на выход за указанне границы.
        /// </summary>
        /// <typeparam name="T">Тип данных, используемый для хранения индекса.</typeparam>
        /// <param name="index">Индекс для проверки.</param>
        /// <param name="min">Минимальная граница.</param>
        /// <param name="max">Максимальная граница.</param>
        /// <returns>Значение индекса для дальнейшего использования.</returns>
        /// <throw>Исключение <see cref="System.ArgumentOutOfRangeException"/></throw>
        public static T CheckIndex<T>(this T index, T min = default(T), T max = default(T))
            where T : IComparable<T>
        {
            if (IsInRange(index, min, max))
                return index;
            else throw new ArgumentOutOfRangeException("Указанный индекс находится за пределами диапазона.");
        }
       
        /// <summary>
        /// Проверяет вхождение значения в заданный промежуток.
        /// </summary>
        /// <typeparam name="T">Тип данных значения.</typeparam>
        /// <param name="value">Значение для проверки.</param>
        /// <param name="start">Начало промежутка.</param>
        /// <param name="end">Конец промежутка.</param>
        /// <param name="isStrictChecking">Показвает, стоит ли трактовать конечное значение как допустимое.</param>
        /// <returns>Наличие факта вхождения данного значения в указанный отрезок.</returns>
        public static bool IsInRange<T>(this T value, T start = default(T), T end = default(T), bool isStrictChecking = true)
           where T : IComparable<T> => value.CompareTo(start) == 0 || isStrictChecking? 
            value.CompareTo(start) * value.CompareTo(end) < 0 : 
            value.CompareTo(start) * value.CompareTo(end) <= 0;

        #endregion

        #region [Коллекции и наборы]

        /// <summary>
        /// Добавление в словарь очередной пары "ключ-значение" с возможностью цепного использования словаря.
        /// </summary>
        /// <typeparam name="TKey">Тип ключа, представленного в словаре.</typeparam>
        /// <typeparam name="TValue">Тип значения, хранимого в словаре.</typeparam>
        /// <param name="source">Исходный словарь.</param>
        /// <param name="key">Ключ для добавления.</param>
        /// <param name="value">Значение для добавления.</param>
        /// <returns>Исхдный словарь <paramref name="source"/>.</returns>
        public static IDictionary<TKey, TValue> AddChain<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (source != null)
                source.Add(key, value);

            return source;
        }

        /// <summary>
        /// Уничтожает все объекты, наследующие <see cref="IDisposable"/> через реализацию данного интерфейса.
        /// </summary>
        /// <remarks>Измеений в наборе не происходит.</remarks>
        /// <param name="source">Исходный набор объектов для уничтожения</param>
        public static void DisposeElements(this IEnumerable source)
        {
            foreach (var element in source)
                (element as IDisposable)?.Dispose();
        }

        /// <summary>
        /// Возвращает указанное значение вместо значения по умолчанию при невозможности получить
        /// последнее значение в наборе.
        /// </summary>
        /// <typeparam name="T">Тип значения, представленных в данном наборе.</typeparam>
        /// <param name="source">Исходный набор объектов.</param>
        /// <param name="defaultValue">Значение, возвращаемое в случае невозможности полуения послднего объекта в наборе.</param>
        /// <returns>Последний объект в наборе, либо заменяющее его значение <paramref name="defaultValue"/>.</returns>
        public static T LastOrDefault<T>(this IEnumerable<T> source, T defaultValue)
            where T : class => source.LastOrDefault() ?? defaultValue;

        #endregion
     }
}