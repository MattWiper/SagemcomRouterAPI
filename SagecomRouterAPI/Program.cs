using CommandLine;
using System;
using System.Collections.Generic;
using System.Threading.Tasks;

namespace SagecomRouterAPI
{
    public enum Request
    {
        [RequestDef(@".\Requests\get-devices.json")]
        GetDevices,

        [RequestDef(@".\Requests\get-hosts.json")]
        GetHosts,

        [RequestDef(@".\Requests\get-port-mappings.json")]
        GetPortMappings,

        [RequestDef(@".\Requests\get-device-info.json")]
        GetDeviceInfo,
    }

    class Program
    {
        public class Options
        {
            [Option('u', Required = true)]
            public string Username { get; set; }

            [Option('p', Required = true)]
            public string Password { get; set; }

            [Option('c', Required = true)]
            public string Command { get; set; }

            [Option('o', Required = true)]
            public string Host { get; set; }

            [Option('r', Required = true)]
            public Request Request { get; set; }
        }

        static void Main(string[] args)
        {
            try
            {
                Console.WriteLine("Request");


                var parser = new Parser(cfg => cfg.CaseInsensitiveEnumValues = true);

                var res = parser.ParseArguments<Options>(args).WithParsed(Run).WithNotParsed(ParseError);
                //res = res.WithParsedAsync(Run).Result;
                //res.WithNotParsed(ParseError);
                //res.

            }
            catch (Exception e)
            {
                Console.WriteLine(e);
            }

        }

        private static void Run(Options options)
        {
            Router router = new Router();

            try
            {
                Console.WriteLine("Reques1");

                var k = router.HandleRequest(options.Request, options.Username, options.Password, options.Host).Result;
            }
            catch (Exception e)
            {
                Console.WriteLine($"Error processing request. Error: {e.Message}");
            }
        }

        private static void ParseError(IEnumerable<Error> errors)
        {
            Console.WriteLine("Errors:");
            foreach (var error in errors)
            {
                Console.WriteLine(error);
            }
        }
    }
}
