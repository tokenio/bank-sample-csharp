using System.Collections.Generic;
using Tokenio.Proto.BankLink;
using Tokenio.Sdk.Utils;

namespace Tokenio.BankSample.Model.Impl
{
    public class AccountLinkingImpl : IAccountLinking
    {
        private readonly BankAccountAuthorizer authorizer;
        private readonly IDictionary<string, AccessTokenAuthorization> authorizations;

        public AccountLinkingImpl(
                BankAccountAuthorizer authorizer,
                IDictionary<string, AccessTokenAuthorization> authorizations)
        {
            this.authorizer = authorizer;
            this.authorizations = authorizations;
        }

        public BankAuthorization GetBankAuthorization(string accessToken)
        {
            AccessTokenAuthorization authorization = authorizations.ContainsKey(accessToken) ? authorizations[accessToken] : null;
            return authorizer.CreateAuthorization(RequestContext.GetAccountHolderId(), authorization.Accounts);
        }
    }
}
