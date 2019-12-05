using System.Collections.Generic;
using System.Linq;
using Labradoratory.Fetch.DependencyInjection;
using Labradoratory.Fetch.Extensions;
using Labradoratory.Fetch.Processors;
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
            Assert.Equal(ServiceLifetime.Singleton, descriptor.Lifetime);
        }
    }
}
