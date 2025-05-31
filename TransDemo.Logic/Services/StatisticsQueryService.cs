using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using System.Collections.Generic;
using System.Threading.Tasks;
using TransDemo.Models;

public class StatisticsQueryService
{
    private readonly string _connCentral;

    public StatisticsQueryService(IConfiguration config)
    {
        _connCentral = config.GetConnectionString("CentralDB")!;
    }

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
