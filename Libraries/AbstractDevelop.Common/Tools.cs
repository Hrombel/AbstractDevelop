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
        /// Проверяет путь на корректность
        /// </summary>
        /// <param name="targetPath">Путь, который нужно проверить</param>
        /// <returns></returns>
        public static bool IsValidPath(this string targetPath)
            => (string.IsNullOrEmpty(targetPath) || targetPath.Contains(InvalidPathChars));

        /// <summary>
        /// Загружает информацию о содержимом пути, если данные по нему существуют
        /// </summary>
        /// <param name="targetPath">Путь, информацию о котором необходимо загрузить</param>
        /// <returns></returns>
        public static FileSystemInfo GetPathValue(this string targetPath)
        {
            return LoadInfo(new FileInfo(targetPath)) ?? LoadInfo(new DirectoryInfo(targetPath));
            // загрузчик информации об элементах
            FileSystemInfo LoadInfo(FileSystemInfo info)
                => info.Exists ? info : default(FileSystemInfo);
        }

        /// <summary>
        /// Проверяет путь на наличие недопустимых символов, а также опционально на его существование.
        /// </summary>
        /// <param name="targetPath">Путь для проверки.</param>
        /// <param name="checkExistance">Следуется ли проверять на существование.</param>
        /// <returns>Путь для дальнейшего использования.</returns>
        public static string CheckPath(this string targetPath, bool checkExistance = true)
            => IsValidPath(targetPath) && (!checkExistance || GetPathValue(targetPath) != default(FileSystemInfo)) ?
                throw new FileNotFoundException(targetPath) : targetPath;

        public static string RemoveWhitespaces(this string source)
            => new string(source.SkipWhile(char.IsWhiteSpace).ToArray());

        public static string RemoveWhitespaces(this string source, out string output)
           => output = new string (source.SkipWhile(char.IsWhiteSpace).ToArray());

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
            => IsInRange(index, min, max)? index : throw new ArgumentOutOfRangeException("Указанный индекс находится за пределами диапазона.");
               
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
            source?.Add(key, value);
            return source;
        }

        /// <summary>
        /// Устаналвивает или добавляет значение ключа в словррь
        /// </summary>
        public static IDictionary<TKey, string> Set<TKey, TValue>(this IDictionary<TKey, string> source, TKey key, TValue value)
        {
            if (source.ContainsKey(key))
                source[key] = value.ToString();
            else source.Add(key, value.ToString());

            return source;
        }

        public static T TryParse<T>(this IDictionary<string, string> source, string key, Func<string, T> converter, T defaultValue = default(T))
            =>  source.TryGetValue(key, out var value) ? converter(value) : defaultValue;

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
        
        //TODO: сделать описание реализованных функций

        public static IEnumerable<T> WhereNot<T>(this IEnumerable<T> source, Func<T, bool> predicate)
        {
            if (source != null)
            {
                foreach (var element in source)
                    if (!predicate(element))
                        yield return element;
            }
            yield break;
        }

        public static int IndexOf<T>(this IEnumerable<T> source, T target)
            where T : IComparable<T>
        {
            if (source != null)
            {
                int index = 0;
                foreach (var element in source)
                {
                    if (element.CompareTo(target) == 0)
                        return index;
                    else index++;
                }
            }

            return -1;
        }

        public static IDictionary<TKey, TValue> ForEach<TKey, TValue>(IDictionary<TKey, TValue> source, Func<TValue, TValue> func)
        {
            foreach (var key in source.Keys)
                source[key] = func(source[key]);
            return source;
        }

        #endregion

        #region [Потоки]

        /// <summary>
        /// Создает объект для чтения потока данных
        /// </summary>
        /// <returns></returns>
        public static StreamReader CreateReader(this Stream stream) 
            => new StreamReader(stream);

        /// <summary>
        /// Создает объект для записи в поток данных
        /// </summary>
        /// <returns></returns>
        public static StreamWriter CreateWriter(this Stream stream, bool autoFlush = true, bool append = true) 
            => new StreamWriter(stream) { AutoFlush = autoFlush }.Do(sw => sw.BaseStream.Seek(0, append? SeekOrigin.End : SeekOrigin.Current)) ;

        #endregion

        #region [Рефлексия]

        static Type
            baseObjectType = typeof(object),
            baseValueType = typeof(ValueType);

        public static bool BasedOn(this Type checkingType, Type baseType)
         => checkingType.IsSubclassOf(baseType) || checkingType.GetInterfaces().Any(interfaceType => interfaceType == baseType);
        
        #endregion

        /// <summary>
        /// Производит указанное действие над объектом и возвращает его
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">Объект для действитя</param>
        /// <param name="action">Производимое действие</param>
        /// <returns></returns>
        public static T Do<T>(this T target, Action<T> action)
        {
            action(target);
            return target;
        }
     }
}