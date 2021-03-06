﻿using System;
using System.Globalization;
using Microsoft.Extensions.Configuration;
using Tokenio.BankSample.Config;
using Tokenio.BankSample.Model;
using Tokenio.BankSample.Services;
using Tokenio.Integration.Api.Service;
using Tokenio.Integration.Utils;
using Tokenio.Proto.Common.SecurityProtos;

namespace Tokenio.BankSample
{
    /// <summary>
    /// A factory class that is used to instantiate various services that are
    /// exposed by the gRPC server.
    /// </summary>
    public sealed class Factory
    {
        private readonly IAccounting accounting;
        private readonly IAccountLinking accountLinking;

        /// <summary>
        /// Creates new factory instance.
        /// </summary>
        /// <param name="configFilePath">path to the config directory</param>
        public Factory(string configFilePath)
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
        public IStorageService StorageService()
        {
            return new StorageServiceImpl();
        }

        /// <summary>
        /// Creates new {@link AccountService} instance.
        /// </summary>
        /// <returns>new account linking service instance</returns>
        public IAccountService AccountService()
        {
            return new AccountServiceImpl(accounting);
        }

        /// <summary>
        /// Creates new {@link AccountLinkingService} instance.
        /// </summary>
        /// <returns></returns>
        public IAccountLinkingService AccountLinkingService()
        {
            return new AccountLinkingServiceImpl(accountLinking);
        }


        /// <summary>
        /// Creates new {@link TransferService} instance.
        /// </summary>
        /// <returns>new transfer service instance</returns>
        public ITransferService TransferService()
        {
            return new TransferServiceImpl(accounting);
        }

        /// <summary>
        /// Creates new {@link AccountManagementService} instance.
        /// </summary>
        /// <returns>new account management service instance</returns>
        public IAccountManagementService AccountManagementService()
        {
            return new AccountManagementServiceImpl();
        }

        /// <summary>
        /// Creates new {@link ConsentManagementService} instance.
        /// </summary>
        /// <returns>new consent management service instance</returns>
        public IConsentManagementService ConsentManagementService()
        {
            return new ConsentManagementServiceImpl();
        }

        /// <summary>
        /// Creates new {@link NotificationService} instance.
        /// </summary>
        /// <returns>new notification service instance</returns>
        public INotificationService NotificationService()
        {
            return new NotificationServiceImpl();
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
