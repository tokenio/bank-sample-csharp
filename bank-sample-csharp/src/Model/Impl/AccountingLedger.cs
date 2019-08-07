using System;
using System.Collections.Generic;
using System.Linq;
using System.Runtime.CompilerServices;
using Tokenio.BankSample.Model.Impl;

namespace Tokenio.BankSample.Model.Impl
{
    /// <summary>
    /// Maintains ledger of transactions.
    /// </summary>
    public class AccountingLedger
    {
        private readonly IList<LedgerEntry> ledger;

        internal AccountingLedger()
        {
            this.ledger = new List<LedgerEntry>();
        }

        /// <summary>
        /// Posts a transfer to ledger. Each transfer results in two transactions
        /// posted.
        /// </summary>
        /// <param name="transfer">account transfer</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void Post(AccountTransfer transfer)
        {
            Post(new List<AccountTransfer> { transfer});
        }

        /// <summary>
        /// Posts transfers to ledger. Each transfer results in two transactions
        /// posted.
        /// </summary>
        /// <param name="transfers">account transfers</param>
        [MethodImpl(MethodImplOptions.Synchronized)]
        internal void Post(params AccountTransfer[] transfers)
        {
            Post(transfers.ToList());
        }

        /// <summary>
        /// Posts transfers to ledger. Each transfer results in two transactions
        /// posted.
        /// </summary>
        /// <param name="transfers"></param>
        private void Post(List<AccountTransfer> transfers)
        {
            foreach(AccountTransfer transfer in transfers)
            {
                Post(LedgerEntry.Debit(transfer));
                Post(LedgerEntry.Credit(transfer));
            }
        }

        /// <summary>
        /// Posts transaction to the ledger.
        /// </summary>
        /// <param name="transaction">transaction to post</param>
        private void Post(LedgerEntry transaction)
        {
            ledger.Add(transaction);
        }

    }
}
