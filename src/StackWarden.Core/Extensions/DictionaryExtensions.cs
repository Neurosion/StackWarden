using System.Collections.Generic;

namespace StackWarden.Core.Extensions
{
    public static class DictionaryExtensions
    {
        public static TValue Get<TKey, TValue>(this IDictionary<TKey, TValue> source, TKey key, TValue @default = default(TValue))
        {
            return source.ContainsKey(key)
                        ? source[key]
                        : @default;
        }
    }
}