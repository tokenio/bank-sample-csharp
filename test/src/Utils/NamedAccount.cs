using System;
using Tokenio.Proto.Common.AccountProtos;
using AccountCase =
    Tokenio.Proto.Common.AccountProtos.BankAccount.AccountOneofCase;

namespace Tokenio.BankSample.Utils
{
    public class NamedAccount
    {
        private readonly string displayName;
        private readonly BankAccount bankAccount;


        public NamedAccount(BankAccount bankAccount, string displayName)
        {
            this.displayName = displayName ?? throw new NullReferenceException();
            this.bankAccount = ValidateBankAccount(bankAccount);
        }

        public NamedAccount(string bankId,
            string accountIdentifier,
            string displayName) : this(new BankAccount
            {
                Custom = new BankAccount.Types.Custom
                {
                    BankId = bankId,
                    Payload = accountIdentifier
                },
                AccountFeatures = new AccountFeatures
                {
                    SupportsInformation = true,
                    SupportsReceivePayment = true,
                    SupportsSendPayment = true
                }
            },
                displayName)
        {
        }


        public BankAccount GetBankAccount()
        {
            return bankAccount;
        }

        public string GetDisplayName()
        {
            return displayName;
        }

        private static BankAccount ValidateBankAccount(BankAccount bankAccount)
        {
            var accountCase = bankAccount.AccountCase;
            if (accountCase == AccountCase.Token ||
                accountCase == AccountCase.TokenAuthorization)
                throw new ArgumentException(
                    "Invalid account value. Token and TokenAuthorization are reserved types.");
            return bankAccount;
        }
    }
}
