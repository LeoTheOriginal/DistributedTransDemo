using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TransDemo.Models;

namespace TransDemo.Logic.Services
{
    /// <summary>
    /// Service for retrieving transaction history from the database for individual branches.
    /// </summary>
    public class HistoryQueryService
    {
        /// <summary>
        /// Connection string for the central database.
        /// </summary>
        private readonly string _connCentral;

        /// <summary>
        /// Connection string for branch 1 database.
        /// </summary>
        private readonly string _connB1;

        /// <summary>
        /// Connection string for branch 2 database.
        /// </summary>
        private readonly string _connB2;

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryQueryService"/> class.
        /// Retrieves connection strings from the provided configuration.
        /// </summary>
        /// <param name="config">Configuration object from appsettings.json.</param>
        public HistoryQueryService(IConfiguration config)
        {
            _connCentral = config.GetConnectionString("CentralDB")!;
            _connB1 = config.GetConnectionString("Branch1DB")!;
            _connB2 = config.GetConnectionString("Branch2DB")!;
        }

        /// <summary>
        /// Retrieves the transaction history for the specified branch.
        /// </summary>
        /// <param name="branchNumber">
        /// Branch number:
        /// <list type="bullet">
        /// <item>
        /// <description>0 - Central database</description>
        /// </item>
        /// <item>
        /// <description>1 - Branch 1</description>
        /// </item>
        /// <item>
        /// <description>2 - Branch 2</description>
        /// </item>
        /// </list>
        /// </param>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains a list of <see cref="HistoryEntry"/> objects.
        /// </returns>
        /// <exception cref="ArgumentOutOfRangeException">Thrown when an unknown branch number is provided.</exception>
        public async Task<IEnumerable<HistoryEntry>> GetBranchHistoryAsync(int branchNumber)
        {
            // Select the appropriate connection string based on the branch number.
            string connStr = branchNumber switch
            {
                0 => _connCentral,
                1 => _connB1,
                2 => _connB2,
                _ => throw new ArgumentOutOfRangeException(nameof(branchNumber), "Unknown branch number")
            };

            // Open a new SQL connection using the selected connection string.
            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();

            // SQL query to retrieve history entries ordered by creation date descending.
            const string sql = @"SELECT HistoryId, Info, CreatedAt FROM History ORDER BY CreatedAt DESC";

            // Execute the query and return the results as a list of HistoryEntry objects.
            return await conn.QueryAsync<HistoryEntry>(sql);
        }
    }
}
