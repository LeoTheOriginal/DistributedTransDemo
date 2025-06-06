// TransDemo.Data/Repositories/SqlCentralBankRepository.cs
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;

namespace TransDemo.Data.Repositories
{
    /// <summary>
    /// Repository for central bank operations using SQL Server.
    /// </summary>
    public class SqlCentralBankRepository : ICentralBankRepository
    {
        private readonly string _connCentral;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlCentralBankRepository"/> class.
        /// </summary>
        /// <param name="connCentral">The connection string to the central bank database.</param>
        public SqlCentralBankRepository(string connCentral) => _connCentral = connCentral;

        /// <summary>
        /// Transfers funds between central accounts using the stored procedure sp_TransferCentral.
        /// Throws an exception if, for example, there are insufficient funds.
        /// </summary>
        /// <param name="fromAccId">Source account ID.</param>
        /// <param name="toAccId">Destination account ID.</param>
        /// <param name="amount">Amount to transfer.</param>
        public void TransferCentral(int fromAccId, int toAccId, decimal amount)
        {
            // Create and open a new SQL connection using the provided connection string.
            using var conn = new SqlConnection(_connCentral);
            conn.Open();

            // Execute the stored procedure to perform the transfer.
            conn.Execute(
                "EXEC dbo.sp_TransferCentral @FromAccId, @ToAccId, @Amount",
                new { FromAccId = fromAccId, ToAccId = toAccId, Amount = amount });
        }
    }
}
