// TransDemo.Data/Repositories/SqlCentralBankRepository.cs
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace TransDemo.Data.Repositories
{
    public class SqlCentralBankRepository : ICentralBankRepository
    {
        private readonly string _connCentral;
        public SqlCentralBankRepository(string connCentral) => _connCentral = connCentral;

        public void TransferCentral(int fromAccId, int toAccId, decimal amount)
        {
            using var conn = new SqlConnection(_connCentral);
            conn.Open();
            conn.Execute(
                "EXEC dbo.sp_TransferCentral @FromAccId, @ToAccId, @Amount",
                new { FromAccId = fromAccId, ToAccId = toAccId, Amount = amount });
        }
    }
}
