using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Labradoratory.Fetch.ChangeTracking
{
    /// <summary>
    /// Represents an immutible path to a change in object hierarchy.
    /// </summary>
    public class ChangePath
    {
        /// <summary>
        /// Represents an empty path.
        /// </summary>
        public static readonly ChangePath Empty = new ChangePath();

        /// <summary>
        /// Creates a <see cref="ChangePath"/> from the provided part.
        /// </summary>
        /// <param name="part">The part to create the path from.</param>
        /// <returns></returns>
        public static ChangePath Create(IChangePathPart part)
        {
            return Empty.Append(part);
        }

        /// <summary>
        /// Creates a <see cref="ChangePath"/> using the provided property name to create 
        /// the initial <see cref="ChangePathProperty"/>.
        /// </summary>
        /// <param name="property">The property.</param>
        /// <returns></returns>
        public static ChangePath Create(string property)
        {
            return Empty.AppendProperty(property);
        }

        private ChangePath(IEnumerable<IChangePathPart> parts, IChangePathPart newPart, ChangeTarget target)
            : this(target)
        {
            Parts = parts.Append(newPart).ToList();
        }

        private ChangePath(IEnumerable<IChangePathPart> parts, ChangeTarget target)
            : this(target)
        {
            Parts = parts.ToList();
        }

        private ChangePath()
            : this(ChangeTarget.Object)
        {
            Parts = new List<IChangePathPart>(0);
        }

        private ChangePath(ChangeTarget target)
        {
            Target = target;
        }

        /// <summary>
        /// Gets or sets the target of the changes in this set.
        /// </summary>
        public ChangeTarget Target { get; }

        /// <summary>
        /// Gets the parts that make up the path.
        /// </summary>
        public IReadOnlyList<IChangePathPart> Parts { get; }

        /// <summary>
        /// Appends the part to the end, creating a new <see cref="ChangePath"/>.
        /// </summary>
        /// <param name="part">The part to append.</param>
        /// <returns>A new <see cref="ChangePath"/> with the part appended.</returns>
        public ChangePath Append(IChangePathPart part)
        {
            return new ChangePath(Parts, part, Target);
        }

        /// <summary>
        /// Appends the provided property name, creating a new <see cref="ChangePath"/>.
        /// </summary>
        /// <param name="property">The property to append.</param>
        /// <returns>A new <see cref="ChangePath"/> with the <see cref="ChangePathProperty"/> appended.</returns>
        public ChangePath AppendProperty(string property)
        {
            return Append(new ChangePathProperty(property));
        }

        /// <summary>
        /// Appends the provided index, creating a new <see cref="ChangePath"/>.
        /// </summary>
        /// <param name="index">The index to append.</param>
        /// <returns>A new <see cref="ChangePath"/> with the <see cref="ChangePathIndex"/> appended.</returns>
        public ChangePath AppendIndex(string index)
        {
            return Append(new ChangePathIndex(index));
        }

        /// <summary>
        /// Appends the provided index, creating a new <see cref="ChangePath"/>.
        /// </summary>
        /// <param name="index">The index to append.</param>
        /// <returns>A new <see cref="ChangePath"/> with the <see cref="ChangePathIndex"/> appended.</returns>
        public ChangePath AppendIndex(int index)
        {
            return AppendIndex(index.ToString());
        }

        /// <summary>
        /// Appends the provided key, creating a new <see cref="ChangePath"/>.
        /// </summary>
        /// <param name="key">The key to append.</param>
        /// <returns>A new <see cref="ChangePath"/> with the <see cref="ChangePathKey"/> appended.</returns>
        public ChangePath AppendKey(object key)
        {
            return Append(new ChangePathKey(key));
        }

        /// <summary>
        /// Creates a new <see cref="ChangePath"/> with the specified target.
        /// </summary>
        /// <param name="target">The target of the path.</param>
        /// <returns>A new <see cref="ChangePath"/> with the specified target.</returns>
        public ChangePath WithTarget(ChangeTarget target)
        {
            return new ChangePath(Parts, target);
        }

        /// <summary>
        /// Converts the <see cref="ChangePath"/> to its string representation.
        /// </summary>
        /// <remarks>The '.' is used to separate the parts.</remarks>
        public override string ToString()
        {
            return ToString('.');
        }

        /// <summary>
        /// Converts the <see cref="ChangePath"/> to its string representation,
        /// using the provided charater to separate the parts.
        /// </summary>
        public string ToString(char separator)
        {
            return string.Join(separator, Parts);
        }

        /// <summary>
        /// Determines whether or not the <see cref="ChangePath"/> starts with
        /// the provided <paramref name="changePath"/>.
        /// </summary>
        /// <param name="changePath">The <see cref="ChangePath"/> to check that is starts with.</param>
        /// <returns></returns>
        public bool StartsWith(ChangePath changePath)
        {
            using (var enumerator1 = changePath.Parts.GetEnumerator())
            using (var enumerator2 = Parts.GetEnumerator())
            {
                while(enumerator1.MoveNext())
                {
                    // If we can't move any further with the current path,
                    // then the path we are checking is longer and cannot
                    // possible be a match.
                    if (!enumerator2.MoveNext())
                        return false;

                    // If a part doesn't match, then it doesn't start the path we are checking.
                    if (!Equals(enumerator1.Current, enumerator2.Current))
                        return false;
                }

                // We matched all of the parts of the path we were checking,
                // so it does start with the path.
                return true;
            }
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ChangePath ck))
                return false;

            return Parts.SequenceEqual(ck.Parts);
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table. 
        /// </returns>
        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

    /// <summary>
    /// A marker interface, identifying a part of a <see cref="ChangePath"/>.
    /// </summary>
    public interface IChangePathPart
    {}

    /// <summary>
    /// Represents a property in a <see cref="ChangePath"/>.
    /// </summary>
    /// <seealso cref="Labradoratory.Fetch.ChangeTracking.IChangePathPart" />
    public class ChangePathProperty : IChangePathPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePathProperty"/> class.
        /// </summary>
        /// <param name="property">The property.</param>
        public ChangePathProperty(string property)
        {
            Property = property;
        }

        /// <summary>
        /// Gets the property.
        /// </summary>
        public string Property { get; }

        /// <summary>
        /// Converts the part to its string representation.
        /// </summary>
        public override string ToString()
        {
            return Property;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ChangePathProperty cpp))
                return false;

            return Property == cpp.Property;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return $"Property{Property}".GetHashCode();
        }
    }

    /// <summary>
    /// Represents an index (Array, List or other) in a <see cref="ChangePath"/>.
    /// </summary>
    /// <seealso cref="Labradoratory.Fetch.ChangeTracking.IChangePathPart" />
    public class ChangePathIndex : IChangePathPart
    {
        /// <summary>
        /// The regular expression that an index value must match.
        /// </summary>
        public static readonly Regex IndexRegex = new Regex(@"^(\d*|-)$", RegexOptions.Compiled);

        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePathIndex"/> class.
        /// </summary>
        /// <param name="index">The index.</param>
        /// <exception cref="ArgumentException">index - Index value must be a positive integer or '-'.</exception>
        public ChangePathIndex(string index)
        {
            if (!IndexRegex.IsMatch(index))
                throw new ArgumentException(nameof(index), "Index value must be a positive integer or '-'.");

            Index = index;
        }

        /// <summary>
        /// Gets the index.
        /// </summary>
        public string Index { get; }

        /// <summary>
        /// Converts the part to its string representation.
        /// </summary>
        public override string ToString()
        {
            return Index;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ChangePathIndex cpi))
                return false;

            return Index == cpi.Index;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return $"Index{Index}".GetHashCode();
        }
    }

    /// <summary>
    /// Represents a key (Dictionary or other) in a <see cref="ChangePath"/>.
    /// </summary>
    /// <seealso cref="Labradoratory.Fetch.ChangeTracking.IChangePathPart" />
    public class ChangePathKey : IChangePathPart
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="ChangePathKey"/> class.
        /// </summary>
        /// <param name="key">The key.</param>
        public ChangePathKey(object key)
        {
            Key = key.ToString();
        }

        /// <summary>
        /// Gets the key.
        /// </summary>
        public string Key { get; }

        /// <summary>
        /// Converts the part to its string representation.
        /// </summary>
        public override string ToString()
        {
            return Key;
        }

        /// <summary>
        /// Determines whether the specified <see cref="System.Object" />, is equal to this instance.
        /// </summary>
        /// <param name="obj">The <see cref="System.Object" /> to compare with this instance.</param>
        /// <returns>
        ///   <c>true</c> if the specified <see cref="System.Object" /> is equal to this instance; otherwise, <c>false</c>.
        /// </returns>
        public override bool Equals(object obj)
        {
            if (!(obj is ChangePathKey cpk))
                return false;

            return Key == cpk.Key;
        }

        /// <summary>
        /// Returns a hash code for this instance.
        /// </summary>
        /// <returns>
        /// A hash code for this instance, suitable for use in hashing algorithms and data structures like a hash table.
        /// </returns>
        public override int GetHashCode()
        {
            return $"Key{Key}".GetHashCode();
        }
    }
}
