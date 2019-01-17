using System.Threading.Tasks;
using Funda.Shared;

namespace Funda.Core
{
    public class InformationCollector
    {
        private readonly IApiGetter _apiGetter;

        public InformationCollector(IApiGetter apiGetter)
        {
            _apiGetter = apiGetter;
        }

        public async Task<string> GetMakelaarsInfo() => await _apiGetter.GetMakelaarInformation();
    }
}
