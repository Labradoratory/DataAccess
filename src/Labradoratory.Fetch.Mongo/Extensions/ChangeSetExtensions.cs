using Labradoratory.Fetch.ChangeTracking;
using MongoDB.Driver;

namespace Labradoratory.Fetch.Mongo.Extensions
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
            foreach (var change in changeSet)
            {
                switch (change.Value.Target)
                {
                    case ChangeTarget.Object:
                        return CreateUpdateDefinitionForObject(change.Key, change.Value, ud);
                    case ChangeTarget.Collection:
                        return CreateUpdateDefinitionForCollection(change.Key, change.Value, ud);
                    case ChangeTarget.Dictionary:
                        return CreateUpdateDefinitionForDictionary(change.Key, change.Value, ud);
                }
            }

            return null;
        }

        private static UpdateDefinition<T> CreateUpdateDefinitionForDictionary<T>(
            string path,
            ChangeValue value,
            UpdateDefinition<T> updateDefinition)
        {
            switch (value.Action)
            {
                case ChangeAction.Add:
                    updateDefinition = updateDefinition.Set(path, value.NewValue);
                    break;
                case ChangeAction.Remove:
                    updateDefinition = updateDefinition.Unset(path);
                    break;
            }

            return updateDefinition;
        }

        private static UpdateDefinition<T> CreateUpdateDefinitionForCollection<T>(
            string path,
            ChangeValue value,
            UpdateDefinition<T> updateDefinition)
        {
            switch (value.Action)
            {
                case ChangeAction.Add:
                    updateDefinition.Push(path, value.NewValue);
                    break;
                case ChangeAction.Remove:
                    updateDefinition.Pull(path, value.OldValue);
                    break;
            }

            return updateDefinition;
        }

        private static UpdateDefinition<T> CreateUpdateDefinitionForObject<T>(
            string path,
            ChangeValue value,
            UpdateDefinition<T> updateDefinition)
        {
            return updateDefinition.Set(path, value.NewValue);
        }
    }
}
