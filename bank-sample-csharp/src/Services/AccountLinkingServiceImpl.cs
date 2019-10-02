using Tokenio.BankSample.Model;
using Tokenio.Integration.Api.Service;
using Tokenio.Proto.BankLink;

namespace Tokenio.BankSample.Services
{
    public class AccountLinkingServiceImpl : IAccountLinkingService
    {
        private readonly IAccountLinking accountLinking;

        public AccountLinkingServiceImpl(IAccountLinking accountLinking)
        {
            this.accountLinking = accountLinking;
        }

        public BankAuthorization GetBankAuthorization(string accessToken)
        {
            return accountLinking.GetBankAuthorization(accessToken);
        }
    }
}
