using System;
using System.Collections.Generic;

namespace Labradoratory.DataAccess.ChangeTracking
{
    /// <summary>
    /// Contains a set of changes.
    /// </summary>
    public class ChangeSet : Dictionary<string, ChangeValue>
    {
        /// <summary>
        /// Gets or sets the target of the changes in this set.
        /// </summary>
        public ChangeTarget Target { get; set; }
    }
}
