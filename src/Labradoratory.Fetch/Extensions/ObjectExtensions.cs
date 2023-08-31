namespace Labradoratory.Fetch.Extensions
{
    /// <summary>
    /// Methods to extend the functionality of <see cref="object"/>.
    /// </summary>
    public static class ObjectExtensions
    {
        /// <summary>
        /// Throws if the object is null, otherwise returns the object.
        /// </summary>
        /// <param name="target"></param>
        /// <param name="errorMessage">A message to include if exception is thrown.</param>
        /// <returns></returns>
        /// <exception cref="ArgumentNullException">The <paramref name="target"/> was null.</exception>
        public static T ThrowIfNull<T>(this T? target, string? errorMessage = null)
        {
            if(target == null)
                throw new ArgumentNullException(errorMessage ?? "Expected value to not be null.");

            return target;
        }

        /// <summary>
        /// Calls the <see cref="object.ToString"/> method and throws if the value is null.
        /// </summary>
        /// <param name="obj"></param>
        /// <returns>The string value returned from <see cref="object.ToString"/>.</returns>
        /// <exception cref="ArgumentNullException"><see cref="object.ToString"/> returned null.</exception>
        public static string ToStringNotNull(this object obj)
        {
            var value = obj.ToString();
            if (value == null)
                throw new ArgumentNullException($"The {typeof(object).Name} return null from ToString().");

            return value;
        }
    }
}
