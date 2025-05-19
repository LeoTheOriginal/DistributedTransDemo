using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;

namespace TransDemo.Data.Repositories
{
    public class SqlHistoryRepository(string connA, string connB) : IHistoryRepository
    {
        private readonly string _connA = connA, _connB = connB;

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
    }
}
