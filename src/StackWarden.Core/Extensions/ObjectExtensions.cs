using System.Text.RegularExpressions;

namespace StackWarden.Core.Extensions
{
    public static class ObjectExtensions
    {
        public static string ToExpandedString(this object source)
        {
            var rawString = source.ToString();
            var expandedString = Regex.Replace(rawString, "([a-z])([A-Z])", "$1 $2");

            return expandedString;
        }
    }
}