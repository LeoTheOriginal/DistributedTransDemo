// TransDemo.Data/Repositories/ICentralBankRepository.cs
using System;

namespace TransDemo.Data.Repositories
{
    public interface ICentralBankRepository
    {
        /// <summary>
        /// Przekazuje środki między rachunkami centralnymi za pomocą sp_TransferCentral.
        /// Rzuca wyjątek, jeśli np. brak środków.
        /// </summary>
        void TransferCentral(int fromAccId, int toAccId, decimal amount);
    }
}
