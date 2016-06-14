using System;
using System.Collections.Generic;
using System.IO;

namespace AbstractDevelop
{
    public static class LinkTools
    {
        static string InvalidPathChars = string.Concat(Path.GetInvalidPathChars());

        public static bool IsValid(this object value, bool shouldThrowException = true)
        {
            if (value == default(object))
            {
                if (shouldThrowException) throw new NullReferenceException($""); //TODO: локализовать выбрасываемое исключение
                else return false;
            }
            return false;
        }

        public static Dictionary<TKey, TValue> AddChain<TKey, TValue>(this Dictionary<TKey, TValue> source, TKey key, TValue value)
        {
            if (source != null)
                source.Add(key, value);

            return source;
        }

        public static TargetType TryGetArg<TargetType>(this TargetType[] array, int index, bool printTraceInfo = true)
        {
            if (array == null || index > array.Length + 1)
            {
                if (printTraceInfo) ; //TODO: вывод информации о несуществующем аргументе
                return default(TargetType);
            }
            else return array[index];
        }

        public static TargetType LastOrDefault<TargetType>(this IEnumerable<TargetType> source, TargetType defaultValue = default(TargetType))
        {
            var value = source.LastOrDefault();
            return value.Equals(default(TargetType)) ? defaultValue : value;
        }

        public static string CheckPath(this string targetPath, bool checkExistance = true)
        {
            if (string.IsNullOrEmpty(targetPath) || targetPath.Contains(InvalidPathChars) || (checkExistance && !(File.Exists(targetPath) || Directory.Exists(targetPath))))
                throw new FileNotFoundException(targetPath);
            else return targetPath;
        }

        public static T CheckIndex<T>(this T index, T min = default(T), T max = default(T))
            where T : IComparable<T>
        {
            if (index.CompareTo(min) < 0 || !(index.CompareTo(max) < 0))
                throw new ArgumentOutOfRangeException("Указанный индекс находится за пределами диапазона.");

            return index;
        }

        public static bool CheckIndex<T>(this T index, bool generateException, T min = default(T), T max = default(T))
            where T : IComparable<T>
        {
            if (index.CompareTo(min) < 0 || !(index.CompareTo(max) < 0))
                if (generateException)
                    throw new ArgumentOutOfRangeException("Указанный индекс находится за пределами диапазона.");
                else return false;

            return true;
        }
        public static T Self<T>(this T value) => value;
    }
}
