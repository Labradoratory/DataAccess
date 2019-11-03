using Labradoratory.DataAccess.ChangeTracking;
using Xunit;

namespace Labradoratory.DataAccess.Test.ChangeTracking
{
    public class ChangeTrackingObject_Tests
    {
        [Fact]
        public void HasChanges_FalseOnInitialize()
        {
            var subject = new TestObject();
            Assert.False(subject.HasChanges);
        }

        [Fact]
        public void HasChanges_TrueWhenChanges()
        {
            var subject = ChangeTrackingObject.CreateTrackable<TestObject>();
            subject.StringValue = "NewValue";
            Assert.True(subject.HasChanges);
        }

        private class TestObject : ChangeTrackingObject
        {
            public string StringValue
            {
                get => GetValue<string>();
                set => SetValue(value);
            }

            public string IntValue
            {
                get => GetValue<string>();
                set => SetValue(value);
            }

            public NestedObject NestedValue
            {
                get => GetValue<NestedObject>();
                set => SetValue(value);
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
