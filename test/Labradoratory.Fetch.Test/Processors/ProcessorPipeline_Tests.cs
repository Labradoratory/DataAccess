using System.Collections.Generic;
using System.Linq;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Labradoratory.Fetch.Test.Processors.DataPackages;
using Moq;
using Xunit;

namespace Labradoratory.Fetch.Test.Processors
{
    public class ProcessorPipeline_Tests
    {
        [Fact]
        public async Task ProcessAsync_UsesProvider()
        {
            var expectedProcessors = new List<IProcessor<TestDataPackage>>();
            var mockProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);
            mockProvider.Setup(p => p.GetProcessors<TestDataPackage>()).Returns(expectedProcessors);
            
            var subject = new ProcessorPipeline(mockProvider.Object);
            await subject.ProcessAsync(new TestDataPackage());
            
            mockProvider.Verify(p => p.GetProcessors<TestDataPackage>(), Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_ExecutesProvidedProcessors()
        {
            var mock2Called = false;

            var mockProcessor1 = new Mock<IProcessor<TestDataPackage>>(MockBehavior.Strict);
            mockProcessor1.SetupGet(p => p.Priority).Returns(0);
            mockProcessor1
                .Setup(p => p.ProcessAsync(It.IsAny<TestDataPackage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback(() => Assert.True(mock2Called));

            var mockProcessor2 = new Mock<IProcessor<TestDataPackage>>(MockBehavior.Strict);
            mockProcessor2.SetupGet(p => p.Priority).Returns(100);
            mockProcessor2
                .Setup(p => p.ProcessAsync(It.IsAny<TestDataPackage>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback(() => mock2Called = true);

            var expectedProcessors = new List<IProcessor<TestDataPackage>>
            {
                mockProcessor1.Object,
                mockProcessor2.Object
            };
            var mockProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);
            mockProvider.Setup(p => p.GetProcessors<TestDataPackage>()).Returns(expectedProcessors);

            var expectedData = new TestDataPackage();

            var subject = new ProcessorPipeline(mockProvider.Object);
            await subject.ProcessAsync(expectedData);

            mockProcessor1.Verify(p => p.ProcessAsync(
                It.Is<TestDataPackage>(v => ReferenceEquals(v, expectedData)),
                It.IsAny<CancellationToken>()),
                Times.Once);

            mockProcessor2.Verify(p => p.ProcessAsync(
                It.Is<TestDataPackage>(v => ReferenceEquals(v, expectedData)),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        [Fact]
        public async Task ProcessAsync_PreviousChainWorks()
        {
            var expectedProcessors = new List<object>();
            var mockProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);
            mockProvider.Setup(p => p.GetProcessors<TestDataPackage>()).Returns(expectedProcessors.OfType<IProcessor<TestDataPackage>>());
            mockProvider.Setup(p => p.GetProcessors<TestDataPackage2>()).Returns(expectedProcessors.OfType<IProcessor<TestDataPackage2>>());

            var expectedData = new TestDataPackage();
            var expectedData2 = new TestDataPackage2();

            var subject = new ProcessorPipeline(mockProvider.Object);

            var mockProcessor1 = new Mock<IProcessor<TestDataPackage>>(MockBehavior.Strict);
            mockProcessor1.SetupGet(p => p.Priority).Returns(0);
            mockProcessor1
                .Setup(p => p.ProcessAsync(It.IsAny<TestDataPackage>(), It.IsAny<CancellationToken>()))
                .Callback<TestDataPackage, CancellationToken>((data, token) => Assert.Null(data.Previous))
                .Returns(Task.CompletedTask);

            var mockProcessor2 = new Mock<IProcessor<TestDataPackage>>(MockBehavior.Strict);
            mockProcessor2.SetupGet(p => p.Priority).Returns(100);
            mockProcessor2
                .Setup(p => p.ProcessAsync(It.IsAny<TestDataPackage>(), It.IsAny<CancellationToken>()))
                .Callback<TestDataPackage, CancellationToken>(async (data, token) =>
                {
                    Assert.Null(data.Previous);
                    await subject.ProcessAsync(expectedData2);
                })
                .Returns(Task.CompletedTask);

            var mockProcessor3 = new Mock<IProcessor<TestDataPackage2>>(MockBehavior.Strict);
            mockProcessor3.SetupGet(p => p.Priority).Returns(0);
            mockProcessor3
                .Setup(p => p.ProcessAsync(It.IsAny<TestDataPackage2>(), It.IsAny<CancellationToken>()))
                .Callback<TestDataPackage2, CancellationToken>((data, token) => Assert.Same(expectedData, data.Previous))
                .Returns(Task.CompletedTask);
            
            expectedProcessors.Add(mockProcessor1.Object);
            expectedProcessors.Add(mockProcessor2.Object);
            expectedProcessors.Add(mockProcessor3.Object);
            
            await subject.ProcessAsync(expectedData);

            mockProcessor1.Verify(p => p.ProcessAsync(
                It.Is<TestDataPackage>(v => ReferenceEquals(v, expectedData)),
                It.IsAny<CancellationToken>()),
                Times.Once);

            mockProcessor2.Verify(p => p.ProcessAsync(
                It.Is<TestDataPackage>(v => ReferenceEquals(v, expectedData)),
                It.IsAny<CancellationToken>()),
                Times.Once);

            mockProcessor3.Verify(p => p.ProcessAsync(
                It.Is<TestDataPackage2>(v => ReferenceEquals(v, expectedData2)),
                It.IsAny<CancellationToken>()),
                Times.Once);
        }

        public class TestDataPackage : DataPackage
        {}

        public class TestDataPackage2 : DataPackage
        { }
    }
}