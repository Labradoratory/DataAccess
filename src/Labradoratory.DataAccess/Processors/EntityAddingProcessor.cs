using System;
using System.Threading.Tasks;

namespace Labradoratory.DataAccess.Processors
{
    public class EntityAddingProcessor<TEntity> : IProcessor<EntityAddingPackage<TEntity>>
    {
        public uint Priority => throw new NotImplementedException();

        public Task ProcessAsync()
        {
            throw new NotImplementedException();
        }
    }

    public class EntityAddingPackage<TEntity> : DataPackage
    {
        public EntityAddingPackage(TEntity entity)
        {
            Entity = entity;
        }

        public TEntity Entity { get; }
    }
}
