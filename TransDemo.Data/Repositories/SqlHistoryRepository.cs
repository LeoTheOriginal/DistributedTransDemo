using Dapper;
using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using TransDemo.Models;

namespace TransDemo.Data.Repositories
{
    public class SqlHistoryRepository(string connCentral, string connA, string connB) : IHistoryRepository
    {
        private readonly string _connCentral =connCentral, _connA = connA, _connB = connB;

        public void AddEntryToA(string info)
        {
            using var conn = new SqlConnection(_connA);
            conn.Open();
            conn.Execute("INSERT INTO History (Info) VALUES (@info)", new { info });
        }

        public void AddEntryToB(string info)
        {
            using var conn = new SqlConnection(_connB);
            conn.Open();
            conn.Execute("INSERT INTO History (Info) VALUES (@info)", new { info });
        }
        public IEnumerable<HistoryEntry> GetHistoryFromBranch(int branchId)
        {
            var connStr = branchId == 1 ? _connA : _connB;
            using var conn = new SqlConnection(connStr);
            conn.Open();
            return conn.Query<HistoryEntry>(
                "SELECT HistoryId, Info, CreatedAt FROM dbo.History ORDER BY CreatedAt DESC");
        }

        public IEnumerable<HistoryEntry> GetCentralHistory()
        {
            using var conn = new SqlConnection(_connCentral);
            conn.Open();
            return conn.Query<HistoryEntry>(
                "SELECT HistoryId, Info, CreatedAt FROM dbo.History ORDER BY CreatedAt DESC");
        }

    }
}
