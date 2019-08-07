using System.Collections.Generic;
using Tokenio.BankSample.Config;
using Tokenio.Proto.Common.AccountProtos;
using Tokenio.Sdk.Api;

namespace Tokenio.BankSample.Model
{
    /// <summary>
    /// AccountTransaction accounting service.Abstracts away bank account data store.
    /// </summary>
    public interface IAccounting
    {
        /// <summary>
        /// Looks up account information.
        /// </summary>
        /// <param name="bankAccount">account to lookup the info for</param>
        /// <returns>account info</returns>
        AccountConfig LookupAccount(BankAccount bankAccount);

        /// <summary>
        /// Looks up account balance.
        /// </summary>
        /// <param name="account"> account to lookup the balance for</param>
        /// <returns>account balance if found</returns>
        Balance LookupBalance(BankAccount account);

        /// <summary>
        /// Creates a new transaction.
        /// </summary>
        /// <param name="transaction">new transaction</param>
        void CreateDebitTransaction(AccountTransaction transaction);

        /// <summary>
        /// Looks up transaction given the account and transaction ID.
        /// </summary>
        /// <param name="account">account to lookup the transaction for</param>
        /// <param name="transactionId">transaction id</param>
        /// <returns>looked up transaction if found</returns>
        AccountTransaction LookupTransaction(BankAccount account, string transactionId);

        /// <summary>
        /// Looks up transactions for the given account.
        /// </summary>
        /// <param name="account">account to lookup the transactions for</param>
        /// <param name="offset">the result offset</param>
        /// <param name="limit">the limit on the number of results returned</param>
        /// <returns>list of looked up transactions</returns>
        IList<AccountTransaction> LookupTransactions(BankAccount account, int offset, int limit);
    }
}
