using System;
using System.Collections.Generic;
using System.Linq;

namespace Labradoratory.Fetch.ChangeTracking
{
    public class ChangeKey
    {
        public ChangeKey Create(ChangePathPart part)
        {
            return new ChangeKey(Enumerable.Empty<ChangePathPart>(), part);
        }

        private ChangeKey(IEnumerable<ChangePathPart> parts, ChangePathPart newPart)
        {
            if (newPart is ChangePathProperty cpp)
                CurrentProperty = cpp;

            Parts = parts.Append(newPart).ToList();
        }

        public IReadOnlyList<ChangePathPart> Parts { get; }

        public ChangePathProperty CurrentProperty { get; }

        public ChangePathAction CurrentAction { get; }

        public ChangeKey Append(ChangePathPart part)
        {
            return new ChangeKey(Parts, part);
        }

        public override string ToString()
        {
            return string.Join('.', Parts);
        }

        public override bool Equals(object obj)
        {
            if (!(obj is ChangeKey ck))
                return false;

            return Parts.SequenceEqual(ck.Parts);
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
            if (!(obj is ChangePathAction cpp))
                return false;

            return Action == cpp.Action;
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
            if (!(obj is ChangePathIndex cpp))
                return false;

            return Index == cpp.Index;
        }

        public override int GetHashCode()
        {
            return $"Index{Index}".GetHashCode();
        }
    }
}
