using Dapper;
using Microsoft.Data.SqlClient;
using System.Collections.Generic;
using TransDemo.Models;

namespace TransDemo.Data.Repositories
{
    /// <summary>
    /// Repository for managing history entries in central and branch databases.
    /// </summary>
    public class SqlHistoryRepository : IHistoryRepository
    {
        private readonly string _connCentral;
        private readonly string _connA;
        private readonly string _connB;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlHistoryRepository"/> class.
        /// </summary>
        /// <param name="connCentral">Connection string to the central database.</param>
        /// <param name="connA">Connection string to branch A database.</param>
        /// <param name="connB">Connection string to branch B database.</param>
        public SqlHistoryRepository(string connCentral, string connA, string connB)
        {
            _connCentral = connCentral;
            _connA = connA;
            _connB = connB;
        }

        /// <summary>
        /// Adds a new history entry to branch A database.
        /// </summary>
        /// <param name="info">The information to be added to the history.</param>
        public void AddEntryToA(string info)
        {
            using var conn = new SqlConnection(_connA);
            conn.Open();
            conn.Execute("INSERT INTO dbo.History (Info) VALUES (@info)", new { info });
        }

        /// <summary>
        /// Adds a new history entry to branch B database.
        /// </summary>
        /// <param name="info">The information to be added to the history.</param>
        public void AddEntryToB(string info)
        {
            using var conn = new SqlConnection(_connB);
            conn.Open();
            conn.Execute("INSERT INTO dbo.History (Info) VALUES (@info)", new { info });
        }

        /// <summary>
        /// Retrieves the history entries from the specified branch.
        /// </summary>
        /// <param name="branchId">The branch identifier (1 for A, otherwise B).</param>
        /// <returns>A collection of <see cref="HistoryEntry"/> from the branch database.</returns>
        public IEnumerable<HistoryEntry> GetHistoryFromBranch(int branchId)
        {
            var connStr = branchId == 1 ? _connA : _connB;
            using var conn = new SqlConnection(connStr);
            conn.Open();
            return conn.Query<HistoryEntry>(
                "SELECT HistoryId, Info, CreatedAt FROM dbo.History ORDER BY CreatedAt DESC");
        }

        /// <summary>
        /// Retrieves the history entries from the central database.
        /// </summary>
        /// <returns>A collection of <see cref="HistoryEntry"/> from the central database.</returns>
        public IEnumerable<HistoryEntry> GetCentralHistory()
        {
            using var conn = new SqlConnection(_connCentral);
            conn.Open();
            return conn.Query<HistoryEntry>(
                "SELECT HistoryId, Info, CreatedAt FROM dbo.History ORDER BY CreatedAt DESC");
        }
    }
}
