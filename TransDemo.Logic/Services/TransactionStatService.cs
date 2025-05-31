using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransDemo.Models;

namespace TransDemo.Logic.Services
{
    public class TransactionStatsService
    {
        private readonly string _connStr;

        public TransactionStatsService(IConfiguration config)
        {
            _connStr = config.GetConnectionString("CentralDB")!;
        }

        public async Task<IEnumerable<DailyTransactionStat>> GetDailyStatsAsync()
        {
            const string sql = @"SELECT TxnDate, TxnCount, TotalAmount FROM dbo.vDailyTransactionStats ORDER BY TxnDate";
            await using var conn = new SqlConnection(_connStr);
            return await conn.QueryAsync<DailyTransactionStat>(sql);
        }
    }
}
