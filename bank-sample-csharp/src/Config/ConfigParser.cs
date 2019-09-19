using System;
using System.Collections.Generic;
using System.Linq;
using Microsoft.Extensions.Configuration;
using Tokenio.BankSample.Model;
using Tokenio.Proto.Common.AccountProtos;
using Tokenio.Proto.Common.AddressProtos;
using Tokenio.Proto.Common.TransactionProtos;
using Tokenio.Sdk.Api;
using Tokenio.Sdk.Security;

namespace Tokenio.BankSample.Config
{
    /// <summary>
    /// Parses configuration file and extracts different pieces of configuration.
    /// </summary>
    public sealed class ConfigParser
    {
        private readonly IConfiguration config;

        /// <summary>
        /// Creates new configuration object.
        /// </summary>
        /// <param name="config">config to parse</param>
        public ConfigParser(IConfiguration config)
        {
            this.config = config;
        }

        /// <summary>
        /// Extracts list of accounts from the config.
        /// </summary>
        /// <returns>list of configured accounts</returns>
        public IList<AccountConfig> CustomerAccounts()
        {
            return AccountsFor("customers");
        }

        /// <summary>
        /// Extracts hold accounts list.
        /// </summary>
        /// <returns>hold accounts</returns>
        public IList<AccountConfig> HoldAccounts()
        {
            return AccountsFor("hold");
        }

        /// <summary>
        /// FX accounts account list.
        /// </summary>
        /// <returns>FX accounts</returns>
        public IList<AccountConfig> FxAccounts()
        {
            return AccountsFor("fx");
        }

        /// <summary>
        /// Extracts bank id from config
        /// </summary>
        /// <returns>bank id</returns>
        public string BankId()
        {
            return config["bank:bank-id"];
        }

        /// <summary>
        /// Extracts the secret key store for generating bank authorization payload
        /// for account linking.
        /// </summary>
        /// <returns>SecretKeyStore</returns>
        public ISecretKeyStore SecretKeyStore()
        {
            return KeyStoreFactory.CreateSecretKeyStore(
                    config.GetSection("account-linking:secret-key-store").GetChildren().ToList());
        }

        /// <summary>
        /// Extracts the trusted key store for generating bank authorization payload
        /// for account linking
        /// </summary>
        /// <returns>TrustedKeyStore</returns>
        public ITrustedKeyStore TrustedKeyStore()
        {
            return KeyStoreFactory.CreateTrustedKeyStore(
                   config.GetSection("account-linking:trusted-key-store").GetChildren().ToList());
        }

        /// <summary>
        /// Extracts the id of the key to be used for encryption for account linking.
        /// </summary>
        /// <returns>encryption key id</returns>
        public string EncryptionKeyId()
        {
            return config["account-linking:encryption:encryption-key-id"];
        }

        /// <summary>
        /// Extracts the encryption method for account linking.
        /// </summary>
        /// <returns>encryption method</returns>
        public string EncryptionMethod()
        {
            return config["account-linking:encryption:encryption-method"];
        }

        /// <summary>
        /// Extracts map of access token string to access token authorization object.
        /// </summary>
        /// <returns>access token authorization map</returns>
        public IDictionary<string, AccessTokenAuthorization> AccessTokenAuthorizations()
        {
            return config.GetSection("access-tokens")
                .GetChildren()
                .ToList()
                .Select(x =>
                {
                    IList<NamedAccount> namedAccounts = x.GetSection("accounts")
                        .GetChildren()
                        .ToList()
                        .Select(number =>
                        {
                var accounts = CustomerAccounts()
               .Where(acc => acc.Number == number.Value).ToList();
                return accounts.Count == 1
                ? ToNamedAccount(accounts[0])
                : throw new ArgumentException("Zero or multiple accounts match "
                                                + "the account number "
                                                + number);

            }).ToList();
                    return new AccessTokenAuthorization(
                                    x["access-token"],
                                    x["member-id"],
                                    namedAccounts);
                }).ToDictionary(auth => auth.AccessToken);
        }


        private IList<AccountConfig> AccountsFor(string category)
        {

            return config.GetSection("accounts:" + category)
                    .GetChildren()
                    .ToList()
                    .Select(x =>
                    {
                        Address address = new Address();
                        var addressConfig = x.GetSection("address");
                        if (addressConfig.Value != null)
                        {
                            address = new Address
                            {
                                HouseNumber = addressConfig["house"],
                                Street = addressConfig["street"],
                                City = addressConfig["city"],
                                PostCode = addressConfig["post_code"],
                                Country = addressConfig["country"]
                            };
                        }
                        AccountTransaction transaction = null;

                        if (category.Equals("customers"))
                        {
                            var transactionConfig = x.GetSection("transaction");
                            TransactionType type = TransactionType.InvalidType;
                            if (transactionConfig["transaction-type"] == "DEBIT")
                                type = TransactionType.Debit;
                            else if (transactionConfig["transaction-type"] == "CREDIT")
                                type = TransactionType.Credit;
                            transaction = AccountTransaction.NewBuilder(type)
                            .Amount(Double.Parse(transactionConfig["amount"]), transactionConfig["currency"])
                            .TransferAmount(Double.Parse(transactionConfig["amount"]), transactionConfig["currency"])
                            .Description(transactionConfig["description"])
                            .From(new BankAccount
                            {
                                Swift = new BankAccount.Types.Swift
                                {
                                    Bic = x["bic"],
                                    Account = x["number"]
                                }
                            })
                            .To(new BankAccount
                            {
                                Swift = new BankAccount.Types.Swift
                                {
                                    Bic = transactionConfig["to:bic"],
                                    Account = transactionConfig["to:number"]
                                }
                            })
                            .Id(transactionConfig["id"])
                            .ReferenceId(transactionConfig["id"])
                            .Build();
                        }

                        double balance = x["balance"] != null
                        ? double.Parse(x["balance"])
                        : 0;

                        return AccountConfig.Create(
                            x["name"],
                            address,
                            x["bic"],
                            x["number"],
                            x["currency"],
                            balance,
                            transaction);
                    }).ToList();
        }

        private static NamedAccount ToNamedAccount(AccountConfig accountConfig)
        {
            return new NamedAccount(
                    new BankAccount
                    {
                        Swift = new BankAccount.Types.Swift
                        {
                            Bic = accountConfig.Bic,
                            Account = accountConfig.Number
                        },
                        AccountFeatures = new AccountFeatures
                        {
                            SupportsPayment = true,
                            SupportsReceivePayment = true,
                            SupportsSendPayment = true,
                            SupportsInformation = true
                        }
                    },
                    accountConfig.Name);
        }
    }
}
