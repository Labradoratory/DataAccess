using Labradoratory.Fetch.ChangeTracking;
using Labradoratory.Fetch.Processors.DataPackages;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.Test.Processors.DataPackages
{
    public class BaseEntityDataPackage_Tests
    {
        [Fact]
        public void Ctor_EntitySet()
        {
            var expectedEntity = ChangeTrackingObject.CreateTrackable<TestEntity>();
            var mockSubject = new Mock<BaseEntityDataPackage<TestEntity>>(
                MockBehavior.Strict,
                expectedEntity);

            Assert.Same(expectedEntity, mockSubject.Object.Entity);
        }

        public class TestEntity : EntityWithIntId
        {
            public string StringValue
            {
                get => GetValue<string>();
                set => SetValue(value);
            }
        }
    }
}
