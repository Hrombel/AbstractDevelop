using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace AbstractDevelop
{
    public static class LinkTools
    {
        public static bool IsValid(this object value, bool shouldThrowException = true)
        {
            if (value == default(object))
            {
                if (shouldThrowException) throw new NullReferenceException($"");
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
    }
}
