// TransDemo.Logic/Services/TransferService.cs
using System;
using System.Collections.Generic;
using System.Transactions;
using TransDemo.Data.Repositories;
using TransDemo.Models;

namespace TransDemo.Logic.Services
{
    /// <summary>
    /// Service responsible for handling distributed transfer operations between accounts.
    /// </summary>
    public class TransferService
    {
        private readonly IDistributedTransferRepository _distributed;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransferService"/> class.
        /// </summary>
        /// <param name="distributed">The distributed transfer repository used to perform transfer operations.</param>
        public TransferService(IDistributedTransferRepository distributed)
        {
            _distributed = distributed;
        }

        /// <summary>
        /// Executes a distributed transfer between two accounts.
        /// </summary>
        /// <param name="from">The source account from which the amount will be debited.</param>
        /// <param name="to">The destination account to which the amount will be credited.</param>
        /// <param name="amount">The amount to transfer.</param>
        /// <param name="simulateError">If set to <c>true</c>, simulates an error by throwing an exception.</param>
        /// <exception cref="InvalidOperationException">Thrown when <paramref name="simulateError"/> is <c>true</c>.</exception>
        public void ExecuteDistributedTransfer(Account from, Account to, decimal amount, bool simulateError = false)
        {
            if (simulateError)
                throw new InvalidOperationException("Symulowany błąd.");
            // TransactionScope could be used here, but since the entire operation is handled in a stored procedure, it is not necessary.
            _distributed.TransferDistributed(from.AccountId, to.AccountId, amount);
        }
    }

}
    