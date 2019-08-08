using System;
using System.IO;
using System.Reflection;
using log4net;
using Tokenio.Sdk;
using Tokenio.Sdk.GrpcServer;

namespace Tokenio.BankSample
{
    /// <summary>
    ///  Main service class. {@link CliArgs} defines the available command line arguments}.
    /// The application parses command line arguments and then configures and starts
    /// gRPC server that listens for incoming requests.
    /// </summary>
    public class Application
    {
        private static readonly ILog logger = LogManager
            .GetLogger(MethodBase.GetCurrentMethod().DeclaringType);

        /// <summary>
        /// App main entry.
        /// </summary>
        /// <param name="argv">cli args</param>
        public static void Main(string[] argv)
        {
            FileInfo file = new FileInfo("log4net.config");
            var logRepository = LogManager.GetRepository(Assembly.GetEntryAssembly());
            log4net.Config.XmlConfigurator.Configure(logRepository, file);

            CliArgs args = new CliArgs(argv);
            logger.Info("Command line arguments: " + args.ToString());

            StartRpcServer(args);
        }
                       
        private static void StartRpcServer(CliArgs args)
        {
            // Create a factory used to instantiate all the service implementations
            // that are needed to initialize the server.
            Factory factory = new Factory("config/application.conf");

            // Build a gRPC server instance.
            ServerBuilder serverBuilder = ServerBuilder
                    .ForPort(args.port)
                    .ReportErrorDetails()
                    .WithAccountService(factory.AccountService())
                    .WithAccountLinkingService(factory.AccountLinkingService())
                    .WithTransferService(factory.TransferService())
                    .WithStorageService(factory.StorageService());

            if (args.useSsl)
            {
                serverBuilder.WithTls(
                        "config/tls/cert.pem",
                        "config/tls/key.pem",
                        "config/tls/trusted-certs.pem");
            }

            RpcServer server = serverBuilder.Build();

            // You will need to Ctrl-C to exit.
            server.Start();
            logger.Info("Server started on port: " + args.port);
            Console.WriteLine("Hit return to stop the server ");
            Console.ReadKey();

            server.Close();
        }
    }
}
