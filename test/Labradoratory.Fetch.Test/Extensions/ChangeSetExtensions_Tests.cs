using System.Text.Json;
using Labradoratory.Fetch.ChangeTracking;
using Labradoratory.Fetch.Extensions;
using Microsoft.AspNetCore.JsonPatch.Operations;
using Xunit;

namespace Labradoratory.Fetch.Test.Extensions
{
    public class ChangeSetExtensions_Tests
    {
        [Fact]
        public void ToJsonPatch_Add_Simple_Success()
        {
            var path = "Test";
            var expectedPath = path.ToLower();
            var expectedNewValue = "My new value";
            var changes = ChangeSet.Create(
                ChangePath.Create(path),
                new ChangeValue
                {
                    Action = ChangeAction.Add,
                    NewValue = expectedNewValue
                });

            var patch = changes.ToJsonPatch();
            Assert.Single(patch);
            var operation = patch[0];
            Assert.Equal(OperationType.Add, operation.OperationType);
            Assert.Equal(expectedPath, operation.path);
            Assert.Equal(JsonSerializer.Serialize(expectedNewValue), operation.value);
        }

        [Fact]
        public void ToJsonPatch_Add_Complex_Success()
        {
            var path = "Test";
            var expectedPath = path.ToLower();
            var expectedNewValue = new TestValue
            {
                StringValue = "My new complex value",
                IntValue = 12345
            };
            var changes = ChangeSet.Create(
                ChangePath.Create(path),
                new ChangeValue
                {
                    Action = ChangeAction.Add,
                    NewValue = expectedNewValue
                });

            var patch = changes.ToJsonPatch();
            Assert.Single(patch);
            var operation = patch[0];
            Assert.Equal(OperationType.Add, operation.OperationType);
            Assert.Equal(expectedPath, operation.path);
            var resultValue = Assert.IsType<TestValue>(operation.value);
            Assert.Equal(expectedNewValue.StringValue, resultValue.StringValue);
            Assert.Equal(expectedNewValue.IntValue, resultValue.IntValue);
        }

        [Fact]
        public void ToJsonPatch_Update_Simple_Success()
        {
            var path = "Test";
            var expectedPath = path.ToLower();
            var expectedNewValue = "My new value";
            var changes = ChangeSet.Create(
                ChangePath.Create(path),
                new ChangeValue
                {
                    Action = ChangeAction.Update,
                    NewValue = expectedNewValue
                });

            var patch = changes.ToJsonPatch();
            Assert.Single(patch);
            var operation = patch[0];
            Assert.Equal(OperationType.Replace, operation.OperationType);
            Assert.Equal(expectedPath, operation.path);
            Assert.Equal(expectedNewValue, operation.value);
        }

        [Fact]
        public void ToJsonPatch_Update_Complex_Success()
        {
            var path = "Test";
            var expectedPath = path.ToLower();
            var expectedNewValue = new TestValue
            {
                StringValue = "My new complex value",
                IntValue = 12345
            };
            var changes = ChangeSet.Create(
                ChangePath.Create(path),
                new ChangeValue
                {
                    Action = ChangeAction.Update,
                    NewValue = expectedNewValue
                });

            var patch = changes.ToJsonPatch();
            Assert.Single(patch);
            var operation = patch[0];
            Assert.Equal(OperationType.Replace, operation.OperationType);
            Assert.Equal(expectedPath, operation.path);
            Assert.Equal(expectedNewValue, operation.value);
        }

        [Fact]
        public void ToJsonPatch_Remove_Success()
        {
            var path = "Test";
            var expectedPath = path.ToLower();
            var expectedNewValue = "My new value";
            var changes = ChangeSet.Create(
                ChangePath.Create(path),
                new ChangeValue
                {
                    Action = ChangeAction.Remove,
                    NewValue = expectedNewValue
                });

            var patch = changes.ToJsonPatch();
            Assert.Single(patch);
            var operation = patch[0];
            Assert.Equal(OperationType.Remove, operation.OperationType);
            Assert.Equal(expectedPath, operation.path);
            Assert.Null(operation.value);
        }

        [Fact]
        public void ToJsonPatch_IgnoresActionNone()
        {
            var changes = ChangeSet.Create(
                ChangePath.Create("Test"),
                new ChangeValue
                {
                    Action = ChangeAction.None
                });

            var patch = changes.ToJsonPatch();
            Assert.Empty(patch);
        }

        public class TestValue
        {
            public string StringValue { get; set; }
            public int IntValue { get; set; }
        }
    }
}
