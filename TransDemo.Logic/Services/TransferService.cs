// TransDemo.Logic/Services/TransferService.cs
using System;
using System.Collections.Generic;
using System.Transactions;
using TransDemo.Data.Repositories;
using TransDemo.Models;

namespace TransDemo.Logic.Services
{
    public class TransferService
    {
        private readonly IDistributedTransferRepository _distributed;
        public TransferService(IDistributedTransferRepository distributed)
        {
            _distributed = distributed;
        }

        public void ExecuteDistributedTransfer(Account from, Account to, decimal amount, bool simulateError = false)
        {
            if (simulateError)
                throw new InvalidOperationException("Symulowany błąd.");
            // …ewentualnie jeszcze TransactionScope, ale skoro całość jest w SP, nie jest już potrzebny  
            _distributed.TransferDistributed(from.AccountId, to.AccountId, amount);
        }
    }

}
