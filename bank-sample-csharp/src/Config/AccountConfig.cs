using System.Collections.Generic;
using Tokenio.BankSample.Model;
using Tokenio.Proto.Common.AccountProtos;
using Tokenio.Proto.Common.AddressProtos;
using Tokenio.Sdk.Api;
using Tokenio.Sdk.Utils;
using static Tokenio.Proto.Common.TransactionProtos.Balance.Types;

namespace Tokenio.BankSample.Config
{
    /// <summary>
    /// A bank account configuration.
    /// </summary>
    public class AccountConfig
    {
        public string Name { get; private set; }
        public Address Address { get; private set; }
        public string Bic { get; private set; }
        public string Number { get; private set; }
        public Balance Balance { get; private set; }
        public AccountTransaction Transaction { get; private set; }

        
        /// <summary>
        /// Creates new bank account data structure.
        /// </summary>
        /// <param name="name">account legal name</param>
        /// <param name="address">account physical address</param>
        /// <param name="bic">account bic</param>
        /// <param name="number">account number</param>
        /// <param name="currency">account currency</param>
        /// <param name="balance">account balance</param>
        /// <param name="transaction">transaction</param>
        /// <returns>newly created account</returns>
        public static AccountConfig Create(
            string name,
            Address address,
            string bic,
            string number,
            string currency,
            double balance,
            AccountTransaction transaction)
        {
            return new AccountConfig(
                    name,
                    address,
                    bic,
                    number,
                    new Balance(
                            currency,
                            decimal.Parse(balance.ToString()),
                            decimal.Parse(balance.ToString()),
                            Util.EpochTimeMillis(),
                            new List<TypedBalance>()),
                    transaction);
        }

        private AccountConfig(
                    string name,
                    Address address,
                    string bic,
                    string number,
                    Balance balance,
                    AccountTransaction transaction)
        {
            this.Name = name;
            this.Address = address;
            this.Bic = bic;
            this.Number = number;
            this.Balance = balance;
            this.Transaction = transaction;
        }

        /// <summary>
        ///  Helper method to convert this object to the proto account definition.
        /// </summary>
        /// <returns>proto account definition</returns>
        public BankAccount ToBankAccount()
        {
            return new BankAccount
            {
                Swift = new BankAccount.Types.Swift
                {
                    Bic = Bic,
                    Account = Number
                }
            };
        }
    }
}
