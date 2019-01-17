using System;
using System.Collections.Generic;
using System.Linq;
using System.Net.Http;
using System.Net.Http.Headers;
using System.Text;
using System.Threading.Tasks;
using Funda.API.Models;
using Funda.Shared;
using Newtonsoft.Json;
using Polly;

namespace Funda.API
{
    public class Api : IApiGetter
    {
        private const string BaseUrl = "http://partnerapi.funda.nl/feeds/Aanbod.svc/";
        private readonly string _optionsApiKey;
        private readonly string _location;
        private readonly bool _withGarden;

        public Api(string optionsApiKey, string location, bool withGarden)
        {
            _optionsApiKey = optionsApiKey;
            _location = location;
            _withGarden = withGarden;
        }

        public async Task<string> GetMakelaarInformation()
        {
            // Build the url
            var tuin = _withGarden ? "/tuin/" : string.Empty;

            // GetAllPages basically loops trough pages to get all results.
            var allResults = await GetAllPages.For(new Uri($"{BaseUrl}{_optionsApiKey}/?type=koop&zo=/{_location}{tuin}&page="));

            //Groups all results by makelaarId and how often they appear
            var grouped = SquashedAndCountedList(allResults);

            // Then build the results. Another way to do this would be to return
            // grouped and then let the cient decide what they want to do with that information
            // but for this excercise I just made it return string
            var builder = new StringBuilder();
            foreach (var groupedResult in grouped.OrderByDescending(p => p.Count).Take(10))
            {
                builder.AppendLine($"MakelaarId = {groupedResult.MakelaarId} Result = {groupedResult.Count}");
            }

            return builder.ToString();
        }

        private static IEnumerable<GroupedResult> SquashedAndCountedList(MakelaarResults listOfMakelaars)
            => listOfMakelaars
                .Objects
                .GroupBy(p => p.MakelaarId)
                .Select(p => new GroupedResult
                {
                    MakelaarId = p.Key,
                    Count = p.Count()
                });
    }

    internal class GetAllPages
    {

        public static async Task<MakelaarResults> For(Uri baseUri)
        {
            //Initalize all the values
            var makelaarResults = new MakelaarResults();
            var client = new HttpClient();
            var counter = 1;
            var isThereANextPage = true;

            // I use Polly to handle retries. If this would fail for some reason it would retry once but we
            // can configure it to retry multiple times with a timeout if we want. Thanks to Polly that's quite easy
            var retryPolicy = Policy
                                .Handle<Exception>()
                                .Retry();
            do
            {
                //Build the request
                var request = new HttpRequestMessage
                {
                    RequestUri = new Uri(baseUri + counter.ToString()),
                    Method = HttpMethod.Get,
                };

                // I like working with json a bit more so I opted for json
                request.Headers.Accept.Add(new MediaTypeWithQualityHeaderValue("application/json"));

                // do "get a page" while "there is a next page"
                await retryPolicy.Execute(() => client.SendAsync(request)
                    .ContinueWith(async getMakelaarsTask =>
                    {
                        var res = await getMakelaarsTask;
                        if (res.IsSuccessStatusCode == false)
                        {
                            isThereANextPage = false;
                            // Ideally this would log somewhere too.
                            throw new Exception($"Something went wrong calling the api: result = {await res.Content.ReadAsStringAsync()}");
                        }

                        // If we made it here that means our request was a success and we have some data.
                        // deserialize it and add it to the previous results
                        var rawJsonResult = await res.Content.ReadAsStringAsync();

                        var listOfMakelaars = JsonConvert.DeserializeObject<MakelaarResults>(rawJsonResult);
                        makelaarResults.Objects.AddRange(listOfMakelaars.Objects);

                        // I know this is a bit weird but the result of volgende url looks like
                        // /~/koop/amsterdam/p2/ which is a bit annoying because i can't just paste
                        // that at the end of the request which has the format ?type=koop&zo=/amsterdam
                        // so I just add the page at the end and if there's a value in here, just
                        // go to the next page, else set nextpage to empty string so the loop ends
                        if (string.IsNullOrEmpty(listOfMakelaars.Paging?.VolgendeUrl) == false)
                        {
                            counter++;
                        }
                        else
                        {
                            isThereANextPage = false;
                        }
                    }));
            } while (isThereANextPage);

            return makelaarResults;
        }
    }
}
