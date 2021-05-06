using Labradoratory.Fetch.ChangeTracking;
using Labradoratory.Fetch.Processors.DataPackages;
using Xunit;

namespace Labradoratory.Fetch.Test.Processors.DataPackages
{
    public class EntityUpdatedPackage_Tests
    {
        [Fact]
        public void Ctor_EntityMatch()
        {
            var expectedEntity = new TestEntity();
            var expectedChangeSet = new ChangeSet();
            var subject = new EntityUpdatedPackage<TestEntity>(expectedEntity, expectedChangeSet);
            Assert.Same(expectedEntity, subject.Entity);
            Assert.Same(expectedChangeSet, subject.Changes);
        }

        private class TestEntity : EntityWithIntId
        { }
    }
}
