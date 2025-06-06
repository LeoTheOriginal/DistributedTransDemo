using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransDemo.Models;

namespace TransDemo.Logic.Services
{
    /// <summary>
    /// Provides methods for retrieving dashboard statistics such as daily transactions, balances, branch shares, and top customers.
    /// </summary>
    public class DashboardStatsService
    {
        private readonly string _connCentral;

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardStatsService"/> class.
        /// </summary>
        /// <param name="config">The application configuration used to retrieve connection strings.</param>
        public DashboardStatsService(IConfiguration config)
        {
            _connCentral = config.GetConnectionString("CentralDB")!;
        }

        /// <summary>
        /// Retrieves daily transaction statistics for the specified number of most recent days.
        /// </summary>
        /// <param name="lastDays">The number of recent days to include in the statistics.</param>
        /// <returns>A collection of <see cref="DailyTransactionStat"/> objects representing daily transaction counts and sums.</returns>
        public async Task<IEnumerable<DailyTransactionStat>> GetStatsAsync(int lastDays)
        {
            var sql = @"
                    SELECT TOP (@Limit) *
                    FROM dbo.vDailyTransactionStats
                    ORDER BY TxnDate DESC";

            await using var conn = new SqlConnection(_connCentral);
            return await conn.QueryAsync<DailyTransactionStat>(sql, new { Limit = lastDays });
        }

        /// <summary>
        /// Retrieves daily cumulative balance statistics for the specified number of most recent days.
        /// </summary>
        /// <param name="lastDays">The number of recent days to include in the balance statistics.</param>
        /// <returns>A collection of <see cref="DailyBalanceStat"/> objects representing daily cumulative balances.</returns>
        public async Task<IEnumerable<DailyBalanceStat>> GetDailyBalanceAsync(int lastDays)
        {
            const string sql = @"
                SELECT TOP (@Limit) *
                FROM dbo.vDailyBalances
                ORDER BY BalanceDate DESC";

            await using var conn = new SqlConnection(_connCentral);
            return await conn.QueryAsync<DailyBalanceStat>(sql, new { Limit = lastDays });
        }

        /// <summary>
        /// Retrieves the transaction share percentage for each branch.
        /// </summary>
        /// <returns>A collection of <see cref="BranchTxnShare"/> objects representing branch transaction shares.</returns>
        public async Task<IEnumerable<BranchTxnShare>> GetBranchShareAsync()
        {
            const string sql = "SELECT * FROM dbo.vBranchTransactionShares";

            await using var conn = new SqlConnection(_connCentral);
            return await conn.QueryAsync<BranchTxnShare>(sql);
        }

        /// <summary>
        /// Retrieves the top customers by total transaction amount.
        /// </summary>
        /// <param name="top">The number of top customers to retrieve. Defaults to 5.</param>
        /// <returns>A collection of <see cref="TopCustomerStat"/> objects representing the top customers.</returns>
        public async Task<IEnumerable<TopCustomerStat>> GetTopCustomersAsync(int top = 5)
        {
            const string sql = @"
                SELECT TOP (@TopN) *
                FROM dbo.vTopCustomerTransactions
                ORDER BY TotalAmount DESC";

            await using var conn = new SqlConnection(_connCentral);
            return await conn.QueryAsync<TopCustomerStat>(sql, new { TopN = top });
        }
    }
}
