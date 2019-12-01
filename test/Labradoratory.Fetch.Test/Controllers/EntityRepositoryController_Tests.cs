using System;
using System.Collections.Generic;
using System.Linq;
using System.Security.Claims;
using System.Threading;
using System.Threading.Tasks;
using AutoMapper;
using Labradoratory.Fetch.Authorization;
using Labradoratory.Fetch.Controllers;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
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
            var expectedAuthorizationResult = AuthorizationResult.Failed();

            var mockAuthorizationService = new Mock<IAuthorizationService>(MockBehavior.Strict);
            mockAuthorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(expectedAuthorizationResult);

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

            mockSubject.Protected().Verify("AuthorizationFailed",
                Times.Once(),
                ItExpr.Is<AuthorizationResult>(v => ReferenceEquals(v, expectedAuthorizationResult)));
        }

        [Fact]
        public async Task GetAll_UnauthorizedWhenNotAuthenticated()
        {
            var expectedAuthorizationResult = AuthorizationResult.Failed();

            var mockAuthorizationService = new Mock<IAuthorizationService>(MockBehavior.Strict);
            mockAuthorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(expectedAuthorizationResult);

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

            mockSubject.Protected().Verify("AuthorizationFailed",
                Times.Once(),
                ItExpr.Is<AuthorizationResult>(v => ReferenceEquals(v, expectedAuthorizationResult)));
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

        [Fact]
        public async Task GetAll_Success()
        {
            var mockAuthorizationService = new Mock<IAuthorizationService>(MockBehavior.Strict);
            mockAuthorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());

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

            var expectedList = new List<TestEntity>
            {
                new TestEntity { Id = 123456 }
            };

            var mockResolver = new Mock<IAsyncQueryResolver<TestEntity>>(MockBehavior.Strict);
            mockResolver
                .Setup(r => r.ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            var mockRepository = new Mock<Repository<TestEntity>>(MockBehavior.Strict, new ProcessorPipeline(null));
            mockRepository
                .Setup(r => r.GetAsyncQueryResolver(It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>()))
                .Returns(mockResolver.Object)
                // This is to check that the FilterGetAll method was passed in.  
                // We just need to call it so that the verify check later passes.
                .Callback<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(f => f(null));

            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            mockMapper
                .Setup(m => m.Map<IEnumerable<TestEntity>>(It.IsAny<object>()))
                .Returns<object>(value => value as IEnumerable<TestEntity>);

            var mockSubject = new Mock<EntityRepositoryController<TestEntity>>(
                MockBehavior.Strict,
                mockRepository.Object,
                mockMapper.Object,
                mockAuthorizationService.Object);
            mockSubject
                .Protected()
                .Setup<IQueryable<TestEntity>>("FilterGetAll", ItExpr.IsAny<IQueryable<TestEntity>>())
                .CallBase();
            mockSubject
                .Setup(c => c.Ok(It.IsAny<object>()))
                .CallBase();
            mockSubject.Object.ControllerContext =
                new ControllerContext(
                    new ActionContext(
                        new DefaultHttpContext(mockFeatureCollection.Object),
                        new RouteData(),
                        new ControllerActionDescriptor()));

            var subject = mockSubject.Object;

            var result = await subject.GetAll(CancellationToken.None);
            Assert.True(result is OkObjectResult);
            var okResult = result as OkObjectResult;
            Assert.Equal(expectedList, okResult.Value);

            mockRepository.Verify(r => r.GetAsyncQueryResolver(
                It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>()),
                Times.Once());

            mockResolver.Verify(r => r.ToListAsync(
                It.IsAny<CancellationToken>()),
                Times.Once());

            mockAuthorizationService.Verify(a => a.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.Is<object>(v => Equals(v, expectedList)),
                It.Is<string>(v => Equals(v, EntityAuthorizationPolicies.GetSome))),
                Times.Once());

            mockSubject.Protected().Verify("FilterGetAll",
                Times.Once(),
                ItExpr.IsAny<IQueryable<TestEntity>>());
        }

        [Fact]
        public async Task GetAll_CallsAuthorizationFailedIfGetSomeCheckFails()
        {
            var expectedAuthorizationResult = AuthorizationResult.Failed();

            var mockAuthorizationService = new Mock<IAuthorizationService>(MockBehavior.Strict);
            mockAuthorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.Is<string>(v => v == EntityAuthorizationPolicies.GetAll)))
                .ReturnsAsync(AuthorizationResult.Success());
            mockAuthorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.Is<string>(v => v == EntityAuthorizationPolicies.GetSome)))
                .ReturnsAsync(expectedAuthorizationResult);

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

            var expectedList = new List<TestEntity>
            {
                new TestEntity { Id = 123456 }
            };

            var mockResolver = new Mock<IAsyncQueryResolver<TestEntity>>(MockBehavior.Strict);
            mockResolver
                .Setup(r => r.ToListAsync(It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedList);

            var mockRepository = new Mock<Repository<TestEntity>>(MockBehavior.Strict, new ProcessorPipeline(null));
            mockRepository
                .Setup(r => r.GetAsyncQueryResolver(It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>()))
                .Returns(mockResolver.Object);

            var mockSubject = new Mock<EntityRepositoryController<TestEntity>>(
                MockBehavior.Strict,
                mockRepository.Object,
                null,
                mockAuthorizationService.Object);
            mockSubject
                .Protected()
                .Setup<IActionResult>("AuthorizationFailed", ItExpr.IsAny<AuthorizationResult>())
                .Returns(new UnauthorizedResult());

            // Would be great to test FilterGetAll, but I'm not sure how to do it...

            mockSubject
                .Setup(c => c.Ok(It.IsAny<object>()))
                .CallBase();
            mockSubject.Object.ControllerContext =
                new ControllerContext(
                    new ActionContext(
                        new DefaultHttpContext(mockFeatureCollection.Object),
                        new RouteData(),
                        new ControllerActionDescriptor()));

            var subject = mockSubject.Object;

            var result = await subject.GetAll(CancellationToken.None);

            mockSubject.Protected().Verify("AuthorizationFailed",
                Times.Once(), 
                ItExpr.Is<AuthorizationResult>(v => ReferenceEquals(v, expectedAuthorizationResult)));

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

        [Fact]
        public async Task GetByKey_ForbidResultWhenNotAuthorized()
        {
            var expectedAuthorizationResult = AuthorizationResult.Failed();
            var mockAuthorizationService = new Mock<IAuthorizationService>(MockBehavior.Strict);
            mockAuthorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(expectedAuthorizationResult);

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

            var expectedEntity = new TestEntity { Id = 123456 };
            var expectedEncodedKeys = expectedEntity.EncodeKeys();

            var mockRepository = new Mock<Repository<TestEntity>>(MockBehavior.Strict, new ProcessorPipeline(null));
            mockRepository
                .Setup(r => r.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedEntity);

            var mockSubject = new Mock<EntityRepositoryController<TestEntity>>(
                MockBehavior.Strict,
                mockRepository.Object,
                null,
                mockAuthorizationService.Object);
            mockSubject
                .Protected()
                .Setup("AuthorizationFailed", ItExpr.IsAny<AuthorizationResult>())
                .CallBase();
            mockSubject
                .Setup(c => c.Forbid())
                .CallBase();
            mockSubject.Object.ControllerContext =
                new ControllerContext(
                    new ActionContext(
                        new DefaultHttpContext(mockFeatureCollection.Object),
                        new RouteData(),
                        new ControllerActionDescriptor()));

            var subject = mockSubject.Object;

            var result = await subject.GetByKeys(expectedEncodedKeys, CancellationToken.None);
            Assert.True(result is ForbidResult);

            mockRepository.Verify(r => r.FindAsync(
                It.Is<object[]>(v => Equals(v[0], expectedEntity.Id)),
                It.IsAny<CancellationToken>()),
                Times.Once());

            mockAuthorizationService.Verify(a => a.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.Is<object>(v => Equals(v, expectedEntity)),
                It.Is<string>(v => Equals(v, EntityAuthorizationPolicies.GetOne))),
                Times.Once());

            mockSubject.Protected().Verify("AuthorizationFailed",
                Times.Once(),
                ItExpr.Is<AuthorizationResult>(v => ReferenceEquals(v, expectedAuthorizationResult)));
        }

        [Fact]
        public async Task GetByKey_UnauthorizedResultWhenNotAuthenticated()
        {
            var expectedAuthorizationResult = AuthorizationResult.Failed();

            var mockAuthorizationService = new Mock<IAuthorizationService>(MockBehavior.Strict);
            mockAuthorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(expectedAuthorizationResult);

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

            var expectedEntity = new TestEntity { Id = 123456 };
            var expectedEncodedKeys = expectedEntity.EncodeKeys();

            var mockRepository = new Mock<Repository<TestEntity>>(MockBehavior.Strict, new ProcessorPipeline(null));
            mockRepository
                .Setup(r => r.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedEntity);

            var mockSubject = new Mock<EntityRepositoryController<TestEntity>>(
                MockBehavior.Strict,
                mockRepository.Object,
                null,
                mockAuthorizationService.Object);
            mockSubject
                .Protected()
                .Setup("AuthorizationFailed", ItExpr.IsAny<AuthorizationResult>())
                .CallBase();
            mockSubject
                .Setup(c => c.Unauthorized())
                .CallBase();
            mockSubject.Object.ControllerContext =
                new ControllerContext(
                    new ActionContext(
                        new DefaultHttpContext(mockFeatureCollection.Object),
                        new RouteData(),
                        new ControllerActionDescriptor()));

            var subject = mockSubject.Object;

            var result = await subject.GetByKeys(expectedEncodedKeys, CancellationToken.None);
            Assert.True(result is UnauthorizedResult);

            mockAuthorizationService.Verify(a => a.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.Is<object>(v => Equals(v, expectedEntity)),
                It.Is<string>(v => Equals(v, EntityAuthorizationPolicies.GetOne))),
                Times.Once());

            mockSubject.Protected().Verify("AuthorizationFailed",
                Times.Once(),
                ItExpr.Is<AuthorizationResult>(v => ReferenceEquals(v, expectedAuthorizationResult)));
        }

        [Fact]
        public async Task GetByKey_Success()
        {
            var mockAuthorizationService = new Mock<IAuthorizationService>(MockBehavior.Strict);
            mockAuthorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());

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

            var expectedEntity = new TestEntity { Id = 123456 };
            var expectedEncodedKeys = expectedEntity.EncodeKeys();

            var mockRepository = new Mock<Repository<TestEntity>>(MockBehavior.Strict, new ProcessorPipeline(null));
            mockRepository
                .Setup(r => r.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(expectedEntity);

            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            mockMapper
                .Setup(m => m.Map<TestEntity>(It.IsAny<object>()))
                .Returns<object>(value => value as TestEntity);

            var mockSubject = new Mock<EntityRepositoryController<TestEntity>>(
                MockBehavior.Strict,
                mockRepository.Object,
                mockMapper.Object,
                mockAuthorizationService.Object);
            mockSubject
                .Setup(c => c.Ok(It.IsAny<object>()))
                .CallBase();
            mockSubject.Object.ControllerContext =
                new ControllerContext(
                    new ActionContext(
                        new DefaultHttpContext(mockFeatureCollection.Object),
                        new RouteData(),
                        new ControllerActionDescriptor()));

            var subject = mockSubject.Object;

            var result = await subject.GetByKeys(expectedEncodedKeys, CancellationToken.None);
            Assert.True(result is OkObjectResult);
            var okResult = result as OkObjectResult;
            Assert.Equal(expectedEntity, okResult.Value);

            mockRepository.Verify(r => r.FindAsync(
                It.Is<object[]>(v => Equals(v[0], expectedEntity.Id)),
                It.IsAny<CancellationToken>()),
                Times.Once());

            mockAuthorizationService.Verify(a => a.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.Is<object>(v => Equals(v, expectedEntity)),
                It.Is<string>(v => Equals(v, EntityAuthorizationPolicies.GetOne))),
                Times.Once());
        }

        [Fact]
        public async Task GetByKey_NotFoundResultWhenNotFound()
        {
            var expectedEntity = new TestEntity { Id = 123456 };
            var expectedEncodedKeys = expectedEntity.EncodeKeys();

            var mockRepository = new Mock<Repository<TestEntity>>(MockBehavior.Strict, new ProcessorPipeline(null));
            mockRepository
                .Setup(r => r.FindAsync(It.IsAny<object[]>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync((TestEntity)null);

            var mockSubject = new Mock<EntityRepositoryController<TestEntity>>(
                MockBehavior.Strict,
                mockRepository.Object,
                null,
                null);
            mockSubject
                .Setup(c => c.NotFound())
                .CallBase();

            var subject = mockSubject.Object;

            var result = await subject.GetByKeys(expectedEncodedKeys, CancellationToken.None);
            Assert.True(result is NotFoundResult);

            mockRepository.Verify(r => r.FindAsync(
                It.Is<object[]>(v => Equals(v[0], expectedEntity.Id)),
                It.IsAny<CancellationToken>()),
                Times.Once());
        }

        [Fact]
        public async Task Add_ForbidResultWhenNotAuthorized()
        {
            var expectedAuthorizationResult = AuthorizationResult.Failed();
            var mockAuthorizationService = new Mock<IAuthorizationService>(MockBehavior.Strict);
            mockAuthorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(expectedAuthorizationResult);

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

            var expectedEntity = new TestEntity();

            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            mockMapper
                .Setup(m => m.Map<TestEntity>(It.IsAny<object>()))
                .Returns<object>(v => v as TestEntity);

            var mockSubject = new Mock<EntityRepositoryController<TestEntity>>(
                MockBehavior.Strict,
                null,
                mockMapper.Object,
                mockAuthorizationService.Object);
            mockSubject
                .Protected()
                .Setup("AuthorizationFailed", ItExpr.IsAny<AuthorizationResult>())
                .CallBase();
            mockSubject
                .Setup(c => c.Forbid())
                .CallBase();
            mockSubject.Object.ControllerContext =
                new ControllerContext(
                    new ActionContext(
                        new DefaultHttpContext(mockFeatureCollection.Object),
                        new RouteData(),
                        new ControllerActionDescriptor()));

            var subject = mockSubject.Object;

            var result = await subject.Add(expectedEntity, CancellationToken.None);
            Assert.True(result is ForbidResult);

            mockAuthorizationService.Verify(a => a.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.Is<object>(v => Equals(v, expectedEntity)),
                It.Is<string>(v => Equals(v, EntityAuthorizationPolicies.Add))),
                Times.Once());

            mockSubject.Protected().Verify("AuthorizationFailed",
                Times.Once(),
                ItExpr.Is<AuthorizationResult>(v => ReferenceEquals(v, expectedAuthorizationResult)));
        }

        [Fact]
        public async Task Add_UnauthorizedResultWhenNotAuthenticated()
        {
            var expectedAuthorizationResult = AuthorizationResult.Failed();
            var mockAuthorizationService = new Mock<IAuthorizationService>(MockBehavior.Strict);
            mockAuthorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(expectedAuthorizationResult);

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

            var expectedEntity = new TestEntity();

            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            mockMapper
                .Setup(m => m.Map<TestEntity>(It.IsAny<object>()))
                .Returns<object>(v => v as TestEntity);

            var mockSubject = new Mock<EntityRepositoryController<TestEntity>>(
                MockBehavior.Strict,
                null,
                mockMapper.Object,
                mockAuthorizationService.Object);
            mockSubject
                .Protected()
                .Setup("AuthorizationFailed", ItExpr.IsAny<AuthorizationResult>())
                .CallBase();
            mockSubject
                .Setup(c => c.Unauthorized())
                .CallBase();
            mockSubject.Object.ControllerContext =
                new ControllerContext(
                    new ActionContext(
                        new DefaultHttpContext(mockFeatureCollection.Object),
                        new RouteData(),
                        new ControllerActionDescriptor()));

            var subject = mockSubject.Object;

            var result = await subject.Add(expectedEntity, CancellationToken.None);
            Assert.True(result is UnauthorizedResult);

            mockAuthorizationService.Verify(a => a.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.Is<object>(v => Equals(v, expectedEntity)),
                It.Is<string>(v => Equals(v, EntityAuthorizationPolicies.Add))),
                Times.Once());

            mockSubject.Protected().Verify("AuthorizationFailed",
                Times.Once(),
                ItExpr.Is<AuthorizationResult>(v => ReferenceEquals(v, expectedAuthorizationResult)));
        }

        [Fact]
        public async Task Add_Success()
        {
            var mockAuthorizationService = new Mock<IAuthorizationService>(MockBehavior.Strict);
            mockAuthorizationService
                .Setup(a => a.AuthorizeAsync(It.IsAny<ClaimsPrincipal>(), It.IsAny<object>(), It.IsAny<string>()))
                .ReturnsAsync(AuthorizationResult.Success());

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

            var expectedViewEntity = new TestEntity();
            var expectedModelEntity = new TestEntity();

            var mockMapper = new Mock<IMapper>(MockBehavior.Strict);
            mockMapper
                .Setup(m => m.Map<TestEntity>(It.Is<object>(v => ReferenceEquals(v, expectedViewEntity))))
                .Returns(expectedModelEntity);
            mockMapper
                .Setup(m => m.Map<TestEntity>(It.Is<object>(v => ReferenceEquals(v, expectedModelEntity))))
                .Returns(expectedViewEntity);

            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityAddingPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityAddingPackage<TestEntity>>>());
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityAddedPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityAddedPackage<TestEntity>>>());

            var mockRespository = new Mock<Repository<TestEntity>>(MockBehavior.Strict, new ProcessorPipeline(mockProcessorProvider.Object));
            mockRespository
                .Protected()
                .Setup<Task>("ExecuteAddAsync", ItExpr.IsAny<TestEntity>(), ItExpr.IsAny<CancellationToken>())
                .Returns(Task.CompletedTask);

            var mockSubject = new Mock<EntityRepositoryController<TestEntity>>(
                MockBehavior.Strict,
                mockRespository.Object,
                mockMapper.Object,
                mockAuthorizationService.Object);
            mockSubject
                .Setup(c => c.Ok(It.IsAny<object>()))
                .CallBase();
            mockSubject.Object.ControllerContext =
                new ControllerContext(
                    new ActionContext(
                        new DefaultHttpContext(mockFeatureCollection.Object),
                        new RouteData(),
                        new ControllerActionDescriptor()));

            var subject = mockSubject.Object;

            var result = await subject.Add(expectedViewEntity, CancellationToken.None);
            Assert.True(result is OkObjectResult);
            var okResult = result as OkObjectResult;
            Assert.Same(expectedViewEntity, okResult.Value);

            mockAuthorizationService.Verify(a => a.AuthorizeAsync(
                It.IsAny<ClaimsPrincipal>(),
                It.Is<object>(v => Equals(v, expectedModelEntity)),
                It.Is<string>(v => Equals(v, EntityAuthorizationPolicies.Add))),
                Times.Once());

            mockMapper.Verify(m => m.Map<TestEntity>(
                It.Is<object>(v => ReferenceEquals(v, expectedViewEntity))),
                Times.Once());
            mockMapper.Verify(m => m.Map<TestEntity>(
                It.Is<object>(v => ReferenceEquals(v, expectedModelEntity))),
                Times.Once());

            mockProcessorProvider.Verify(p => p.GetProcessors<EntityAddingPackage<TestEntity>>(),
                Times.Once);
            mockProcessorProvider.Verify(p => p.GetProcessors<EntityAddedPackage<TestEntity>>(),
                Times.Once());
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
