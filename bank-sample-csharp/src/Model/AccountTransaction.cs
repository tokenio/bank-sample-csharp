using System;
using Grpc.Core.Utils;
using Io.Token.Proto.Bankapi;
using Tokenio.Proto.Common.AccountProtos;
using Tokenio.Proto.Common.TransactionProtos;
using Tokenio.Sdk.Utils;

namespace Tokenio.BankSample.Model
{
    /// <summary>
    /// Represents an account transaction. The transaction captures from, to, amount
    /// and the current status.
    /// </summary>
    public sealed class AccountTransaction
    {
        public TransactionType Type { get; private set; }
        public string Id { get; private set; }
        public string ReferenceId { get; private set; }
        public BankAccount From { get; private set; }
        public BankAccount To { get; private set; }
        public double Amount { get; private set; }
        public string Currency { get; private set; }
        public double TransferAmount { get; private set; }
        public string TransferCurrency { get; private set; }
        public string Description { get; private set; }
        public volatile StatusCode status;

        /// <summary>
        /// Creates new transaction builder.
        /// </summary>
        /// <param name="type">transaction type</param>
        /// <returns></returns>
        public static Builder NewBuilder(TransactionType type)
        {
            return new Builder(type);
        }

        /// <summary>
        /// Creates new transaction instance.
        /// </summary>
        /// <param name="type">transaction type</param>
        /// <param name="id">transaction id</param>
        /// <param name="referenceId">transaction reference id, used to capture caller transaction identifier</param>
        /// <param name="from">from / remitter account</param>
        /// <param name="to">to / beneficiary account</param>
        /// <param name="amount">transaction amount, as posted to the customer account</param>
        /// <param name="currency">transaction currency</param>
        /// <param name="transferAmount">transfer amount, could be different from customer
        ///                             amount if FX is involved</param>
        /// <param name="transferCurrency">transfer currency</param>
        /// <param name="description">transaction description</param>
        private AccountTransaction(
               TransactionType type,
               string id,
               string referenceId,
               BankAccount from,
               BankAccount to,
               double amount,
               string currency,
               double transferAmount,
               string transferCurrency,
               string description)
        {
            this.Type = type;
            this.Id = id;
            this.ReferenceId = referenceId;
            this.From = from;
            this.To = to;
            this.Amount = amount;
            this.Currency = currency;
            this.TransferAmount = transferAmount;
            this.TransferCurrency = transferCurrency;
            this.Description = description;
            this.status = StatusCode.Processing;
        }

        /// <summary>
        /// Sets transaction status.
        /// </summary>
        /// <param name="status">new transaction status</param>
        public void Status(StatusCode status)
        {
            this.status = status;
        }

        /// <summary>
        /// Converts this object to the transaction as defined by the integration
        /// API.
        /// </summary>
        /// <returns>transaction</returns>
        public Transaction ToTransaction()
        {
            return new Transaction
            {
                Id = Id,
                TokenTransferId = ReferenceId,
                Type = Type,
                Status = ProtoFactory.ToTransactionStatus(status),
                Description = Description,
                Amount = new Proto.Common.MoneyProtos.Money
                {
                    Value = Amount.ToString(),
                    Currency = Currency
                }
            };                   
        }


        /// <summary>
        /// Used to build {@link AccountTransaction} instances.
        /// </summary>
        public sealed class Builder
        {
            private readonly TransactionType type;
            private string id;
            private string referenceId;
            private BankAccount from;
            private BankAccount to;
            private double amount;
            private string currency;
            private double transferAmount;
            private string transferCurrency;
            private string description;

            /// <summary>
            /// Creates new builder.
            /// </summary>
            /// <param name="type">transaction type</param>
            internal Builder(TransactionType type)
            {
                this.type = type;
                this.description = "";
            }

            /// <summary>
            /// Sets unique transaction ID.
            /// </summary>
            /// <param name="id">transaction ID</param>
            /// <returns>this builder</returns>
            public Builder Id(string id)
            {
                this.id = id;
                return this;
            }

            /// <summary>
            /// Sets transaction reference id. Reference ID captures the external transaction ID.
            /// </summary>
            /// <param name="referenceId">reference id</param>
            /// <returns>this builder</returns>
            public Builder ReferenceId(string referenceId)
            {
                this.referenceId = referenceId;
                return this;
            }

            /// <summary>
            /// Sets from / remitter account.
            /// </summary>
            /// <param name="account">remitter account</param>
            /// <returns>this builder</returns>
            public Builder From(BankAccount account)
            {
                this.from = account;
                return this;
            }

            /// <summary>
            /// Sets to / beneficiary account.
            /// </summary>
            /// <param name="account">beneficiary account</param>
            /// <returns>this builder</returns>
            public Builder To(BankAccount account)
            {
                this.to = account;
                return this;
            }

            /// <summary>
            /// Sets transaction amount.
            /// </summary>
            /// <param name="amount">transaction amount</param>
            /// <param name="currency">transaction currency</param>
            /// <returns>this builder</returns>
            public Builder Amount(double amount, string currency)
            {
                this.amount = amount;
                this.currency = currency;
                return this;
            }

            /// <summary>
            /// Sets transfer amount. This could be different from transaction amount
            /// if FX is involved
            /// </summary>
            /// <param name="amount">transfer amount</param>
            /// <param name="currency">transaction currency</param>
            /// <returns>this builder</returns>
            public Builder TransferAmount(double amount, string currency)
            {
                this.transferAmount = amount;
                this.transferCurrency = currency;
                return this;
            }

            /// <summary>
            /// Sets transaction description.
            /// </summary>
            /// <param name="description">transaction description</param>
            /// <returns>this builder</returns>
            public Builder Description(string description)
            {
                this.description = description;
                return this;
            }

            /// <summary>
            /// Finishes building {@link AccountTransaction} instance and returns it
            /// to the caller.
            /// </summary>
            /// <returns>built transaction instance</returns>
            public AccountTransaction Build()
            {
                GrpcPreconditions.CheckArgument(amount > 0, "Amount must be set");
                GrpcPreconditions.CheckArgument(transferAmount > 0, "Transfer amount must be set");
                return new AccountTransaction(
                        type,
                        GrpcPreconditions.CheckNotNull(id, "AccountTransaction id must be set"),
                        GrpcPreconditions.CheckNotNull(referenceId, "AccountTransaction reference id must be set"),
                        GrpcPreconditions.CheckNotNull(from, "'From' account must be set"),
                        GrpcPreconditions.CheckNotNull(to, "'To' account must be set"),
                        amount,
                        GrpcPreconditions.CheckNotNull(currency, "Currency must be set"),
                        transferAmount,
                        GrpcPreconditions.CheckNotNull(transferCurrency, "Transfer currency must be set"),
                        description);
            }
        }

    }
}
