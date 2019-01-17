using Funda.API;
using Xunit;

namespace Funda.Tests
{
    public class ApiTests
    {
        private const string TestApiKey = "ac1b0b1572524640a0ecc54de453ea9f";

        [Fact]
        public void The_api_can_return_things()
        {
            var api = new Api(TestApiKey, "amsterdam", false);

            // Calling an external api in your tests is a bit risky because it can be flaky. 
            // Maybe put it in another project called Integration tests
            var result = api.GetMakelaarInformation().Result;

            Assert.NotNull(result);
            Assert.False(string.IsNullOrEmpty(result), "Expected result to not be empty but it was");
        }
    }
}