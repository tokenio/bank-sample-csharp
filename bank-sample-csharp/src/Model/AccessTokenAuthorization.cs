using System.Collections.Generic;
using Tokenio.Integration.Api;


namespace Tokenio.BankSample.Model
{
    public class AccessTokenAuthorization
    {
        public string AccessToken { get; private set; }
        public string MemberId { get; private set; }
        public IList<NamedAccount> Accounts { get; private set; }

        /// <summary>
        /// Creates a new access token authorization object.
        /// </summary>
        /// <param name="accessToken">access token string</param>
        /// <param name="memberId">token member id</param>
        /// <param name="accounts">list of named account</param>
        public AccessTokenAuthorization (
            string accessToken,
            string memberId,
            IList<NamedAccount> accounts)
        {
            this.AccessToken = accessToken;
            this.MemberId = memberId;
            this.Accounts = accounts;
        }

    }
}
