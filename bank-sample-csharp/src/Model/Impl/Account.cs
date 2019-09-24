using System.Collections.Generic;
using System.Linq;
using Io.Token.Proto.Bankapi;
using Tokenio.Integration.Api;
using Tokenio.Integration.Utils;
using static Tokenio.Proto.Common.TransactionProtos.Balance.Types;

namespace Tokenio.BankSample.Model
{
    /// <summary>
    /// Maintains a list of per account transactions.
    /// </summary>
    class Account
    {
        private readonly IList<AccountTransaction> transactions;
        private readonly IDictionary<string, AccountTransaction> transactionsById;
        private readonly string currency;
        private double balanceAvailable;
        private double balanceCurrent;

        internal Account(
            string currency,
            double balanceAvailable,
            double balanceCurrent,
            AccountTransaction transaction)
        {
            this.currency = currency;
            this.transactions = new List<AccountTransaction>();
            this.transactionsById = new Dictionary<string, AccountTransaction>();
            this.balanceAvailable = balanceAvailable;
            this.balanceCurrent = balanceCurrent;

            if (transaction != null)
            {
                transactions.Add(transaction);
                transactionsById.Add(transaction.Id, transaction);
            }
        }

        internal Balance GetBalance()
        {
            return Balance.Create(
                    currency,
                    decimal.Round(decimal.Parse(balanceAvailable.ToString()), 2),
                    decimal.Round(decimal.Parse(balanceCurrent.ToString()), 2),
                    Util.EpochTimeMillis(),
                    new List<TypedBalance>());
        }

        /// <summary>
        /// Adds new transaction to the account.
        /// </summary>
        /// <param name="transaction"> transaction to add</param>
        /// <returns>true if transaction has been created, false if duplicate</returns>
        public bool CreateTransaction(AccountTransaction transaction)
        {
            if (transactionsById.ContainsKey(transaction.Id))
            {
                return false;
            }

            if (transaction.Amount > balanceAvailable)
            {
                throw new TransferException(StatusCode.FailureInsufficientFunds, "Balance exceeded");
            }

            transactions.Insert(0, transaction);
            transactionsById.Add(transaction.Id, transaction);
            balanceAvailable -= transaction.Amount;
            return true;
        }

        /// <summary>
        /// Commits a transaction. Note this method is not called by Token; the specifics of when a
        /// transaction is considered complete is up to the bank and payment scheme used.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        internal AccountTransaction CommitTransaction(string transactionId)
        {
            var transaction = transactionsById.ContainsKey(transactionId) ? transactionsById[transactionId] : null;
            balanceCurrent -= transaction.Amount;
            transaction.Status(StatusCode.Success);
            return transaction;
        }

        /// <summary>
        /// Cancels a transaction. Note this method is not called by Token; the specifics of when a
        /// transaction is rejected is up to the bank and payment scheme used.
        /// </summary>
        /// <param name="transactionId"></param>
        /// <returns></returns>
        internal AccountTransaction RollbackTransaction(string transactionId)
        {
            var transaction = transactionsById.ContainsKey(transactionId) ? transactionsById[transactionId] : null;
            balanceCurrent += transaction.Amount;
            transaction.Status(StatusCode.FailureCanceled);
            return transaction;
        }

        /// <summary>
        /// Looks up a payment by ID.
        /// </summary>
        /// <param name="id">payment ID</param>
        /// <returns>looked up payment</returns>
        internal AccountTransaction LookupTransaction(string id)
        {
            return transactionsById.ContainsKey(id) ? transactionsById[id] : null;
        }

        /// <summary>
        /// Looks up multiple payments.
        /// </summary>
        /// <param name="offset">offset to start from</param>
        /// <param name="limit">max number of payments to lookup</param>
        /// <returns>list of payments</returns>
        internal IList<AccountTransaction> LookupTransactions(int offset, int limit)
        { 
            return transactions.ToList()
                .GetRange(
                    offset,
                    (offset + limit) < transactions.Count ? limit : transactions.Count);
        }
    }
}
