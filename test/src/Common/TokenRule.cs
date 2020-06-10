using System;
using Grpc.Core;
using Grpc.Core.Interceptors;
using Io.Token.Proto.Gateway.Testing;
using Microsoft.Extensions.Configuration;
using Tokenio.BankSample.Fank;
using Tokenio.Rpc;

namespace Tokenio.BankSample.Common
{
    public abstract class TokenRule
    {
        protected static readonly long timeoutMs = 10 * 60 * 1000;
        protected static readonly string defaultEnv = "development";

        protected readonly EnvConfig envConfig;
        protected readonly FankTestBank testBank;

        protected readonly TestingGatewayService.TestingGatewayServiceClient
            testingGateway;
        
        public TokenRule()
            : this(new ConfigurationBuilder()
                .AddJsonFile(
                    string.Format(
                        "resources/{0}.conf",
                        GetEnvProperty("TOKEN_ENV", defaultEnv)))
                .Build())
        {
        }

        private TokenRule(IConfiguration config)
        {
            envConfig = new EnvConfig(config);
            testBank = new FankTestBank(config);
            var hostAndPort = envConfig.GetGateway();
            var channel = new Channel(
                hostAndPort.Host,
                hostAndPort.Port,
                envConfig.UseSsl()
                    ? new SslCredentials()
                    : ChannelCredentials.Insecure);
            Interceptor[] interceptors =
            {
                new AsyncTimeoutInterceptor(timeoutMs)
            };
            channel.Intercept(interceptors);
            testingGateway =
                new TestingGatewayService.TestingGatewayServiceClient(channel);
        }

        public static string GetEnvProperty(string name, string defaultValue)
        {
            return Environment.GetEnvironmentVariable(name) ?? defaultValue;
        }
    }
}
