using System;
using Microsoft.Extensions.Configuration;
using Tokenio.BankSample.Config;
using Tokenio.BankSample.Model;
using Tokenio.BankSample.Model.Impl;
using Tokenio.BankSample.Services;
using Tokenio.Proto.Common.SecurityProtos;
using Tokenio.Sdk.Api.Service;
using System.Globalization;
using Tokenio.Sdk.Utils;

namespace Tokenio.BankSample
{
    /// <summary>
    /// A factory class that is used to instantiate various services that are
    /// exposed by the gRPC server.
    /// </summary>
    sealed class Factory
    {
        private readonly IAccounting accounting;
        private readonly IAccountLinking accountLinking;

        /// <summary>
        /// Creates new factory instance.
        /// </summary>
        /// <param name="configFilePath">path to the config directory</param>
        internal Factory(string configFilePath)
        {
            ConfigParser config = new ConfigParser(new ConfigurationBuilder()
                                .AddJsonFile(configFilePath)
                                .Build());
            IAccounts accounts = new AccountsImpl(
                    config.HoldAccounts(),
                    config.FxAccounts(),
                    config.CustomerAccounts());

            BankAccountAuthorizer authorizer = BankAccountAuthorizer.NewBuilder(config.BankId())
                    .WithSecretKeystore(config.SecretKeyStore())
                    .WithTrustedKeystore(config.TrustedKeyStore())
                    .UseKey(config.EncryptionKeyId())
                    .UseMethod((SealedMessage.MethodOneofCase)Enum.Parse(typeof(SealedMessage.MethodOneofCase),ToTitleCase(config.EncryptionMethod())))
                    // expiration is set to 1 day by default
                    .Build();

            this.accounting = new AccountingImpl(accounts);
            this.accountLinking = new AccountLinkingImpl(
                    authorizer,
                    config.AccessTokenAuthorizations());
        }

        /// <summary>
        /// Creates new {@link StorageService} instance.
        /// </summary>
        /// <returns>new storage service instance</returns>
        internal IStorageService StorageService()
        {
            return new StorageServiceImpl();
        }

        /// <summary>
        /// Creates new {@link AccountService} instance.
        /// </summary>
        /// <returns>new account linking service instance</returns>
        internal IAccountService AccountService()
        {
            return new AccountServiceImpl(accounting);
        }

        /// <summary>
        /// Creates new {@link AccountLinkingService} instance.
        /// </summary>
        /// <returns></returns>
        internal IAccountLinkingService AccountLinkingService()
        {
            return new AccountLinkingServiceImpl(accountLinking);
        }


        /// <summary>
        /// Creates new {@link TransferService} instance.
        /// </summary>
        /// <returns>new transfer service instance</returns>
        internal ITransferService TransferService()
        {
            return new TransferServiceImpl(accounting);
        }

        private static string ToTitleCase(string str)
        {
            TextInfo tesxtInfo = new CultureInfo("en-US", false).TextInfo;
            str = tesxtInfo.ToTitleCase(str.ToLower());
            str = str.Replace("_", "");
            return str;
        }
    }
}
