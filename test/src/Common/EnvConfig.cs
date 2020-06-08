using System.IO;
using System.Net;
using System.Reflection;
using log4net;
using log4net.Config;
using Microsoft.Extensions.Configuration;

namespace Tokenio.BankSample.Common
{
    public class EnvConfig
    {
        private readonly string bankId;
        private readonly string devKey;
        private readonly DnsEndPoint gateway;
        private readonly DnsEndPoint restfulGateway;
        private readonly bool useSsl;

        public EnvConfig(IConfiguration config)
        {
            useSsl = bool.Parse(config["use-ssl"]);
            bankId = config["bank-id"];
            var gatewaySection = config.GetSection("gateway");
            var restGatewaySection = config.GetSection("restful-gateway");
            gateway = new DnsEndPoint(
                gatewaySection["host"],
                int.Parse(gatewaySection["port"]));
            restfulGateway = new DnsEndPoint(
                restGatewaySection["host"],
                int.Parse(gatewaySection["port"]));
            devKey = config["dev-key"];
            var f = new FileInfo("log4net.config"); //please modify this line
            var logRepository =
                LogManager.GetRepository(Assembly.GetEntryAssembly());
            XmlConfigurator.Configure(logRepository, f);
        }

        public bool UseSsl()
        {
            return useSsl;
        }

        public string GetBankId()
        {
            return bankId;
        }

        public DnsEndPoint GetGateway()
        {
            return gateway;
        }

        public DnsEndPoint GetRestfulGateway()
        {
            return restfulGateway;
        }

        public string GetDevKey()
        {
            return devKey;
        }
    }
}
