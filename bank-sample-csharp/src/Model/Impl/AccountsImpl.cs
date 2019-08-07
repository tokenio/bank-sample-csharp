using System.Collections.Generic;
using System.Linq;
using Io.Token.Proto.Bankapi;
using Tokenio.BankSample.Config;
using Tokenio.Proto.Common.AccountProtos;
using Tokenio.Sdk.Api;
using AccountCase = Tokenio.Proto.Common.AccountProtos.BankAccount.AccountOneofCase;

namespace Tokenio.BankSample.Model.Impl
{
    /// <summary>
    /// Configuration based {@link Accounts} implementation.
    /// </summary>
    public class AccountsImpl : IAccounts
    {
        private readonly IDictionary<string, AccountConfig> holdAccounts;
        private readonly IDictionary<string, AccountConfig> fxAccounts;
        private readonly IList<AccountConfig> accounts;

        public AccountsImpl(
            IList<AccountConfig> holdAccounts,
            IList<AccountConfig> fxAccounts,
            IList<AccountConfig> customerAccounts)
        {
            this.holdAccounts = IndexAccounts(holdAccounts);
            this.fxAccounts = IndexAccounts(fxAccounts);
            this.accounts = new List<AccountConfig>(holdAccounts);
            accounts.Concat(fxAccounts).Concat(customerAccounts).ToList();
        }

        public override BankAccount GetHoldAccount(string currency)
        {
            var holdaccount = holdAccounts[currency];
            if (holdaccount != null)
            {
                return holdaccount.ToBankAccount();
            }
            
            throw new BankException(StatusCode.FailureAccountNotFound, "Hold account is not found for: " + currency);            
        }

        public override BankAccount GetFxAccount(string currency)
        {
            var fxaccount = fxAccounts[currency];
            if (fxaccount == null)
            {
                throw new BankException(StatusCode.FailureAccountNotFound, "FX account is not found for: " + currency);                
            }
            return fxaccount.ToBankAccount();

        }

        public override IList<AccountConfig> GetAllAccounts()
        {
            return accounts;
        }

        public override AccountConfig TryLookupAccount(BankAccount account)
        {
            var swift = ToSwiftAccount(account);

            return accounts.Where(a => a.Bic.Equals(swift.Bic))
                .Where(a => a.Number.Equals(swift.Account))
                .First();
        }

        private static IDictionary<string, AccountConfig> IndexAccounts(
            IList<AccountConfig> accounts)
        {
            return accounts.ToDictionary((acc) => acc.Balance.Currency);
        }

        private static BankAccount.Types.Swift ToSwiftAccount(BankAccount account)
        {
            if (account.AccountCase != AccountCase.Swift)
            {
                return new BankAccount.Types.Swift();
            }
            return account.Swift;            
        }
    }
}
