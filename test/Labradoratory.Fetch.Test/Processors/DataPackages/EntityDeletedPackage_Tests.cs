using Labradoratory.Fetch.Processors.DataPackages;
using Xunit;

namespace Labradoratory.Fetch.Test.Processors.DataPackages
{
    public class EntityDeletedPackage_Tests
    {
        [Fact]
        public void Ctor_EntityMatch()
        {
            var expectedEntity = new TestEntity();
            var subject = new EntityDeletedPackage<TestEntity>(expectedEntity);
            Assert.Same(expectedEntity, subject.Entity);
        }

        private class TestEntity : EntityWithIntId
        { }
    }
}
