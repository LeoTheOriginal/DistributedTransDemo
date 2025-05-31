using System.Collections.Generic;
using System.Threading.Tasks;
using Dapper;
using Microsoft.Data.SqlClient;
using Microsoft.Extensions.Configuration;
using TransDemo.Models;

namespace TransDemo.Logic.Services
{
    /// <summary>
    /// Serwis do pobierania historii transakcji z bazy danych dla poszczególnych oddziałów.
    /// </summary>
    public class HistoryQueryService
    {
        private readonly string _connCentral;
        private readonly string _connB1;
        private readonly string _connB2;

        /// <summary>
        /// Konstruktor przyjmujący IConfiguration, z którego pobierane są connection stringi.
        /// </summary>
        /// <param name="config">Obiekt konfiguracji z pliku appsettings.json</param>
        public HistoryQueryService(IConfiguration config)
        {
            _connCentral = config.GetConnectionString("CentralDB")!;
            _connB1 = config.GetConnectionString("Branch1DB")!;
            _connB2 = config.GetConnectionString("Branch2DB")!;
        }

        /// <summary>
        /// Pobiera historię transakcji dla wskazanego oddziału.
        /// </summary>
        /// <param name="branchNumber">Numer oddziału (1 lub 2).</param>
        /// <returns>Lista wpisów historii.</returns>
        public async Task<IEnumerable<HistoryEntry>> GetBranchHistoryAsync(int branchNumber)
        {
            string connStr = branchNumber switch
            {
                0 => _connCentral,
                1 => _connB1,
                2 => _connB2,
                _ => throw new ArgumentOutOfRangeException(nameof(branchNumber), "Nieznany numer oddziału")
            };

            await using var conn = new SqlConnection(connStr);
            await conn.OpenAsync();
            const string sql = @"SELECT HistoryId, Info, CreatedAt FROM History ORDER BY CreatedAt DESC";
            return await conn.QueryAsync<HistoryEntry>(sql);
        }


    }
}
