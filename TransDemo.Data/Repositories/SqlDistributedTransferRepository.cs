using Microsoft.Data.SqlClient;
using System;
using System.Collections.Generic;
using System.Data;
using System.Linq;
using System.Text;
using System.Threading.Tasks;
using Dapper;


namespace TransDemo.Data.Repositories
{
    public class SqlDistributedTransferRepository : IDistributedTransferRepository
    {
        private readonly string _connCentral;
        public SqlDistributedTransferRepository(string connCentral) => _connCentral = connCentral;

        public void TransferDistributed(int fromAccId, int toAccId, decimal amount)
        {
            using IDbConnection conn = new SqlConnection(_connCentral);
            conn.Open();
            conn.Execute(
                "EXEC dbo.sp_TransferDistributed @FromAccId, @ToAccId, @Amount",
                new { FromAccId = fromAccId, ToAccId = toAccId, Amount = amount });
        }
    }
}
