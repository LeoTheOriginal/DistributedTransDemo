using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransDemo.Models;

namespace TransDemo.Logic.Services
{
    public class DashboardStatsService
    {
        private readonly string _connCentral;

        public DashboardStatsService(IConfiguration config)
        {
            _connCentral = config.GetConnectionString("CentralDB")!;
        }

        // 1. Dzienna liczba transakcji i suma kwot
        public async Task<IEnumerable<DailyTransactionStat>> GetStatsAsync(int lastDays)
        {
            var sql = @"
                SELECT TOP (@Limit) *
                FROM dbo.vDailyTransactionStats
                ORDER BY TxnDate DESC";

            await using var conn = new SqlConnection(_connCentral);
            return await conn.QueryAsync<DailyTransactionStat>(sql, new { Limit = lastDays });
        }

        // 2. Saldo dzienne (narastające)
        public async Task<IEnumerable<DailyBalanceStat>> GetDailyBalanceAsync(int lastDays)
        {
            const string sql = @"
            SELECT TOP (@Limit) *
            FROM dbo.vDailyBalances
            ORDER BY BalanceDate DESC";

            await using var conn = new SqlConnection(_connCentral);
            return await conn.QueryAsync<DailyBalanceStat>(sql, new { Limit = lastDays });
        }

        public async Task<IEnumerable<BranchTxnShare>> GetBranchShareAsync()
        {
            const string sql = "SELECT * FROM dbo.vBranchTransactionShares";

            await using var conn = new SqlConnection(_connCentral);
            return await conn.QueryAsync<BranchTxnShare>(sql);
        }

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
