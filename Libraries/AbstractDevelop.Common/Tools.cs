using System;
using System.Collections;
using System.Collections.Generic;
using System.ComponentModel;
using System.IO;
using System.Linq;
using System.Runtime.CompilerServices;
using System.Text;

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

        static StringBuilder builder = new StringBuilder();

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

        public static string Build(this IEnumerable<char> source)
        {
            var local = builder.Clear();
            foreach (var c in source) local.Append(c);
            return local.ToString();
        }

        public static string GetAndClear(this StringBuilder sb)
            => sb.ToString().Do(v => sb.Clear());

        /// <summary>
        /// Удаляет все вхождения символов, соответствующие данному шаблону
        /// </summary>
        /// <param name="source">Исходная строка</param>
        /// <param name="predicate">Шаблон удаляемых символов</param>
        /// <param name="onlyPreceeding">Следует ли удалять знаки только с начала строки</param>
        /// <returns></returns>
        public static string RemoveChars(this string source, Func<char, bool> predicate, bool onlyPreceeding = true)
            => (onlyPreceeding ? source.SkipWhile(predicate) : source.WhereNot(predicate)).Build();

        public static bool RemoveChars(this string source, Func<char, bool> predicate, out string result, bool onlyPreceeding = true)
            => (result = (onlyPreceeding ? source.SkipWhile(predicate) : source.WhereNot(predicate)).Build()) != source;

        public static bool RemoveChars(this string source, out string result, params char[] chars)
            => source.RemoveChars(chars.Contains, out result, false);

        public static string RemoveWhitespaces(this string source, bool onlyPreceeding = true)
            => RemoveChars(source, char.IsWhiteSpace, onlyPreceeding);

        public static string RemoveWhitespaces(this string source, out string output, bool onlyPreceeding = true)
           => output = source.RemoveWhitespaces(onlyPreceeding);

        public static bool Replace(this string source, string old, string @new, out string result)
            => (result = source.Replace(old, @new)) != source;

        public static bool Replace(this string source, char old, char @new, out string result)
            => (result = source.Replace(old, @new)) != source;

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
        public static bool IsInRange<T>(this T value, T start = default(T), T end = default(T))
           where T : IComparable<T>
            => start.CompareTo(value) <= 0 && end.CompareTo(value) >= 0;

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

        public static T Last<T>(this IList<T> source)
            => source[source.Count - 1];

        public static bool Apply<T>(this IEnumerable<T> source, Action<T> action)
            => source?.Try(action.ApplyTo) ?? false;

        public static bool Any<T>(this IEnumerable<T> source, Func<T, bool> predicate, out T value)
        {
            foreach (var item in source)
                if (predicate?.Invoke(item) ?? false)
                {
                    value = item;
                    return true;
                }

            value = default(T);
            return false;
        }

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

        public static T FirstOfType<T>(this IEnumerable source)
            => (T)source.Try(src => src?.Cast<object>().FirstOrDefault(e => e is T));

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
   
        public static T[] Replace<T>(this T[] source, int index, T value)
        {
            if (index < source.Length)
                source[index] = value;
      
            return source;
        }

        /// <summary>
        /// Производит указанное действие над объектом и возвращает его
        /// </summary>
        /// <typeparam name="T"></typeparam>
        /// <param name="target">Объект для действитя</param>
        /// <param name="action">Производимое действие</param>
        /// <returns></returns>
        public static T Do<T>(this T target, Action<T> action)
        {
            action?.Invoke(target);
            return target;
        }

        public static T? Do<T>(this T? target, Action<T> action)
            where T : struct
            => target.Do(t => { if (t.HasValue) t.Value.Do(action); });

        public static bool Execute<T>(this T target, Action<T> action)
        {
            action(target);
            return true;
        }

        public static void ApplyTo<T>(this Action<T> action, IEnumerable<T> target)
        {
            foreach (var e in target)
                action(e);
        }

        public static FileStream OpenFile(this string source, FileMode mode = FileMode.OpenOrCreate)
            => File.Open(source, mode);

        public static TOut UsingFile<TOut>(this string source, Func<FileStream, TOut> processing, FileMode mode = FileMode.OpenOrCreate)
            => OpenFile(source).Using(processing);

        public static TOut UsingFile<P1, TOut>(this string source, Func<FileStream, P1, TOut> processing, P1 param1, FileMode mode = FileMode.OpenOrCreate)
            => OpenFile(source).Using(processing, param1);

        public static T Using<TSource, T>(this TSource target, Func<TSource, T> convert)
            where TSource: IDisposable
        {
            using (target) return convert(target);
        }

        public static T Using<TSource, P1, T>(this TSource target, Func<TSource, P1, T> convert, P1 param1)
            where TSource : IDisposable
        {
            using (target) return convert(target, param1);
        }

        public static bool Try(this Delegate value, Func<Exception, bool> onError = null, params object[] args)
        {
            try
            {
                value?.DynamicInvoke(args);
                return true;
            }
            catch (Exception ex)
            {
                return onError?.Invoke(ex) ?? false;
            }
        }

        /// <summary>
        /// Возвращает пустой делегат, если указанный делегат не задан
        /// </summary>
        /// <typeparam name="T">Тип параметра делегата</typeparam>
        /// <param name="source">Исходный делегат</param>
        /// <returns></returns>
        public static Action<T> OrEmpty<T>(this Action<T> source)
             => source ?? new Action<T>((t) => { });

        public static TRet Try<T, TRet>(this T value, Func<T, TRet> func, TRet defaultValue = default(TRet))
        {
            try { return func(value); }
            catch { return defaultValue; }
        }

        public static T TryGet<T>(this IEnumerable<T> source, int index)
            => source.Try(s => s.Skip(index)).FirstOrDefault();

        public static T TryGet<T>(this IEnumerable<T> source, int index, out IEnumerable<T> skipped)
            => (skipped = source.Skip(index)).FirstOrDefault();

        public static bool Try<T>(this T value, Action<T> action, Action<T> errorAction = null)
            => action.Try(ex => value.Try(errorAction), value);

        public static bool Check<T>(this T target, Func<T, bool> validator)
            => validator(target);
       
        public static bool CheckAll<T>(this T target, params Func<T, bool>[] validators)
            => validators.All(v => Check(target, v));

        public static bool CheckAny<T>(this T target, params Func<T, bool>[] validators)
            => validators.Any(v => Check(target, v));

        public static void Invoke(this PropertyChangedEventHandler @event, object sender = null, [CallerMemberName] string propertyName = null)
            => @event?.Invoke(sender, new PropertyChangedEventArgs(propertyName));

        public static T ToVariable<T>(this T value, out T variable)
            => variable = value;

        public static bool Decision(this bool condition, Action @true = null, Action @false = null)
        {
            if (condition) @true?.Invoke();
            else @false?.Invoke();

            return condition;
        }

        public static bool Decision<T>(this bool condition, Func<T> @true = null, Func<T> @false = null)
        {
            if (condition) @true?.Invoke();
            else @false?.Invoke();

            return condition;
        }

        public static bool Select<T>(this bool condition, T @true, T @false, out T value)
        {
            value = condition ? @true : @false;
            return condition;
        }

        public static bool Select<T>(this Func<bool> condition, Func<T> @true, Func<T> @false, out T value)
            => condition().Select(@true(), @false(), out value);

        public static bool MoveNext<T>(this IEnumerator<T> enumerator, out T variable)
            => enumerator.MoveNext().Select(enumerator.Current, default(T), out variable);
    }
}