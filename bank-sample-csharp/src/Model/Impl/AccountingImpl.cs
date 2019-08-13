using System.Collections.Generic;
using System.Runtime.CompilerServices;
using Tokenio.BankSample.Config;
using Tokenio.Proto.Common.AccountProtos;
using Balance = Tokenio.Sdk.Api.Balance;
using Tokenio.Proto.Common.TransactionProtos;

namespace Tokenio.BankSample.Model.Impl
{
	/// <summary>
	/// Configuration based account service implementation.
	/// </summary>
	public sealed class AccountingImpl : IAccounting
    {
        private readonly IAccounts config;
        private readonly IDictionary<AccountConfig, Account> accounts;
        private readonly AccountingLedger ledger;

        public AccountingImpl(IAccounts config)
        {
            this.config = config;
            IDictionary<AccountConfig, Account> map = new Dictionary<AccountConfig, Account>();
            foreach (var account in config.GetAllAccounts())
            {
                map.Add(account, new Account(
                                    account.Balance.Currency,
                                    double.Parse(account.Balance.Available.ToString()),
                                    double.Parse(account.Balance.Current.ToString()))
                                    );
            }
            this.accounts = map;
            this.ledger = new AccountingLedger();
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public AccountConfig LookupAccount(BankAccount account) {
            return config.TryLookupAccount(account);
        }


        [MethodImpl(MethodImplOptions.Synchronized)]
        public Balance LookupBalance(BankAccount account) {
            var acc = config.TryLookupAccount(account);
            if (accounts.ContainsKey(acc) && accounts[acc] != null)
            {
                return accounts[acc].GetBalance();
            }
            return null;
		}

        [MethodImpl(MethodImplOptions.Synchronized)]
        public void CreateDebitTransaction(AccountTransaction transaction)
        {
            if (transaction.Type == TransactionType.Debit)
                if (!CreateTransaction(transaction))
                {
                    return;
                }

            if (transaction.Currency.Equals(transaction.TransferCurrency))
            {
                // If FX is not needed, just move the money to the holding account.
                ledger.Post(AccountTransfer.NewBuilder()
                        .From(transaction.From)
                        .To(config.GetHoldAccount(transaction.Currency))
                        .WithAmount(
                                transaction.Amount,
                                transaction.Currency)
                        .Build());
            }
            else
            {
                // With FX.
                // Create two transfers to account for FX.
                // 1) DB customer, credit FX in the customer account currency.
                // 2) DB FX, credit hold account in the settlement account currency.
                // Note that we are not accounting for the spread with this
                // transaction pair, it goes 'nowhere'.
                ledger.Post(
                        AccountTransfer.NewBuilder()
                                .From(transaction.From)
                                .To(config.GetFxAccount(transaction.Currency))
                                .WithAmount(
                                        transaction.Amount,
                                        transaction.Currency)
                                .Build(),
                        AccountTransfer.NewBuilder()
                                .From(config.GetFxAccount(transaction.TransferCurrency))
                                .To(config.GetHoldAccount(transaction.TransferCurrency))
                                .WithAmount(
                                        transaction.TransferAmount,
                                        transaction.TransferCurrency)
                                .Build());
            }
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public AccountTransaction LookupTransaction(
            BankAccount account,
            string transactionId) {
            var acc = config.TryLookupAccount(account);
            if (accounts.ContainsKey(acc) && accounts[acc] != null)
            {
                return accounts[acc].LookupTransaction(transactionId);
            }
            return null;            
        }

        [MethodImpl(MethodImplOptions.Synchronized)]
        public IList<AccountTransaction> LookupTransactions(
            BankAccount account,
            int offset,
            int limit) {
            var acc = config.LookupAccount(account);
            if (accounts.ContainsKey(acc) && accounts[acc] != null)
            {
                return accounts[acc].LookupTransactions(offset, limit);
            }
            return new List<AccountTransaction>();            
        }

        private bool CreateTransaction(AccountTransaction transaction)
        {
            return accounts[config.LookupAccount(transaction.From)]
                    .CreateTransaction(transaction);
        }
    }
}
