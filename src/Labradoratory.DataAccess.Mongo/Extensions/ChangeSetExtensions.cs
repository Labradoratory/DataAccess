using Labradoratory.DataAccess.ChangeTracking;
using MongoDB.Driver;

namespace Labradoratory.DataAccess.Mongo.Extensions
{
    public static class ChangeSetExtensions
    {
        /// <summary>
        /// Converts a <see cref="ChangeSet"/> into an <see cref="UpdateDefinition{TDocument}"/>.
        /// </summary>
        /// <typeparam name="T">The type of entity being updated.</typeparam>
        /// <param name="changeSet">The change set.</param>
        /// <returns>The <see cref="UpdateDefinition{TDocument}"/> that can be used to apply the changes.</returns>
        public static UpdateDefinition<T> CreateUpdateDefinition<T>(this ChangeSet changeSet)
        {
            var ud = Builders<T>.Update.Combine();
            switch(changeSet.Target)
            {
                case ChangeTarget.Object:
                    return CreateUpdateDefinitionForObject(changeSet, ud);
                case ChangeTarget.Collection:
                    return CreateUpdateDefinitionForCollection(changeSet, ud);
                case ChangeTarget.Dictionary:
                    return CreateUpdateDefinitionForDictionary(changeSet, ud);
            }

            return null;
        }

        private static UpdateDefinition<T> CreateUpdateDefinitionForDictionary<T>(
            ChangeSet changeSet, 
            UpdateDefinition<T> updateDefinition)
        {
            foreach (var change in changeSet)
            {
                switch (change.Value.Action)
                {
                    case ChangeAction.Add:
                        break;
                    case ChangeAction.Remove:
                        break;
                    case ChangeAction.Update:
                        break;
                }
            }

            return updateDefinition;
        }

        private static UpdateDefinition<T> CreateUpdateDefinitionForCollection<T>(
            ChangeSet changeSet,
            UpdateDefinition<T> updateDefinition)
        {
            foreach(var change in changeSet)
            {
                switch(change.Value.Action)
                {
                    case ChangeAction.Add:
                        break;
                    case ChangeAction.Remove:
                        break;
                }
            }

            return updateDefinition;
        }

        private static UpdateDefinition<T> CreateUpdateDefinitionForObject<T>(
            ChangeSet changeSet,
            UpdateDefinition<T> updateDefinition)
        {
            foreach(var change in changeSet)
            {
                updateDefinition = updateDefinition.Set(change.Key, change.Value.NewValue);
            }

            return updateDefinition;
        }
    }
}
