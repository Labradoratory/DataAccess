using Labradoratory.Fetch.Authorization;
using Xunit;

namespace Labradoratory.Fetch.Test.Authorization
{
    public class EntityAuthorizationPolicy_Tests
    {
        [Fact]
        public void Ctor_SetName()
        {
            var expectedName = "Test";
            EntityAuthorizationPolicy subject = expectedName;
            Assert.Equal(expectedName, subject.Name);
        }

        [Fact]
        public void ForType_Success()
        {
            var expectedName = "Test";
            EntityAuthorizationPolicy subject = expectedName;
            var result = subject.ForType<TestEntity>();
            Assert.Equal($"{expectedName}-{typeof(TestEntity).Name}", result);
        }

        private class TestEntity
        { }
    }
}
