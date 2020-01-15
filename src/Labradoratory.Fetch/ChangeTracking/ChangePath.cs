using System;
using System.Collections.Generic;
using System.Linq;

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

        private ChangePath(IEnumerable<IChangePathPart> parts, IChangePathPart newPart)
        {
            Action = ChangeAction.None;
            Parts = parts.Append(newPart).ToList();
        }

        private ChangePath(IEnumerable<IChangePathPart> parts, ChangeAction action)
        {
            Action = action;
            Parts = parts.ToList();
        }

        private ChangePath()
        {
            Action = ChangeAction.None;
            Parts = new List<IChangePathPart>(0);
        }

        public ChangeAction Action { get; }

        public IReadOnlyList<IChangePathPart> Parts { get; }

        public ChangePath Append(IChangePathPart part)
        {
            return new ChangePath(Parts, part);
        }

        public ChangePath AppendProperty(string property)
        {
            return Append(new ChangePathProperty(property));
        }

        public ChangePath AppendIndex(int index)
        {
            return Append(new ChangePathIndex(index));
        }

        public ChangePath AppendKey(object key)
        {
            return Append(new ChangePathKey(key));
        }

        public ChangePath WithAction(ChangeAction action)
        {
            return new ChangePath(Parts, action);
        }

        public override string ToString()
        {
            return ToString('.');
        }

        public string ToString(char separator)
        {
            return $"{Action}:{string.Join(separator, Parts)}";
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChangePath ck))
                return false;

            return Action == ck.Action && Parts.SequenceEqual(ck.Parts);
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
        public ChangePathIndex(int index)
        {
            Index = index;
        }

        public int Index { get; }

        public override string ToString()
        {
            return Index.ToString();
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
