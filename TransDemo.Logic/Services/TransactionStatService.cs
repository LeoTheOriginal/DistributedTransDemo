using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransDemo.Models;

namespace TransDemo.Logic.Services
{
    /// <summary>
    /// Service responsible for retrieving transaction statistics from the database.
    /// </summary>
    public class TransactionStatsService
    {
        private readonly string _connStr;

        /// <summary>
        /// Initializes a new instance of the <see cref="TransactionStatsService"/> class.
        /// </summary>
        /// <param name="config">The application configuration used to retrieve the connection string.</param>
        public TransactionStatsService(IConfiguration config)
        {
            _connStr = config.GetConnectionString("CentralDB")!;
        }

        /// <summary>
        /// Asynchronously retrieves daily transaction statistics from the database.
        /// </summary>
        /// <returns>
        /// A task that represents the asynchronous operation. The task result contains an enumerable collection of <see cref="DailyTransactionStat"/>.
        /// </returns>
        public async Task<IEnumerable<DailyTransactionStat>> GetDailyStatsAsync()
        {
            const string sql = @"SELECT TxnDate, TxnCount, TotalAmount FROM dbo.vDailyTransactionStats ORDER BY TxnDate";
            await using var conn = new SqlConnection(_connStr);
            return await conn.QueryAsync<DailyTransactionStat>(sql);
        }
    }
}
