using System;
using Tokenio.Proto.Common.AccountProtos;
using Tokenio.Proto.Common.MoneyProtos;

namespace Tokenio.BankSample.Model.Impl
{
    /// <summary>
    ///  Represents a transaction posted to the source and destination accounts. The
    /// change credits one account and debits the other.
    /// </summary>
    public class AccountTransfer
    {
        public string TransferId { get; private set; }
        public BankAccount From { get; private set; }
        public BankAccount To { get; private set; }
        public double Amount { get; private set; }
        public string Currency { get; private set; }

        public AccountTransfer(string transferId,
                            BankAccount from,
                            BankAccount to,
                            double amount,
                            string currency)
        {
            this.TransferId = transferId;
            this.From = from;
            this.To = to;
            this.Amount = amount;
            this.Currency = currency;
        }

        /// <summary>
        /// Creates a new {@link Builder} that is used to create
        /// {@link AccountTransfer} instances.
        /// </summary>
        /// <returns></returns>
        public static Builder NewBuilder()
        {
            return new Builder();
        }

        public class Builder
        {
            private string transferId;
            private BankAccount from;
            private BankAccount to;
            private double amount;
            private string currency;

            internal Builder()
            {
                TransferId(Guid.NewGuid().ToString());
            }

            /// <summary>
            /// Sets unique transfer id.
            /// </summary>
            /// <param name="transferId">transfer id</param>
            /// <returns>this object</returns>
            public Builder TransferId(string transferId)
            {
                this.transferId = transferId;
                return this;
            }

            /// <summary>
            /// Sets source/from account.
            /// </summary>
            /// <param name="from">from account</param>
            /// <returns>this object</returns>
            public Builder From(BankAccount from)
            {
                this.from = from;
                return this;
            }

            /// <summary>
            /// Sets destination/to account.
            /// </summary>
            /// <param name="to">to account</param>
            /// <returns>this object</returns>
            public Builder To(BankAccount to)
            {
                this.to = to;
                return this;
            }

            /// <summary>
            /// Sets transfer amount.
            /// </summary>
            /// <param name="amount">transfer amount</param>
            /// <param name="currency">transfer currency</param>
            /// <returns>this object</returns>
            public Builder WithAmount(double amount, string currency)
            {
                this.amount = amount;
                this.currency = currency;
                return this;
            }

            /// <summary>
            /// Sets transfer amount.
            /// </summary>
            /// <param name="amount">transfer amount</param>
            /// <returns>this object</returns>
            public Builder WithAmount(Money amount)
            {
                this.amount = double.Parse(amount.Value);
                this.currency = amount.Currency;
                return this;
            }

            /// <summary>
            /// Creates new {@link AccountTransfer}.
            /// </summary>
            /// <returns>newly created {@link AccountTransfer}</returns>
            public AccountTransfer Build()
            {
                if(amount <= 0)
                {
                    throw new ArgumentException("Amount must be set");
                }

                transferId = transferId ?? throw new NullReferenceException("Transfer id must be set");
                from = from ?? throw new NullReferenceException("Source account must be set");
                to = to ?? throw new NullReferenceException("Destination account must be set");
                currency = currency ?? throw new NullReferenceException("Currency must be set");
                return new AccountTransfer(
                        transferId,
                        from,
                        to, 
                        amount,
                        currency);
            }
        }
    }
}
