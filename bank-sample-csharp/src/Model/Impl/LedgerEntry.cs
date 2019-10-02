using Tokenio.Proto.Common.AccountProtos;

namespace Tokenio.BankSample.Model
{
    /// <summary>
    ///  Represents an account journal entry posted to the source and
    /// destination accounts.The change credits one account and debits
    /// the other.
    /// </summary>
    public class LedgerEntry
    {
        public string id { get; private set; }
        public string transactionId { get; private set; }
        public BankAccount account { get; private set; }
        public BankAccount counterPartyAccount { get; private set; }
        public double amount { get; private set; }
        public string currency { get; private set; }

        public LedgerEntry(string id, string transactionId, BankAccount account, BankAccount counterPartyAccount, double amount, string currency)
        {
            this.id = id;
            this.transactionId = transactionId;
            this.account = account;
            this.counterPartyAccount = counterPartyAccount;
            this.amount = amount;
            this.currency = currency;
        }

        /// <summary>
        /// Creates new debit journal entry.
        /// </summary>
        /// <param name="transfer">to extract the transaction information from</param>
        /// <returns>newly created transaction</returns>
        internal static LedgerEntry Debit(AccountTransfer transfer)
        {
            return new LedgerEntry(
                        transfer.TransferId + ":debit",
                        transfer.TransferId,
                        transfer.From,
                        transfer.To,
                        - transfer.Amount,
                        transfer.Currency);
        }

        /// <summary>
        /// Creates new credit journal entry.
        /// </summary>
        /// <param name="transfer">to extract the transaction information from</param>
        /// <returns>newly created transaction</returns>
        internal static LedgerEntry Credit(AccountTransfer transfer)
        {
            return new LedgerEntry(
                        transfer.TransferId + ":credit",
                        transfer.TransferId,
                        transfer.From,
                        transfer.To,
                        + transfer.Amount,
                        transfer.Currency);
        }
    }
}
