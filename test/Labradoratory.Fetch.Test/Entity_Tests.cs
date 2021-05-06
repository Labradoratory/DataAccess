using System.Linq;
using Xunit;

namespace Labradoratory.Fetch.Test
{
    public class Entity_Tests
    {
        [Fact]
        public void ToKeys_Static_CreatesArray()
        {
            var expected0 = 100;
            var expected1 = "Test";
            var expected2 = true;

            var result = Entity.ToKeys(expected0, expected1, expected2);
            Assert.Equal(3, result.Length);
            Assert.Equal(expected0, result[0]);
            Assert.Equal(expected1, result[1]);
            Assert.Equal(expected2, result[2]);
        }

        [Fact]
        public void DecodeKeys_Static_UsesTypeSpecificImplementation()
        {
            var result = Entity.DecodeKeys<TestEntity>("ThisValueDoesNotMatterForTestEntity");
            Assert.Equal(TestEntity.DecodedKeyValue, result);
        }

        public class TestEntity : EntityWithIntId
        {
            public const string EncodedKeyValue = "My encoded key value";
            public static object[] DecodedKeyValue = new object[1];

            public override string EncodeKeys()
            {
                return EncodedKeyValue;
            }

            public override object[] DecodeKeys(string encodedKeys)
            {
                return DecodedKeyValue;
            }
        }
    }
}
