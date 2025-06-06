using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransDemo.Models;

/// <summary>
/// Provides methods for querying transaction statistics from the central database.
/// </summary>
public class StatisticsQueryService
{
    private readonly string _connCentral;

    /// <summary>
    /// Initializes a new instance of the <see cref="StatisticsQueryService"/> class.
    /// </summary>
    /// <param name="config">The application configuration used to retrieve connection strings.</param>
    public StatisticsQueryService(IConfiguration config)
    {
        _connCentral = config.GetConnectionString("CentralDB")!;
    }

    /// <summary>
    /// Retrieves daily transaction statistics for the last specified number of days.
    /// </summary>
    /// <param name="lastNDays">The number of most recent days to include in the statistics.</param>
    /// <returns>
    /// An asynchronous task that returns an <see cref="IEnumerable{DailyTransactionStat}"/> containing the daily statistics.
    /// </returns>
    public async Task<IEnumerable<DailyTransactionStat>> GetDailyStatsAsync(int lastNDays)
    {
        var sql = @"
            SELECT TxnDate, TxnCount, TotalAmount
            FROM dbo.vDailyTransactionStats
            WHERE TxnDate >= CAST(GETDATE() - @days AS DATE)
            ORDER BY TxnDate";

        await using var conn = new SqlConnection(_connCentral);
        return await conn.QueryAsync<DailyTransactionStat>(sql, new { days = lastNDays });
    }
}
