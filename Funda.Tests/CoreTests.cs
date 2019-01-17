using System.Threading.Tasks;
using Funda.Core;
using Funda.Shared;
using Xunit;

namespace Funda.Tests
{
    public class CoreTests
    {
        // The core tests are a bit boring but using the ports and adapter pattern
        // makes it quite easy to make mocks and just test your actual business code that lives
        // in the core. My business code is basically return the list :)
        [Fact]
        public void InformationCollector_can_return_makelaars()
        {
            var collector = new InformationCollector(new FakeApiGetter());

            var result = collector.GetMakelaarsInfo().Result;

            Assert.False(string.IsNullOrEmpty(result), "Expected result to not be empty but it was.");
        }
    }

    //I can use a mocking framework or can just use fake classes that essentially do the same.
    public class FakeApiGetter : IApiGetter
    {
        public Task<string> GetMakelaarInformation() => Task.FromResult("New Makelaar");
    }
}
