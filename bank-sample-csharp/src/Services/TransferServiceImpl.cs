using Io.Token.Proto.Bankapi;
using Tokenio.BankSample.Model;
using Tokenio.Sdk.Api;
using Tokenio.Sdk.Api.Service;
using TransactionType = Tokenio.Proto.Common.TransactionProtos.TransactionType;
using TransferService = Tokenio.Sdk.Api.Service.TransferService;

namespace Tokenio.BankSample.Services
{
    /// <summary>
    /// Sample implementation of the {@link TransferService}. Returns fake data.
    /// </summary>
    public class TransferServiceImpl : TransferService
    {
        private readonly IAccounting accounts;

        public TransferServiceImpl(IAccounting accounts)
        {
            this.accounts = accounts;
        }

        public override string Transfer(Transfer transfer)
        {
            Balance balance = accounts
                .LookupBalance(transfer.Account)
                ?? throw new TransferException(
                    StatusCode.FailureGeneric,
                    "Account not found: " + transfer.Account
                    );

            if (!balance.Currency.Equals(transfer.RequestedAmountCurrency))
            {
                throw new TransferException(
                        StatusCode.FailureInvalidCurrency,
                        "FX is not supported");
            }

            if (balance.Available.CompareTo(transfer.TransactionAmount) < 0)
            {
                throw new TransferException(
                        StatusCode.FailureInsufficientFunds,
                        "Balance exceeded");
            }

            AccountTransaction transaction = AccountTransaction.NewBuilder(TransactionType.Debit)
            .Id(string.Join(":", transfer.TokenTransferId, TransactionType.Debit.ToString()))
            .ReferenceId(transfer.TokenTransferId)
            .From(transfer.Account)
            .To(transfer.Destinations[0].Account)
            .Amount(
                    double.Parse(transfer.TransactionAmount.ToString()),
                    transfer.TransactionAmountCurrency)
            .TransferAmount(
                    double.Parse(transfer.TransactionAmount.ToString()),
                    transfer.TransactionAmountCurrency)
            .Description(transfer.Description)
            .Build();
            accounts.CreateDebitTransaction(transaction);

            // A bank needs to initiate a transfer here. Leaving this part out
            // since it changes from scheme to scheme.

            return transaction.Id;
        }
    }
}
