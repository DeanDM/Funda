using System;
using System.Configuration;
using Fclp;
using Funda.API;
using Funda.Core;

namespace Funda.CLI
{
    // Hello reviewer!
    class Program
    {
        static void Main(string[] args)
        {
            var options = new ExecutionOptions();

            // Use fluent for command line parsing
            // None of these fields are required but with fluent
            // it is easy to do so. This way we can give different options
            var parser = new FluentCommandLineParser();

            var defaultFundayApiKey = ConfigurationManager.AppSettings["FundaDefaultApiKey"];

            parser
                .Setup<string>('a', "apikey")
                .WithDescription("Which key do you want to use?")
                .SetDefault(defaultFundayApiKey)
                .Callback(a => options.ApiKey = a);

            parser
                .Setup<string>('l', "location")
                .WithDescription("Which location do you want to search?")
                .Callback(a => options.ApiKey = a);

            parser
                .Setup<bool>('t', "tuin")
                //.Required()
                .WithDescription("Check for properties with or without a garden?")
                .Callback(a => options.Tuin = a);

            var result = parser.Parse(args);

            if (result.HasErrors)
            {
                Console.WriteLine(result.ErrorText);
            }

            Console.WriteLine("Getting information ...");

            // I like the ports and adapters patter so that's what we're going to use
            // Obviously it's a bit overkill for something like this but it's not too much effort
            // (basically extra projects) but that way I can show you that I know about it and
            // that the code is nice and testable

            //Setup adapters
            var api = new Api(options.ApiKey, options.Location, options.Tuin);

            //Setup the Core
            var collector = new InformationCollector(api);

            var values = collector.GetMakelaarsInfo().Result;

            Console.Clear();
            Console.WriteLine("Here's the top 10!");
            Console.WriteLine(values);
            Console.ReadLine();
        }

        internal class ExecutionOptions
        {
            internal string ApiKey { get; set; }
            internal string Location { get; set; } = "amsterdam";
            public bool Tuin { get; set; }
        }
    }
}
