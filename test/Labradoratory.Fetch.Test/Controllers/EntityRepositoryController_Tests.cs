using System.Collections.Generic;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Labradoratory.Fetch.Authorization;
using Labradoratory.Fetch.Controllers;
using Labradoratory.Fetch.Processors;
using Microsoft.AspNetCore.Authorization;
using Microsoft.AspNetCore.Http;
using Microsoft.AspNetCore.Http.Features;
using Microsoft.AspNetCore.Http.Features.Authentication;
using Microsoft.AspNetCore.Mvc;
using Microsoft.AspNetCore.Mvc.Abstractions;
using Microsoft.AspNetCore.Mvc.Controllers;
using Microsoft.AspNetCore.Routing;
using Moq;
using Moq.Protected;
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

        [Fact]
        public void Ctor_PropertiesSet()
        {
            var mockRepostiory = new Mock<Repository<TestEntity>>(MockBehavior.Strict, new ProcessorPipeline(null));
            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            var mockAuthorizationService = new Mock<IAuthorizationService>(MockBehavior.Strict);
            var subject = new TestController(
                mockRepostiory.Object,
                mockMapper.Object,
                mockAuthorizationService.Object);

            subject.CheckProperties(
                mockRepostiory.Object,
                mockMapper.Object,
                mockAuthorizationService.Object);
        }

        [Fact]
        public async Task GetAll_ForbiddenWhenAuthorizationServiceFails()
        {
            var mockAuthorizationService = new Mock<IAuthorizationService>(MockBehavior.Strict);
            mockAuthorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var mockAuthFeature = new Mock<IHttpAuthenticationFeature>(MockBehavior.Strict);
            mockAuthFeature
                .SetupGet(af => af.User)
                .Returns(new ClaimsPrincipal(new ClaimsIdentity(
                    new List<Claim>()
                    {
                        new Claim(ClaimTypes.Name, "username"),
                        new Claim(ClaimTypes.NameIdentifier, "userId"),
                        new Claim("name", "John Doe"),
                    }, "TestAuthType")));

            var mockFeatureCollection = new Mock<IFeatureCollection>(MockBehavior.Strict);
            mockFeatureCollection
                .SetupGet(fc => fc.Revision)
                .Returns(1);
            mockFeatureCollection
                .Setup(fc => fc.Get<IHttpAuthenticationFeature>())
                .Returns(mockAuthFeature.Object);

            var mockSubject = new Mock<EntityRepositoryController<TestEntity>>(
                MockBehavior.Strict,
                null,
                null,
                mockAuthorizationService.Object);
            mockSubject
                .Protected()
                .Setup("AuthorizationFailed", ItExpr.IsAny<AuthorizationResult>())
                .CallBase();
            mockSubject
                .Setup(a => a.Forbid())
                .CallBase();
            mockSubject.Object.ControllerContext =
                new ControllerContext(
                    new ActionContext(
                        new DefaultHttpContext(mockFeatureCollection.Object),
                        new RouteData(),
                        new ControllerActionDescriptor()));

            var subject = mockSubject.Object;

            var result = await subject.GetAll(CancellationToken.None);
            Assert.True(result is ForbidResult);
        }

        [Fact]
        public async Task GetAll_UnauthorizedWhenNotAuthenticated()
        {
            var mockAuthorizationService = new Mock<IAuthorizationService>(MockBehavior.Strict);
            mockAuthorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var mockAuthFeature = new Mock<IHttpAuthenticationFeature>(MockBehavior.Strict);
            mockAuthFeature
                .SetupGet(af => af.User)
                .Returns(new ClaimsPrincipal(new ClaimsIdentity()));

            var mockFeatureCollection = new Mock<IFeatureCollection>(MockBehavior.Strict);
            mockFeatureCollection
                .SetupGet(fc => fc.Revision)
                .Returns(1);
            mockFeatureCollection
                .Setup(fc => fc.Get<IHttpAuthenticationFeature>())
                .Returns(mockAuthFeature.Object);

            var mockSubject = new Mock<EntityRepositoryController<TestEntity>>(
                MockBehavior.Strict,
                null,
                null,
                mockAuthorizationService.Object);
            mockSubject
                .Protected()
                .Setup("AuthorizationFailed", ItExpr.IsAny<AuthorizationResult>())
                .CallBase();
            mockSubject
                .Setup(a => a.Unauthorized())
                .CallBase();
            mockSubject.Object.ControllerContext =
                new ControllerContext(
                    new ActionContext(
                        new DefaultHttpContext(mockFeatureCollection.Object),
                        new RouteData(),
                        new ControllerActionDescriptor()));

            var subject = mockSubject.Object;

            var result = await subject.GetAll(CancellationToken.None);
            Assert.True(result is UnauthorizedResult);
        }

        [Fact]
        public async Task GetAll_CorrectPolicyBeforeExecuting()
        {
            var mockAuthorizationService = new Mock<IAuthorizationService>(MockBehavior.Strict);
            mockAuthorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Failed());

            var mockSubject = new Mock<EntityRepositoryController<TestEntity>>(
                MockBehavior.Strict,
                null,
                null,
                mockAuthorizationService.Object);
            mockSubject
                .Protected()
                .Setup<IActionResult>("AuthorizationFailed", ItExpr.IsAny<AuthorizationResult>())
                .Returns(new ForbidResult());

            var subject = mockSubject.Object;

            var result = await subject.GetAll(CancellationToken.None);

            mockAuthorizationService.Verify(a => a.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.Is<object>(v => Equals(v, typeof(TestEntity))),
                It.Is<string>(v => Equals(v, EntityAuthorizationPolicies.GetAll))),
                Times.Once());
        }

        public class TestController : EntityRepositoryController<TestEntity>
        {
            public TestController(
                Repository<TestEntity> repository,
                IMapper mapper,
                IAuthorizationService authorizationService) 
                : base(repository, mapper, authorizationService)
            {
            }

            public void CheckProperties(
                Repository<TestEntity> expectedRepository,
                IMapper expectedMapper,
                IAuthorizationService expectedAuthorizationService)
            {
                Assert.Same(expectedRepository, Repository);
                Assert.Same(expectedMapper, Mapper);
                Assert.Same(expectedAuthorizationService, AuthorizationService);
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
