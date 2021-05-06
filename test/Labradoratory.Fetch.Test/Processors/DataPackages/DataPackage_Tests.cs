using System;
using System.Collections.Generic;
using Labradoratory.Fetch.Processors.DataPackages;
using Xunit;

namespace Labradoratory.Fetch.Test.Processors.DataPackages
{
    public class DataPackage_Tests
    {
        [Fact]
        public void GetValue_Success()
        {
            var subject = new TestDataPackage();
            var expectedKey = "TestKey";
            var expectedValue = "My value";
            subject[expectedKey] = expectedValue;
            var result = subject.TestGetValue<string>(expectedKey);
            Assert.Equal(expectedValue, result);
        }

        [Fact]
        public void GetValue_ThrowsWhenPropertyNull()
        {
            var subject = new TestDataPackage();
            Assert.Throws<ArgumentNullException>(() => subject.TestGetValue<string>(null));
        }

        [Fact]
        public void GetValue_PrimitiveNotFound_Default()
        {
            var subject = new TestDataPackage();
            var result = subject.TestGetValue<int>("notfound");
            Assert.Equal(default, result);
        }

        [Fact]
        public void GetValue_ObjectNotFound_Default()
        {
            var subject = new TestDataPackage();
            var result = subject.TestGetValue<TestDataPackage>("notfound");
            Assert.Null(result);
        }

        [Fact]
        public void SetValue_ThrowsWhenPropertyNull()
        {
            var subject = new TestDataPackage();
            Assert.Throws<ArgumentNullException>(() => subject.TestSetValue(100, null));
        }

        [Fact]
        public void SetValue_Success()
        {
            var subject = new TestDataPackage();
            var expectedValue = 100;
            var expectedKey = "TestKey";
            subject.TestSetValue(expectedValue, expectedKey);
            Assert.Contains(expectedKey, subject as IDictionary<string, object>);
            Assert.Equal(expectedValue, subject[expectedKey]);
        }

        public class TestDataPackage : DataPackage
        {
            public T TestGetValue<T>(string propertyName)
            {
                return GetValue<T>(propertyName);
            }

            public void TestSetValue<T>(T value, string propertyName)
            {
                SetValue(value, propertyName);
            }
        }
    }
}
