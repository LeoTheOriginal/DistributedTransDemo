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
    /// <summary>
    /// Repository for handling distributed transfer operations using SQL Server.
    /// </summary>
    public class SqlDistributedTransferRepository : IDistributedTransferRepository
    {
        private readonly string _connCentral;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlDistributedTransferRepository"/> class.
        /// </summary>
        /// <param name="connCentral">The connection string to the central database.</param>
        public SqlDistributedTransferRepository(string connCentral) => _connCentral = connCentral;

        /// <summary>
        /// Executes a distributed transfer between two accounts by calling a stored procedure.
        /// </summary>
        /// <param name="fromAccId">The ID of the account to transfer from.</param>
        /// <param name="toAccId">The ID of the account to transfer to.</param>
        /// <param name="amount">The amount to transfer.</param>
        public void TransferDistributed(int fromAccId, int toAccId, decimal amount)
        {
            // Create and open a new SQL connection using the provided connection string.
            using IDbConnection conn = new SqlConnection(_connCentral);
            conn.Open();

            // Execute the stored procedure for distributed transfer with the specified parameters.
            conn.Execute(
                "EXEC dbo.sp_TransferDistributed @FromAccId, @ToAccId, @Amount",
                new { FromAccId = fromAccId, ToAccId = toAccId, Amount = amount });
        }
    }
}
