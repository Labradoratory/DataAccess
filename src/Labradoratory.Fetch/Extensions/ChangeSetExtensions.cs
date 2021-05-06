using System;
using System.Collections.Generic;
using System.Linq;
using System.Text.Json;
using Labradoratory.Fetch.ChangeTracking;
using Microsoft.AspNetCore.JsonPatch.Operations;

namespace Labradoratory.Fetch.Extensions
{
    /// <summary>
    /// Methods to make working with <see cref="ChangeSet"/> a little easier.
    /// </summary>
    public static class ChangeSetExtensions
    {
        /// <summary>
        /// Converts the <see cref="ChangeSet"/> into an array of <see cref="Operation"/>
        /// objects representing a JSON Patch.
        /// </summary>
        /// <param name="changes">The changes to convert.</param>
        /// <returns>The array of <see cref="Operation"/> objects representing a JSON Patch.</returns>
        public static Operation[] ToJsonPatch(this ChangeSet changes)
        {
            var operations = new List<Operation>();
            foreach(var change in changes)
            {
                foreach (var value in change.Value.Where(v => v.Action != ChangeAction.None))
                {
                    var operation = new Operation
                    {
                        op = value.Action.ToOpName(),
                        path = $"/{string.Join('/', change.Key.Parts.Select(p => p.ToString().ToCamelCase()))}"
                    };

                    if (value.Action != ChangeAction.Remove)
                        operation.value = value.NewValue;

                    operations.Add(operation);
                }
            }

            return operations.ToArray();
        }

        private static string ToOpName(this ChangeAction action)
        {
            switch(action)
            {
                case ChangeAction.Add:
                    return "add";
                case ChangeAction.Remove:
                    return "remove";
                case ChangeAction.Update:
                    return "replace";
            }

            throw new ArgumentException(nameof(action), $"{typeof(ChangeAction).Name} cannot be converted into a JsonPatch operation.");
        }


    }
}
