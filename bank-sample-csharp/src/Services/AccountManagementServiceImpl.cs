using System.Collections.Generic;
using System.Collections.Immutable;
using Tokenio.Proto.Common.AccountProtos;
using Tokenio.Sdk.Api.Service;

namespace Tokenio.BankSample.Services
{
    public sealed class AccountManagementServiceImpl : IAccountManagementService
    {
        private readonly IDictionary<string, BankAccount> linkedAccounts = new Dictionary<string, BankAccount>();

        public void OnAccountLinked(
            string tokenAccountId,
            BankAccount account)
        {
            linkedAccounts.Add(tokenAccountId, account);
        }

        public void OnAccountUnlinked(
            string tokenAccountId,
            BankAccount account)
        {
            linkedAccounts.Remove(tokenAccountId);
        }

        public IDictionary<string, BankAccount> GetLinkedAccounts()
        {
            return linkedAccounts.ToImmutableDictionary();
        }
    }
}