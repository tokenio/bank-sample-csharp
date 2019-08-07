using System.Collections.Generic;
using Io.Token.Proto.Bankapi;
using Tokenio.BankSample.Config;
using Tokenio.Proto.Common.AccountProtos;
using Tokenio.Sdk.Api;

namespace Tokenio.BankSample.Model
{
    /// <summary>
    /// Provides access to the configured list of accounts.
    /// </summary>
    public abstract class IAccounts
    {
        /// <summary>
        /// Returns hold account for the given currency.
        /// </summary>
        /// <param name="currency">currency to lookup the account for</param>
        /// <returns>looked up account</returns>
        public abstract BankAccount GetHoldAccount(string currency);

        /// <summary>
        /// Returns FX account for the given currency.
        /// </summary>
        /// <param name="currency">currency to lookup the account for</param>
        /// <returns>all accounts</returns>
        public abstract BankAccount GetFxAccount(string currency);

        /// <summary>
        /// Returns all the configured accounts.
        /// </summary>
        /// <returns></returns>
        public abstract IList<AccountConfig> GetAllAccounts();

        /// <summary>
        /// Looks up the account.
        /// </summary>
        /// <param name="account">account to look up</param>
        /// <returns>looked up account</returns>
        public abstract AccountConfig TryLookupAccount(BankAccount account);

        /// <summary>
        /// Looks up the account.
        /// </summary>
        /// <param name="account">account to look up</param>
        /// <returns>looked up account</returns>
        internal AccountConfig LookupAccount(BankAccount account)
        {
            var lookupAccount = TryLookupAccount(account);
            if (lookupAccount == null)
            {
                return lookupAccount;
            }
            throw new BankException(StatusCode.FailureAccountNotFound, "Account not found");            
        }
    }
}
