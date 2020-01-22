using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.RegularExpressions;

namespace Labradoratory.Fetch.ChangeTracking
{
    public class ChangePath
    {
        public static readonly ChangePath Empty = new ChangePath();

        public static ChangePath Create(IChangePathPart part)
        {
            return Empty.Append(part);
        }

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

        public IReadOnlyList<IChangePathPart> Parts { get; }

        public ChangePath Append(IChangePathPart part)
        {
            return new ChangePath(Parts, part, Target);
        }

        public ChangePath AppendProperty(string property)
        {
            return Append(new ChangePathProperty(property));
        }

        public ChangePath AppendIndex(string index)
        {
            return Append(new ChangePathIndex(index));
        }

        public ChangePath AppendKey(object key)
        {
            return Append(new ChangePathKey(key));
        }

        public ChangePath WithTarget(ChangeTarget target)
        {
            return new ChangePath(Parts, target);
        }

        public override string ToString()
        {
            return ToString('.');
        }

        public string ToString(char separator)
        {
            return string.Join(separator, Parts);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChangePath ck))
                return false;

            return Parts.SequenceEqual(ck.Parts);
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

    public interface IChangePathPart
    {

    }

    public class ChangePathProperty : IChangePathPart
    {
        public ChangePathProperty(string property)
        {
            Property = property;
        }

        public string Property { get; }

        public override string ToString()
        {
            return Property;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChangePathProperty cpp))
                return false;

            return Property == cpp.Property;
        }

        public override int GetHashCode()
        {
            return $"Property{Property}".GetHashCode();
        }
    }

    public class ChangePathIndex : IChangePathPart
    {
        public static readonly Regex IndexRegex = new Regex(@"^(\d*|-)$", RegexOptions.Compiled);

        public ChangePathIndex(string index)
        {
            if (!IndexRegex.IsMatch(index))
                throw new ArgumentException(nameof(index), "Index value must be a positive integer or '-'.");

            Index = index;
        }

        public string Index { get; }

        public override string ToString()
        {
            return Index;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChangePathIndex cpi))
                return false;

            return Index == cpi.Index;
        }

        public override int GetHashCode()
        {
            return $"Index{Index}".GetHashCode();
        }
    }

    public class ChangePathKey : IChangePathPart
    {
        public ChangePathKey(object key)
        {
            Key = key.ToString();
        }

        public string Key { get; }

        public override string ToString()
        {
            return Key;
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChangePathKey cpk))
                return false;

            return Key == cpk.Key;
        }

        public override int GetHashCode()
        {
            return $"Key{Key}".GetHashCode();
        }
    }
}
