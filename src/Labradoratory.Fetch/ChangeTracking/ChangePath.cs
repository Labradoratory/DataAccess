using System;
using System.Collections.Generic;
using System.Linq;

namespace Labradoratory.Fetch.ChangeTracking
{
    public class ChangePath
    {
        public static readonly ChangePath Empty = new ChangePath();

        public static ChangePath Create(ChangePathPart part)
        {
            return Empty.Append(part);
        }

        public static ChangePath Create(string property)
        {
            return Empty.AppendProperty(property);
        }

        private ChangePath(IEnumerable<ChangePathPart> parts, ChangePathPart newPart)
        {
            Parts = parts.Append(newPart).ToList();
        }

        private ChangePath()
        {
            Parts = new List<ChangePathPart>(0);
        }

        public IReadOnlyList<ChangePathPart> Parts { get; }

        public ChangePath Append(ChangePathPart part)
        {
            return new ChangePath(Parts, part);
        }

        public ChangePath AppendProperty(string property)
        {
            return Append(new ChangePathProperty(property));
        }

        public ChangePath AppendAction(ChangeAction action)
        {
            return Append(new ChangePathAction(action));
        }

        public ChangePath AppendIndex(int index)
        {
            return Append(new ChangePathIndex(index));
        }

        public ChangePath AppendKey(object key)
        {
            return Append(new ChangePathKey(key));
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

            var result = Parts.SequenceEqual(ck.Parts);
            return result;
        }

        public override int GetHashCode()
        {
            return ToString().GetHashCode();
        }
    }

    public abstract class ChangePathPart
    {

    }

    public class ChangePathProperty : ChangePathPart
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

    public class ChangePathAction : ChangePathPart
    {
        public ChangePathAction(ChangeAction action)
        {
            Action = action;
        }

        public ChangeAction Action { get; }

        public override string ToString()
        {
            return Action.ToString().ToLower();
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChangePathAction cpa))
                return false;

            return Action == cpa.Action;
        }

        public override int GetHashCode()
        {
            return $"Action{Action}".GetHashCode();
        }
    }

    public class ChangePathIndex : ChangePathPart
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

    public class ChangePathKey : ChangePathPart
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
