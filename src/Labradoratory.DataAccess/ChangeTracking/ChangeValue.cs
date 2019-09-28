using System;
using System.Collections.Generic;
using System.Text;

namespace Labradoratory.DataAccess.ChangeTracking
{
    public class ChangeValue
    {
        public object CurrentValue { get; set; }

        public object OldValue { get; set; }
    }
}
