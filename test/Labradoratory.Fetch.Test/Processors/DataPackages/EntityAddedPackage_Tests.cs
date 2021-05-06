using Labradoratory.Fetch.Processors.DataPackages;
using Xunit;

namespace Labradoratory.Fetch.Test.Processors.DataPackages
{
    public class EntityAddedPackage_Tests
    {
        [Fact]
        public void Ctor_EntityMatch()
        {
            var expectedEntity = new TestEntity();
            var subject = new EntityAddedPackage<TestEntity>(expectedEntity);
            Assert.Same(expectedEntity, subject.Entity);
        }

        private class TestEntity : EntityWithIntId
        { }
    }
}
