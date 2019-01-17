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

        // Once again I know this file seems a bit useless now but if we extend the code
        // or wanna add extra's like caching or logging or another api, these are all just adapters
        // that we pass into the core. http://blog.ploeh.dk/2013/12/03/layers-onions-ports-adapters-its-all-the-same/ explains
        // it better than I ever can!
        public async Task<string> GetMakelaarsInfo() => await _apiGetter.GetMakelaarInformation();
    }
}
