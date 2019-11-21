using AutoMapper;
using Labradoratory.Fetch.Controllers;
using Microsoft.AspNetCore.Authorization;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.Test.Controllers
{
    public class EntityRepositoryController_Tests
    {
        [Fact]
        public void Ctor_NoViewType_EntityAsView()
        {
            var mockSubject = new Mock<EntityRepositoryController<TestEntity>>(
                MockBehavior.Strict,
                null,
                null,
                null);

            Assert.True(mockSubject.Object is EntityRepositoryController<TestEntity, TestEntity>);
        }

        

        public class TestController : EntityRepositoryController<TestEntity>
        {
            public TestController() 
                : base(Mock.Of<Repository<TestEntity>>(), Mock.Of<IMapper>(), Mock.Of<IAuthorizationService>())
            {
            }
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
