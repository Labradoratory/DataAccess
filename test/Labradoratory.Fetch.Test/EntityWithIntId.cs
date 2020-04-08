using System;

namespace Labradoratory.Fetch.Test
{
    public abstract class EntityWithIntId : Entity
    {
        public override object[] GetKeys()
        {
            return ToKeys(Id);
        }

        public override string EncodeKeys()
        {
            return Id.ToString();
        }

        public override object[] DecodeKeys(string encodedKeys)
        {
            return new object[] { Convert.ToInt32(encodedKeys) };
        }

        public int Id { get; set; }
    }
}
