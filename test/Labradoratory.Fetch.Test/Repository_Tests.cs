using System;
using System.Collections.Generic;
using System.Linq;
using System.Linq.Expressions;
using System.Threading;
using System.Threading.Tasks;
using Labradoratory.Fetch.ChangeTracking;
using Labradoratory.Fetch.Processors;
using Labradoratory.Fetch.Processors.DataPackages;
using Labradoratory.Fetch.Processors.Stages;
using Moq;
using Moq.Protected;
using Xunit;

namespace Labradoratory.Fetch.Test
{
    public class Repository_Tests
    {
        [Fact]
        public void Ctor_()
        {
            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);
            var expectedPipeline = new ProcessorPipeline(mockProcessorProvider.Object);
            var subject = new TestRepository(expectedPipeline);
            Assert.Same(expectedPipeline, subject.TestGetInternalPipeline());
        }

        [Fact]
        public async Task AddAsync_PipelineExecuted()
        {
            var addingCalled = false;
            var addedCalled = false;
            var executeAddCalled = false;

            var mockProcessorAdding = new Mock<IProcessor<EntityAddingPackage<TestEntity>>>(MockBehavior.Strict);
            mockProcessorAdding.SetupGet(p => p.Stage).Returns(new NumericPriorityStage(0));
            mockProcessorAdding
                .Setup(p => p.ProcessAsync(It.IsAny<EntityAddingPackage<TestEntity>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback(() =>
                {
                    Assert.False(addedCalled);
                    Assert.False(executeAddCalled);
                    addingCalled = true;
                });

            var mockProcessorAdded = new Mock<IProcessor<EntityAddedPackage<TestEntity>>>(MockBehavior.Strict);
            mockProcessorAdded.SetupGet(p => p.Stage).Returns(new NumericPriorityStage(0));
            mockProcessorAdded
                .Setup(p => p.ProcessAsync(It.IsAny<EntityAddedPackage<TestEntity>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback(() =>
                {
                    Assert.True(addingCalled);
                    Assert.True(executeAddCalled);
                    addedCalled = true;
                });

            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityAddingPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityAddingPackage<TestEntity>>> { mockProcessorAdding.Object });
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityAddedPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityAddedPackage<TestEntity>>> { mockProcessorAdded.Object });

            var expectedPipeline = new ProcessorPipeline(mockProcessorProvider.Object);
            var mockSubject = new Mock<Repository<TestEntity>>(MockBehavior.Strict, expectedPipeline);
            mockSubject
                .Protected()
                .Setup<Task>("ExecuteAddAsync", ItExpr.IsAny<TestEntity>(), It.IsAny<CancellationToken>())
                .Returns(Task.CompletedTask)
                .Callback(() =>
                {
                    Assert.True(addingCalled);
                    Assert.False(addedCalled);
                    executeAddCalled = true;
                });

            var expectedEntity = new TestEntity();

            await mockSubject.Object.AddAsync(expectedEntity, CancellationToken.None);

            mockProcessorProvider.Verify(p => p.GetProcessors<EntityAddingPackage<TestEntity>>(), Times.Once);
            mockProcessorProvider.Verify(p => p.GetProcessors<EntityAddedPackage<TestEntity>>(), Times.Once);

            mockProcessorAdding.Verify(p => p.ProcessAsync(
                It.Is<EntityAddingPackage<TestEntity>>(v => ReferenceEquals(v.Entity, expectedEntity)),
                It.IsAny<CancellationToken>()),
                Times.Once());

            mockProcessorAdded.Verify(p => p.ProcessAsync(
                It.Is<EntityAddedPackage<TestEntity>>(v => ReferenceEquals(v.Entity, expectedEntity)),
                It.IsAny<CancellationToken>()),
                Times.Once());

            mockSubject
                .Protected()
                .Verify("ExecuteAddAsync",
                    Times.Once(),
                    ItExpr.Is<TestEntity>(v => ReferenceEquals(v, expectedEntity)),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task UpdateAsync_DoesNothingWhenNoChanges()
        {
            var expectedPipeline = new ProcessorPipeline(null);
            var mockSubject = new Mock<Repository<TestEntity>>(MockBehavior.Strict, expectedPipeline);

            var expectedEntity = new TestEntity();

            await mockSubject.Object.UpdateAsync(expectedEntity, CancellationToken.None);
        }

        [Fact]
        public async Task UpdateAsync_PipelineExecuted()
        {
            var updatingCalled = false;
            var updatedCalled = false;
            var executeUpdateCalled = false;

            var mockProcessorUpdating = new Mock<IProcessor<EntityUpdatingPackage<TestEntity>>>(MockBehavior.Strict);
            mockProcessorUpdating.SetupGet(p => p.Stage).Returns(new NumericPriorityStage(0));
            mockProcessorUpdating
                .Setup(p => p.ProcessAsync(It.IsAny<EntityUpdatingPackage<TestEntity>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback(() =>
                {
                    Assert.False(updatedCalled);
                    Assert.False(executeUpdateCalled);
                    updatingCalled = true;
                });

            var mockProcessorUpdated = new Mock<IProcessor<EntityUpdatedPackage<TestEntity>>>(MockBehavior.Strict);
            mockProcessorUpdated.SetupGet(p => p.Stage).Returns(new NumericPriorityStage(0));
            mockProcessorUpdated
                .Setup(p => p.ProcessAsync(It.IsAny<EntityUpdatedPackage<TestEntity>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback(() =>
                {
                    Assert.True(updatingCalled);
                    Assert.True(executeUpdateCalled);
                    updatedCalled = true;
                });

            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityUpdatingPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityUpdatingPackage<TestEntity>>> { mockProcessorUpdating.Object });
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityUpdatedPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityUpdatedPackage<TestEntity>>> { mockProcessorUpdated.Object });

            ChangeSet expectedChangeSet = null;

            var expectedPipeline = new ProcessorPipeline(mockProcessorProvider.Object);
            var mockSubject = new Mock<Repository<TestEntity>>(MockBehavior.Strict, expectedPipeline);
            mockSubject
                .Protected()
                .Setup<Task<ChangeSet>>("ExecuteUpdateAsync", ItExpr.IsAny<TestEntity>(), ItExpr.IsAny<ChangeSet>(), It.IsAny<CancellationToken>())
                .Callback<TestEntity, ChangeSet, CancellationToken>((e, cs, ct) =>
                {
                    expectedChangeSet = cs;
                    Assert.True(updatingCalled);
                    Assert.False(updatedCalled);
                    executeUpdateCalled = true;
                })
                .Returns(Task.FromResult(expectedChangeSet));

            var expectedEntity = ChangeTrackingObject.CreateTrackable<TestEntity>();
            expectedEntity.StringValue = "NewValue";

            var result = await mockSubject.Object.UpdateAsync(expectedEntity, CancellationToken.None);

            mockProcessorProvider.Verify(p => p.GetProcessors<EntityUpdatingPackage<TestEntity>>(), Times.Once);
            mockProcessorProvider.Verify(p => p.GetProcessors<EntityUpdatedPackage<TestEntity>>(), Times.Once);

            mockProcessorUpdating.Verify(p => p.ProcessAsync(
                It.Is<EntityUpdatingPackage<TestEntity>>(v => ReferenceEquals(v.Entity, expectedEntity)),
                It.IsAny<CancellationToken>()),
                Times.Once());

            mockProcessorUpdated.Verify(p => p.ProcessAsync(
                It.Is<EntityUpdatedPackage<TestEntity>>(v => ReferenceEquals(v.Entity, expectedEntity)),
                It.IsAny<CancellationToken>()),
                Times.Once());

            mockSubject
                .Protected()
                .Verify("ExecuteUpdateAsync",
                    Times.Once(),
                    ItExpr.Is<TestEntity>(v => ReferenceEquals(v, expectedEntity)),
                    ItExpr.Is<ChangeSet>(v => ReferenceEquals(v, expectedChangeSet)),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public async Task DeleteAsync_PipelineExecuted()
        {
            var deletingCalled = false;
            var deletedCalled = false;
            var executeDeleteCalled = false;

            var mockProcessorDeleting = new Mock<IProcessor<EntityDeletingPackage<TestEntity>>>(MockBehavior.Strict);
            mockProcessorDeleting.SetupGet(p => p.Stage).Returns(new NumericPriorityStage(0));
            mockProcessorDeleting
                .Setup(p => p.ProcessAsync(It.IsAny<EntityDeletingPackage<TestEntity>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback(() =>
                {
                    Assert.False(deletedCalled);
                    Assert.False(executeDeleteCalled);
                    deletingCalled = true;
                });

            var mockProcessorDeleted = new Mock<IProcessor<EntityDeletedPackage<TestEntity>>>(MockBehavior.Strict);
            mockProcessorDeleted.SetupGet(p => p.Stage).Returns(new NumericPriorityStage(0));
            mockProcessorDeleted
                .Setup(p => p.ProcessAsync(It.IsAny<EntityDeletedPackage<TestEntity>>(), It.IsAny<CancellationToken>()))
                .Returns(Task.CompletedTask)
                .Callback(() =>
                {
                    Assert.True(deletingCalled);
                    Assert.True(executeDeleteCalled);
                    deletedCalled = true;
                });

            var mockProcessorProvider = new Mock<IProcessorProvider>(MockBehavior.Strict);
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityDeletingPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityDeletingPackage<TestEntity>>> { mockProcessorDeleting.Object });
            mockProcessorProvider
                .Setup(p => p.GetProcessors<EntityDeletedPackage<TestEntity>>())
                .Returns(new List<IProcessor<EntityDeletedPackage<TestEntity>>> { mockProcessorDeleted.Object });

            var expectedPipeline = new ProcessorPipeline(mockProcessorProvider.Object);
            var mockSubject = new Mock<Repository<TestEntity>>(MockBehavior.Strict, expectedPipeline);
            mockSubject
                .Protected()
                .Setup<Task>("ExecuteDeleteAsync", ItExpr.IsAny<TestEntity>(), It.IsAny<CancellationToken>())
                .Returns(Task.CompletedTask)
                .Callback(() =>
                {
                    Assert.True(deletingCalled);
                    Assert.False(deletedCalled);
                    executeDeleteCalled = true;
                });

            var expectedEntity = new TestEntity();

            await mockSubject.Object.DeleteAsync(expectedEntity, CancellationToken.None);

            mockProcessorProvider.Verify(p => p.GetProcessors<EntityDeletingPackage<TestEntity>>(), Times.Once);
            mockProcessorProvider.Verify(p => p.GetProcessors<EntityDeletedPackage<TestEntity>>(), Times.Once);

            mockProcessorDeleting.Verify(p => p.ProcessAsync(
                It.Is<EntityDeletingPackage<TestEntity>>(v => ReferenceEquals(v.Entity, expectedEntity)),
                It.IsAny<CancellationToken>()),
                Times.Once());

            mockProcessorDeleted.Verify(p => p.ProcessAsync(
                It.Is<EntityDeletedPackage<TestEntity>>(v => ReferenceEquals(v.Entity, expectedEntity)),
                It.IsAny<CancellationToken>()),
                Times.Once());

            mockSubject
                .Protected()
                .Verify("ExecuteDeleteAsync",
                    Times.Once(),
                    ItExpr.Is<TestEntity>(v => ReferenceEquals(v, expectedEntity)),
                    ItExpr.IsAny<CancellationToken>());
        }

        [Fact]
        public void GetAsyncQueryResolver_NoParams_CallsParamVersionWithUnmodifedQuery()
        {
            var mockExecuter = new Mock<IAsyncQueryResolver<TestEntity>>(MockBehavior.Strict);
            mockExecuter
                .Setup(e => e.AnyAsync(It.IsAny<Expression<Func<TestEntity, bool>>>(), It.IsAny<CancellationToken>()))
                .ReturnsAsync(true);

            var expectedPipeline = new ProcessorPipeline(null);
            var mockSubject = new Mock<Repository<TestEntity>>(MockBehavior.Strict, expectedPipeline);
            mockSubject
                .Setup(r => r.GetAsyncQueryResolver(It.IsAny<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>()))
                .Returns(mockExecuter.Object)
                .Callback<Func<IQueryable<TestEntity>, IQueryable<TestEntity>>>(f =>
                {
                    // Check to make sure the function just returns the same queryable, as-is.
                    var expectedQueryable = new List<TestEntity>().AsQueryable();
                    var result1 = f(expectedQueryable);
                    Assert.Same(expectedQueryable, result1);
                });

            var result = mockSubject.Object.GetAsyncQueryResolver();
            Assert.Same(result, mockExecuter.Object);
        }

        public class TestEntity : EntityWithIntId
        {
            public string StringValue
            {
                get => GetValue<string>();
                set => SetValue(value);
            }
        }

        public class TestRepository : Repository<TestEntity>
        {
            public TestRepository(ProcessorPipeline processorPipeline)
                : base(processorPipeline)
            {
            }

            public ProcessorPipeline TestGetInternalPipeline()
            {
                return ProcessorPipeline;
            }

            public override Task<TestEntity> FindAsync(object[] keys, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            public override IQueryable<TestEntity> Get()
            {
                throw new NotImplementedException();
            }

            public override IAsyncQueryResolver<TResult> GetAsyncQueryResolver<TResult>(Func<IQueryable<TestEntity>, IQueryable<TResult>> query)
            {
                throw new NotImplementedException();
            }

            protected override Task ExecuteAddAsync(TestEntity entity, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            protected override Task ExecuteDeleteAsync(TestEntity entity, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }

            protected override Task<ChangeSet> ExecuteUpdateAsync(TestEntity entity, ChangeSet changes, CancellationToken cancellationToken)
            {
                throw new NotImplementedException();
            }
        }
    }
}
