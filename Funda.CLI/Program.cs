using System;
using System.Configuration;
using Fclp;
using Funda.API;
using Funda.Core;

namespace Funda.CLI
{
    class Program
    {
        static void Main(string[] args)
        {
            var options = new ExecutionOptions();

            // Use fluent for command line parsing
            // None of these fields are required but with fluent
            // it is easy to do so
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

            var api = new Api(options.ApiKey, options.Location, options.Tuin);
            var collector = new InformationCollector(api);

            var values = collector.GetMakelaarsInfo().Result;;

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
