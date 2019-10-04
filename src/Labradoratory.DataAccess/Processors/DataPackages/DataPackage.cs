using System;
using System.Collections.Generic;
using System.Runtime.CompilerServices;

namespace Labradoratory.DataAccess.Processors.DataPackages
{
    /// <summary>
    /// Represents a piece of data that is to be processed.
    /// </summary>
    /// <seealso cref="Dictionary{String, Object}" />
    public abstract class DataPackage : Dictionary<string, object>
    {
        /// <summary>
        /// Gets the previous <see cref="DataPackage"/> that caused this <see cref="DataPackage"/>
        /// to enter the processing pipeline.
        /// </summary>
        /// <remarks>
        /// <para>
        /// Because processing can cause additional changes, it may be necessary to track the
        /// <see cref="DataPackage"/> that resulted in this one.  This can be useful in some
        /// scenarios, such as: processing loops, hierarchical data exchange, etc.
        /// </para>
        /// <para>This property will be <c>null</c> for the first package in a chain.</para>
        /// <para>See <see cref="ProcessorPipeline"/> for more info.</para>
        /// </remarks>
        public DataPackage Previous { get; internal set; }

        /// <summary>
        /// Gets the value from the base dictionary for a specified property.
        /// </summary>
        /// <typeparam name="T">The type of the value to get.</typeparam>
        /// <param name="property">
        /// [Optional] The property to get the value for.
        /// Has the <see cref="CallerMemberNameAttribute"/> which is used if not specified.
        /// </param>
        /// <returns>The value for the specified property.</returns>
        /// <exception cref="ArgumentNullException">property</exception>
        /// <exception cref="KeyNotFoundException">property</exception>
        /// <remarks>
        /// <para>
        /// The <see cref="DataPackage"/> class inherits from <see cref="Dictionary{TKey, TValue}"/>
        /// so that undefined values can be stored in a package for use by other processors.
        /// </para>
        /// <para>
        /// Constant properties of a specialized instance of <see cref="DataPackage"/> should use the 
        /// GetValue/SetValue members to store values in the base dictionary.
        /// </para>
        /// </remarks>
        /// <example>
        /// public class MyDataPackage : DataPackage
        /// {
        ///     public string MyValue
        ///     {
        ///         get => GetValue{string}();
        ///         set => SetValue(value);
        ///     }
        /// } 
        /// </example>
        protected T GetValue<T>([CallerMemberName] string property = null)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            if (TryGetValue(property, out object value))
                return (T)value;

            return default;
        }

        /// <summary>
        /// Sets the value for a specified property.
        /// </summary>
        /// <typeparam name="T">The type of the value being set.</typeparam>
        /// <param name="value">The value.</param>
        /// <param name="property">
        /// [Optional] The name of the property to set the value for.
        /// Has the <see cref="CallerMemberNameAttribute"/> which is used if not specified.
        /// </param>
        /// <exception cref="ArgumentNullException">property</exception>
        protected void SetValue<T>(T value, [CallerMemberName] string property = null)
        {
            if (property == null)
                throw new ArgumentNullException(nameof(property));

            this[property] = value;
        }
    }
}
