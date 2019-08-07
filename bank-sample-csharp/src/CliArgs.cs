using System;
using McMaster.Extensions.CommandLineUtils;

namespace Tokenio.BankSample
{
    /// <summary>
    /// Command line arguments supported by the server application.
    /// </summary>
    public class CliArgs
    {
        public int port { get; set; } = 9300;
        public bool useSsl { get; set; } = false;
        public string config { get; set; } = "config";
        public bool usage { get; set; }
        public bool useHttp { get; set; } = false;
        public string httpBearerToken { get; set; }


        public CliArgs(string[] argv)
        {
            var app = new CommandLineApplication();
            app.HelpOption("-h|--help");
            var portCmd = app.Option("--port|-p", "gRPC port to listen on", CommandOptionType.SingleValue);
            var usesslCmd = app.Option("--ssl | -s", "gRPC port to listen on", CommandOptionType.SingleValue);
            var usehttpCmd = app.Option("--http", "gRPC port to listen on", CommandOptionType.SingleValue);
            var httpBearerTokenCmd = app.Option("--http-bearer-token", "gRPC port to listen on", CommandOptionType.SingleValue);
            var configCmd = app.Option("--config| -c", "gRPC port to listen on", CommandOptionType.SingleValue);
            var usageCmd = app.Option("--usage|-u", "gRPC port to listen on", CommandOptionType.SingleValue);
            app.Execute(argv);

            port = portCmd.HasValue() ? int.Parse(portCmd.Value()) : port;
            useSsl = usesslCmd.HasValue() ? bool.Parse(usesslCmd.Value()) : useSsl;
            config = configCmd.HasValue() ? configCmd.Value() : config;
            usage = usageCmd.HasValue() ? bool.Parse(usageCmd.Value()) : usage;
            useHttp = usehttpCmd.HasValue() ? bool.Parse(usehttpCmd.Value()) : useHttp;
            httpBearerToken = httpBearerTokenCmd.HasValue() ? httpBearerTokenCmd.Value() : httpBearerToken;
        }

        public override string ToString()
        {
            return "port = " + port + ", useSsl =" + useSsl + ", config = " + config + ", usage = " + usage + ", useHttp = " + useHttp + ", httpBearerToken = " + httpBearerToken;
        }
    }
}
