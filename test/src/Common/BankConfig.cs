using System.Collections.Generic;
using Microsoft.Extensions.Configuration;
using Tokenio.BankSample.Utils;
using Tokenio.Security;
using Tokenio.Security.Keystore;

namespace Tokenio.BankSample.Common
{
    public class BankConfig
    {
        private readonly string secretKeyId;
        private readonly ISecretKeyStore secretKeyStore;
        private readonly string trustedKeyId;
        private readonly ITrustedKeyStore trustedKeyStore;

        public BankConfig(string bankId, string env)
        {
            var keyConfigFileName = string.Format(
                "resources/{0}-bank.conf",
                bankId);
            var config =
                new ConfigurationBuilder().AddJsonFile(keyConfigFileName)
                    .Build();
            var bankConfig = config.GetSection(env);
            SecretKeyPair secretKeyPair =
                TestKeyUtils.ToSecretKeyPair(
                    bankConfig["secret-key-store:public-key"],
                    bankConfig["secret-key-store:private-key"],
                    bankConfig["secret-key-store:algorithm"]);
            secretKeyId = secretKeyPair.Id();
            secretKeyStore = new InMemorySecretKeyStore(secretKeyPair);
            var trustedKey =
                TestKeyUtils.ToTrustedKey(
                    bankConfig["trusted-key-store:public-key"],
                    bankConfig["trusted-key-store:algorithm"]);
            trustedKeyId = trustedKey.Id();
            trustedKeyStore =
                new InMemoryTrustedKeyStore(new List<TrustedKey> {trustedKey});
        }

        public ISecretKeyStore GetSecretKeyStore()
        {
            return secretKeyStore;
        }

        public ITrustedKeyStore GetTrustedKeyStore()
        {
            return trustedKeyStore;
        }

        public string GetSecretKeyId()
        {
            return secretKeyId;
        }

        public string GetTrustedKeyId()
        {
            return trustedKeyId;
        }
    }
}
