using System;
using System.Collections.Generic;
using System.Text;

namespace Labradoratory.DataAccess.ChangeTracking
{
    public class ChangeTrackingObject
    {
        protected Dictionary<string, ChangeValue> Changes = new Dictionary<string, ChangeValue>();

        protected T GetValue<T>([CallerName])
    }
}
