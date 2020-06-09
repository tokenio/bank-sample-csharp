using System;
using System.Net;
using Io.Token.Proto.Bankapi;
using Microsoft.Extensions.Configuration;
using Tokenio.BankSample.Common;
using Tokenio.BankSample.Utils;
using Tokenio.Proto.Common.AccountProtos;
using Tokenio.Proto.Common.TransferInstructionsProtos;
using Tokenio.Utils;

namespace Tokenio.BankSample.Fank
{
    public class FankTestBank
    {
        private static readonly string CURRENCY = "EUR";
        private static readonly string CLIENT_ID_KEY = "CLIENT_ID";
        private readonly FankClient fank;
        private readonly string bic;

        public FankTestBank(IConfiguration config)
            : this(new FankConfig(config))
        {
        }

        public FankTestBank(FankConfig fankConfig)
            : this(
                fankConfig.GetBic(),
                fankConfig.GetFank(),
                fankConfig.UseSsl())
        {
        }

        public FankTestBank(string bic, DnsEndPoint fank, bool useSsl)
        {
            this.bic = bic;
            this.fank = new FankClient(
                fank.Host,
                fank.Port,
                useSsl);
        }

        public NamedAccount NewAccount(Client client)
        {
            var accountName = Sample.RandomAlphaNumeric(15)
                .ToLower();
            var random = new Random();
            var bankAccountNumber = "iban:" + Util.EpochTimeMillis() +
                random.Next(0, 10000000)
                    .ToString("D7");
            fank.AddAccount(
                client,
                accountName,
                bic,
                bankAccountNumber,
                1000000.00,
                CURRENCY);
            return new NamedAccount(
                SwiftAccount(bankAccountNumber, client.Id),
                accountName);
        }


        public Client NewClient()
        {
            return fank.AddClient(bic, "Test " + RandomNumeric(15), "Testoff")
                .Result;
        }

        private BankAccount SwiftAccount(string bankAccountNumber,
            string clientId)
        {
            var account = Swift(bic, bankAccountNumber)
                .Account;
            account.Metadata.Add(CLIENT_ID_KEY, clientId);
            return account;
        }

        private TransferEndpoint Swift(string bic, string account)
        {
            return new TransferEndpoint
            {
                Account = new BankAccount
                {
                    Swift = new BankAccount.Types.Swift
                    {
                        Bic = bic,
                        Account = account
                    }
                }
            };
        }

        private string RandomNumeric(int size)
        {
            return Guid.NewGuid()
                .ToString()
                .Replace("-", string.Empty)
                .Substring(0, size);
        }
    }
}
