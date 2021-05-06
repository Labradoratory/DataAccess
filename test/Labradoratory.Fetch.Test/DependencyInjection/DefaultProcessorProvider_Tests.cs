using System;
using System.Collections.Generic;
using Labradoratory.Fetch.DependencyInjection;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.Test.DependencyInjection
{
    public class DefaultProcessorProvider_Tests
    {
        [Fact]
        public void GetProcessors_Success()
        {
            var mockServiceProvider = new Mock<System.IServiceProvider>(MockBehavior.Strict);
            mockServiceProvider
                .Setup(sp => sp.GetService(It.IsAny<Type>()))
                .Returns(new List<IProcessor<TestDataPackage>>());

            var subject = new DefaultProcessorProvider(mockServiceProvider.Object);
            subject.GetProcessors<TestDataPackage>();

            mockServiceProvider.Verify(sp => sp.GetService(
                It.Is<Type>(v => Equals(v, typeof(IEnumerable<IProcessor<TestDataPackage>>)))),
                Times.Once());
        }

        public class TestDataPackage : DataPackage
        { }
    }
}
