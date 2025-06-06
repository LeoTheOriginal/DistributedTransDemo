using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransDemo.Data.Repositories
{
    /// <summary>
    /// Interface for distributed transfer operations between accounts.
    /// </summary>
    public interface IDistributedTransferRepository
    {
        /// <summary>
        /// Transfers a specified amount from one account to another in a distributed manner.
        /// </summary>
        /// <param name="fromAccId">The ID of the source account.</param>
        /// <param name="toAccId">The ID of the destination account.</param>
        /// <param name="amount">The amount to transfer.</param>
        void TransferDistributed(int fromAccId, int toAccId, decimal amount);
    }
}

