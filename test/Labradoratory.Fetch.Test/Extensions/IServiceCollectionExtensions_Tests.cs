using System;
using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.DependencyInjection;
using Labradoratory.Fetch.Extensions;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Labradoratory.Fetch.Processors.Stages;
using Microsoft.Extensions.DependencyInjection;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.Test.Extensions
{
    public class IServiceCollectionExtensions_Tests
    {
        [Fact]
        public void AddFetch_ReturnsServiceCollection()
        {
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Loose);
            mockServiceCollection
                .Setup(sc => sc.GetEnumerator())
                .Returns(Enumerable.Empty<ServiceDescriptor>().GetEnumerator());
            var expectedServiceCollection = mockServiceCollection.Object;
            var result = expectedServiceCollection.AddFetch();
            Assert.Same(expectedServiceCollection, result);
        }

        [Fact]
        public void AddFetch_AddsDefaultProcessorProvider()
        {
            TestServiceAdded<IProcessorProvider, DefaultProcessorProvider>();
        }

        [Fact]
        public void AddFetch_AddsProcessorPipeline()
        {
            TestServiceAdded<ProcessorPipeline>();
        }

        [Fact]
        public void AddFetchAddingProcessor_Success()
        {
            TestProcessorAdd<EntityAddingPackage<TestEntity>>(sc => sc.AddFetchAddingProcessor<TestEntity, TestProcessor>());
        }

        [Fact]
        public void AddFetchAddedProcessor_Success()
        {
            TestProcessorAdd<EntityAddedPackage<TestEntity>>(sc => sc.AddFetchAddedProcessor<TestEntity, TestProcessor>());
        }

        [Fact]
        public void AddFetchUpdatingProcessor_Success()
        {
            TestProcessorAdd<EntityUpdatingPackage<TestEntity>>(sc => sc.AddFetchUpdatingProcessor<TestEntity, TestProcessor>());
        }

        [Fact]
        public void AddFetchUpdateProcessor_Success()
        {
            TestProcessorAdd<EntityUpdatedPackage<TestEntity>>(sc => sc.AddFetchUpdatedProcessor<TestEntity, TestProcessor>());
        }

        [Fact]
        public void AddFetchDeletingProcessor_Success()
        {
            TestProcessorAdd<EntityDeletingPackage<TestEntity>>(sc => sc.AddFetchDeletingProcessor<TestEntity, TestProcessor>());
        }

        [Fact]
        public void AddFetchDeletedProcessor_Success()
        {
            TestProcessorAdd<EntityDeletedPackage<TestEntity>>(sc => sc.AddFetchDeletedProcessor<TestEntity, TestProcessor>());
        }

        private void TestProcessorAdd<TPackage>(Func<IServiceCollection, IServiceCollection> test)
            where TPackage : DataPackage
        {
            var descriptors = new List<ServiceDescriptor>();
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);
            mockServiceCollection
                .Setup(sc => sc.GetEnumerator())
                .Returns(Enumerable.Empty<ServiceDescriptor>().GetEnumerator());
            mockServiceCollection
                .Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(d => descriptors.Add(d));

            var expectedServiceCollection = mockServiceCollection.Object;
            var result = test(expectedServiceCollection);
            Assert.Same(expectedServiceCollection, result);

            mockServiceCollection.Verify(sc => sc.Add(
                It.Is<ServiceDescriptor>(v => v.ServiceType == typeof(IProcessor<TPackage>)
                && v.ImplementationType == typeof(TestProcessor))),
                Times.Once());
        }

        private void TestServiceAdded<TServiceAndImplementation>()
        {
            TestServiceAdded<TServiceAndImplementation, TServiceAndImplementation>();
        }

        private void TestServiceAdded<TService, TImplementation>()
        {
            var descriptors = new List<ServiceDescriptor>();
            var mockServiceCollection = new Mock<IServiceCollection>(MockBehavior.Strict);
            mockServiceCollection
                .Setup(sc => sc.GetEnumerator())
                .Returns(Enumerable.Empty<ServiceDescriptor>().GetEnumerator());
            mockServiceCollection
                .Setup(sc => sc.Add(It.IsAny<ServiceDescriptor>()))
                .Callback<ServiceDescriptor>(d => descriptors.Add(d));

            var expectedServiceCollection = mockServiceCollection.Object;
            var result = expectedServiceCollection.AddFetch();
            Assert.Same(expectedServiceCollection, result);
            Assert.Single(descriptors, d => d.ServiceType == typeof(TService));
            var descriptor = descriptors.Single(d => d.ServiceType == typeof(TService));
            Assert.Equal(typeof(TService), descriptor.ServiceType);
            Assert.Equal(typeof(TImplementation), descriptor.ImplementationType);
            Assert.Equal(ServiceLifetime.Scoped, descriptor.Lifetime);
        }

        public class TestEntity : Entity
        {
            public override object[] DecodeKeys(string encodedKeys)
            {
                throw new System.NotImplementedException();
            }

            public override string EncodeKeys()
            {
                throw new System.NotImplementedException();
            }

            public override object[] GetKeys()
            {
                throw new System.NotImplementedException();
            }
        }

        public class TestProcessor :
            IProcessor<EntityAddedPackage<TestEntity>>,
            IProcessor<EntityAddingPackage<TestEntity>>,
            IProcessor<EntityUpdatedPackage<TestEntity>>,
            IProcessor<EntityUpdatingPackage<TestEntity>>,
            IProcessor<EntityDeletedPackage<TestEntity>>,
            IProcessor<EntityDeletingPackage<TestEntity>>
        {
            public IStage Stage => throw new System.NotImplementedException();

            public Task ProcessAsync(EntityAddedPackage<TestEntity> package, CancellationToken cancellationToken = default)
            {
                throw new System.NotImplementedException();
            }

            public Task ProcessAsync(EntityAddingPackage<TestEntity> package, CancellationToken cancellationToken = default)
            {
                throw new System.NotImplementedException();
            }

            public Task ProcessAsync(EntityUpdatedPackage<TestEntity> package, CancellationToken cancellationToken = default)
            {
                throw new System.NotImplementedException();
            }

            public Task ProcessAsync(EntityUpdatingPackage<TestEntity> package, CancellationToken cancellationToken = default)
            {
                throw new System.NotImplementedException();
            }

            public Task ProcessAsync(EntityDeletedPackage<TestEntity> package, CancellationToken cancellationToken = default)
            {
                throw new System.NotImplementedException();
            }

            public Task ProcessAsync(EntityDeletingPackage<TestEntity> package, CancellationToken cancellationToken = default)
            {
                throw new System.NotImplementedException();
            }
        }

    }
}
