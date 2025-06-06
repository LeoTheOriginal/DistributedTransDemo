using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransDemo.Models;

namespace TransDemo.Data.Repositories
{
    /// <summary>
    /// Interface for managing history entries in different branches and the central repository.
    /// </summary>
    public interface IHistoryRepository
    {
        /// <summary>
        /// Adds a history entry to branch A.
        /// </summary>
        /// <param name="info">The information to be added as a history entry.</param>
        void AddEntryToA(string info);

        /// <summary>
        /// Adds a history entry to branch B.
        /// </summary>
        /// <param name="info">The information to be added as a history entry.</param>
        void AddEntryToB(string info);

        /// <summary>
        /// Retrieves the history entries from a specific branch.
        /// </summary>
        /// <param name="branchId">The identifier of the branch.</param>
        /// <returns>An enumerable collection of history entries from the specified branch.</returns>
        IEnumerable<HistoryEntry> GetHistoryFromBranch(int branchId);

        /// <summary>
        /// Retrieves the central history entries.
        /// </summary>
        /// <returns>An enumerable collection of central history entries.</returns>
        IEnumerable<HistoryEntry> GetCentralHistory();
    }
}
