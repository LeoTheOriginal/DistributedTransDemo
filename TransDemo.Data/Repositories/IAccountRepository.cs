using System.Collections.Generic;
using TransDemo.Models;

namespace TransDemo.Data.Repositories
{
    /// <summary>
    /// Defines methods for accessing and modifying account data.
    /// </summary>
    public interface IAccountRepository
    {
        /// <summary>
        /// Retrieves all accounts.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="Account"/> objects.</returns>
        IEnumerable<Account> GetAll();

        /// <summary>
        /// Debits the specified amount from the account with the given ID.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account.</param>
        /// <param name="amount">The amount to debit.</param>
        void Debit(int accountId, decimal amount);

        /// <summary>
        /// Credits the specified amount to the account with the given ID.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account.</param>
        /// <param name="amount">The amount to credit.</param>
        void Credit(int accountId, decimal amount);
    }
}
