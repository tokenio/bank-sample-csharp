using System.Collections.Generic;
using System.Linq;
using Tokenio.BankSample.Restful;
using Tokenio.BankSample.Utils;

namespace Tokenio.BankSample.Common
{
    public class TokenRestfulBankRule : TokenRule
    {
        public readonly BankClient bankClient;

        public TokenRestfulBankRule() : this(AuthorizationType.Token)
        {
        }

        public TokenRestfulBankRule(AuthorizationType type)
            : this(type, string.Empty)
        {
        }

        public TokenRestfulBankRule(AuthorizationType type,
            string optionalBankId)
        {
            var bankId = string.IsNullOrEmpty(optionalBankId)
                ? envConfig.GetBankId()
                : optionalBankId;

            var bankConfig = new BankConfig(
                bankId,
                GetEnvProperty("TOKEN_ENV", defaultEnv));
            bankClient = new BankClient(
                bankId,
                bankConfig,
                envConfig.GetRestfulGateway(),
                type);
        }

        public BankClient GetBankClient()
        {
            return bankClient;
        }

        public List<NamedAccount> CreateAccount(int count)
        {
            var client = testBank.NewClient();
            return Enumerable.Range(0, count)
                .Select(index => testBank.NewAccount(client))
                .ToList();
        }
    }
}
