using System.Collections.Generic;
using System.IO;

namespace System
{
    public static class StringExtensions
    {
        public static bool IsNullOrEmpty(this string value) => string.IsNullOrEmpty(value);

        public static bool IsNullOrWhiteSpace(this string value) => string.IsNullOrWhiteSpace(value);

        public static string EnsureEndsWithDot(this string value) => value.EndsWith(".") ? value : $"{value}.";

        public static string PathCombines(this string f1, List<string> f2s)
        {
            var path = f1;
            foreach (var f2 in f2s)
            {
                path = Path.Combine(path, f2);
            }

            return path;
        }

        public static string ToUrl(this string path)
        {
            return "" + path;
        }

        public static long UnixTimeNow()
        {
            var timeSpan = (DateTime.UtcNow - new DateTime(1970, 1, 1, 0, 0, 0));
            return (long)timeSpan.TotalSeconds;
        }
    }
}
