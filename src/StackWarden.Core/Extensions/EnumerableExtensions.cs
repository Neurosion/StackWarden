using System.Collections.Generic;
using System.Linq;

namespace StackWarden.Core.Extensions
{
    public static class EnumerableExtensions
    {
        public static bool IsNullOrEmpty<T>(this IEnumerable<T> source)
        {
            return !source?.Any() ?? true;
        }
    }
}