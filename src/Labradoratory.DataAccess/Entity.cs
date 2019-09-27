using Labradoratory.DataAccess.ChangeTracking;

namespace Labradoratory.DataAccess
{
    public abstract class Entity : ChangeTrackingObject
    {
        public abstract object[] GetKeys();
    }
}
