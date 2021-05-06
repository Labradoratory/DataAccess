using Labradoratory.Fetch.Extensions;
using Xunit;

namespace Labradoratory.Fetch.Test.Extensions
{
    public class StringExtensions_Tests
    {
        [Fact]
        public void ToCamelCase_PascalCase_Success()
        {
            const string value = "TestValueOne";
            const string expected = "testValueOne";
            var result = value.ToCamelCase();
            Assert.Equal(expected, result);
        }

        [Fact]
        public void ToCamelCase_PascalCaseWithAcronym_Success()
        {
            const string value = "ACRONYMTestValueOne";
            const string expected = "acronymTestValueOne";
            var result = value.ToCamelCase();
            Assert.Equal(expected, result);
        }
    }
}
