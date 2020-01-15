using System;
using System.Collections.Generic;
using Labradoratory.Fetch.ChangeTracking;
using Xunit;

namespace Labradoratory.Fetch.Test.ChangeTracking
{
    public class ChangeTrackingObject_Tests
    {
        [Fact]
        public void HasChanges_FalseOnInitialize()
        {
            var subject = ChangeTrackingObject.CreateTrackable<TestObject>();
            Assert.False(subject.HasChanges);
        }

        [Fact]
        public void HasChanges_TrueWhenChanges()
        {
            var subject = ChangeTrackingObject.CreateTrackable<TestObject>();
            subject.StringValue = "NewValue";
            Assert.True(subject.HasChanges);
        }

        [Fact]
        public void GetValue_Success()
        {
            var expectedValue = "My expected value";
            var subject = ChangeTrackingObject.CreateTrackable<TestObject>();
            subject.StringValue = expectedValue;
            var result = subject.TestGetValue<string>(nameof(subject.StringValue));
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GetValue_ThrowsWhenNull()
        {
            var subject = ChangeTrackingObject.CreateTrackable<TestObject>();
            Assert.Throws<ArgumentNullException>(() => subject.TestGetValue<string>(null));
        }

        [Fact]
        public void GetValue_ReturnsDefaultWhenNotSet()
        {
            var subject = ChangeTrackingObject.CreateTrackable<TestObject>();
            var result = subject.TestGetValue<int>("someproperty");
            Assert.Equal(default, result);
        }

        [Fact]
        public void SetValue_Success()
        {
            var expectedValue = "My expected value";
            var subject = ChangeTrackingObject.CreateTrackable<TestObject>();
            subject.TestSetValue(expectedValue, nameof(subject.StringValue));
            Assert.Equal(expectedValue, subject.StringValue);
        }

        [Fact]
        public void SetValue_ThrowsWhenNull()
        {
            var subject = ChangeTrackingObject.CreateTrackable<TestObject>();
            Assert.Throws<ArgumentNullException>(() => subject.TestSetValue("test value", null));
        }

        [Fact]
        public void Reset_ClearsAllChanges()
        {
            var subject = ChangeTrackingObject.CreateTrackable<TestObject>();
            subject.IntValue = 100;
            subject.StringValue = "test";
            subject.NestedValue = new NestedObject();
            Assert.True(subject.HasChanges);
            subject.Reset();
            Assert.False(subject.HasChanges);
        }

        [Fact]
        public void GetChangeSet_HasChangesFalse_ReturnsNull()
        {
            var subject = ChangeTrackingObject.CreateTrackable<TestObject>();
            Assert.False(subject.HasChanges);
            var result = subject.GetChangeSet(ChangePath.Empty);
            Assert.Null(result);
        }

        [Fact]
        public void GetChangeSet_HasChangesTrue_ReturnsMergedChangeSet()
        {
            var subject = new TestObject()
            {
                IntValue = 50,
                StringValue = "The old value"
            };
            subject.IntValue = 100;
            subject.StringValue = "The new value";

            var result = subject.GetChangeSet(ChangePath.Empty);
            Assert.Equal(2, result.Count);
        }

        [Fact]
        public void HasChanges_NotTrueWhenSameValue()
        {
            var expected = "Test string";
            var subject = new TestObject();
            subject.StringValue = expected;
            Assert.False(subject.HasChanges);
            subject.StringValue = expected;
            Assert.False(subject.HasChanges);
        }

        private class TestObject : ChangeTrackingObject
        {
            public string StringValue
            {
                get => GetValue<string>();
                set => SetValue(value);
            }

            public int IntValue
            {
                get => GetValue<int>();
                set => SetValue(value);
            }

            public NestedObject NestedValue
            {
                get => GetValue<NestedObject>();
                set => SetValue(value);
            }

            public T TestGetValue<T>(string property)
            {
                return GetValue<T>(property);
            }

            public void TestSetValue<T>(T value, string property)
            {
                SetValue(value, property);
            }
        }

        private class NestedObject : ChangeTrackingObject
        {
            public string StringValue
            {
                get => GetValue<string>();
                set => SetValue(value);
            }
        }
    }
}
