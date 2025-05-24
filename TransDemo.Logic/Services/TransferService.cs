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
        private readonly ICentralBankRepository _central;
        private readonly IDictionary<int, IBranchClientRepository> _branches;

        /// <param name="branches">
        ///   Mapa: BranchId → repozytorium oddziału (BranchClients).
        ///   BranchId pobieramy z encji Account.BranchId.
        /// </param>
        public TransferService(
            ICentralBankRepository central,
            IDictionary<int, IBranchClientRepository> branches)
        {
            _central = central;
            _branches = branches;
        }

        public void ExecuteDistributedTransfer(
            Account from, Account to,
            decimal amount,
            bool simulateError = false)
        {
            var opts = new TransactionOptions
            {
                IsolationLevel = IsolationLevel.ReadCommitted
            };

            using var scope = new TransactionScope(
                TransactionScopeOption.Required,
                opts,
                TransactionScopeAsyncFlowOption.Enabled);

            // 1) Centralny transfer sp_TransferCentral
            _central.TransferCentral(from.AccountId, to.AccountId, amount);

            if (simulateError)
                throw new InvalidOperationException("Symulowany błąd po centralnym opłaceniu.");

            // 2) Lokalny oddział: obciążenie i zapis historii
            _branches[from.BranchId]
                .Debit(from.CustomerId, amount, "Transfer z konta centralnego");

            // 3) Lokalny oddział docelowy: uznanie i zapis historii
            _branches[to.BranchId]
                .Credit(to.CustomerId, amount, "Transfer do konta centralnego");

            // 4) Commit 2PC
            scope.Complete();
        }
    }
}
