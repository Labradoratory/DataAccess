using System;
using System.Collections.Generic;
using System.Text;

namespace Labradoratory.DataAccess.ChangeTracking
{
    public class ChangeValue
    {
        public ChangeAction Action { get; set; }

        public object NewValue { get; set; }

        public object OldValue { get; set; }
    }
}
