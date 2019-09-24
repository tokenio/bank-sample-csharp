using System.Collections.Generic;
using System.Linq;
using Grpc.Core;
using Tokenio.BankSample.Config;
using Tokenio.BankSample.Model;
using Tokenio.Integration.Api;
using Tokenio.Integration.Api.Service;
using Tokenio.Proto.Common.AccountProtos;
using Tokenio.Proto.Common.TransactionProtos;
using Tokenio.Proto.Common.TransferInstructionsProtos;
using Balance = Tokenio.Integration.Api.Balance;
using StatusCode = Io.Token.Proto.Bankapi.StatusCode;

namespace Tokenio.BankSample.Services
{
    /// <summary>
    /// Sample implementation of the {@link AccountService}. Returns fake data.
    /// </summary>
    public class AccountServiceImpl : IAccountService
    {
        private readonly IAccounting accounts;

        public AccountServiceImpl(IAccounting accounts)
        {
            this.accounts = accounts;
        }

        public Balance GetBalance(BankAccount account)
        {
            return accounts.LookupBalance(account)
                ?? throw new BankException(StatusCode.FailureAccountNotFound, "Account not found");
        }

        public Transaction GetTransaction(BankAccount account, string transactionId)
        {
            return accounts.LookupTransaction(account, transactionId).ToTransaction();
        }

        public PagedList<Transaction> GetTransactions(
            BankAccount account,
            string offset,
            int limit)
        {
            int cursor = DecodeCursor(offset);
            IList<Transaction> transactions = accounts
                    .LookupTransactions(account, cursor, limit)
                    .Select(transaction => transaction.ToTransaction())
                    .ToList();
            return new PagedList<Transaction>(transactions, EncodeCursor(cursor + transactions.Count));
        }

        public IList<TransferDestination> ResolveTransferDestinations(BankAccount account)
        {
            AccountConfig accountConfig = accounts
                    .LookupAccount(account)
                    ?? throw new BankException(StatusCode.FailureAccountNotFound, "Account not found");

            CustomerData customerData = new CustomerData
            {
                Address = accountConfig.Address
            };

            // Append to list of account holder names.
            // It's a list because there might be more than
            // one, e.g., for a joint account.
            // (Config test data doesn't have any joint accounts.)
            customerData.LegalNames.Add(accountConfig.Name);


            // For a bank that supports more than one way to transfer,
            // this list would have more than one item.
            // This simple sample only does Swift. But a bank
            // that supports other transfer-methods can return more.
            IList<TransferDestination> destinations = new List<TransferDestination>();
            destinations.Add(new TransferDestination
            {
                Sepa = new TransferDestination.Types.Sepa
                {
                    Bic = account.Sepa.Bic,
                    Iban = account.Sepa.Iban
                },
                CustomerData = customerData
            });

            return destinations;
        }

        public AccountDetails GetAccountDetails(BankAccount account)
        {
            throw new RpcException(new Status(Grpc.Core.StatusCode.Unimplemented,""));
        }

        private static int DecodeCursor(string encoded)
        {
            if (string.IsNullOrEmpty(encoded))
            {
                // An empty cursor indicates paging should begin at the start.
                return 0;
            }
            // Parse the cursor. The format of the string is up to the bank and is opaque to Token.
            return int.Parse(encoded);            
        }

        private static string EncodeCursor(int offset)
        {
            // Encode the cursor to return in the PagedList. This value will be passed into
            // getTransactions in subsequent requests.
            //
            // The format of the string is up to the bank and is opaque to Token; in this case we are
            // just using the string value of the cursor's position in the transaction list.
            return offset.ToString();
        }
    }
}
