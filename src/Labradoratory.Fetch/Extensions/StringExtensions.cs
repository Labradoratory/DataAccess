using System;

namespace Labradoratory.Fetch.Extensions
{
    /// <summary>
    /// Methods to make working with a string a little easier.
    /// </summary>
    public static class StringExtensions
    {
        /// <summary>
        /// Converts a string to camelCase.
        /// </summary>
        /// <param name="value">The value.</param>
        /// <returns>The string in camelCase.</returns>
        public static string ToCamelCase(this string value)
        {
            var count = 0;
            foreach(var c in value)
            {
                if (char.IsLower(c))
                    break;

                count++;
            }

            if (count == 0)
                return value;

            if (count == 1)
                return char.ToLower(value[0]) + value.Substring(1);

            var acronymLength = count - 1;
            return value.Substring(0, acronymLength).ToLower() + value.Substring(acronymLength);
        }
    }
}
