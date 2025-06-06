// TransDemo.Data/Repositories/SqlBranchClientRepository.cs
using System;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;


namespace TransDemo.Data.Repositories
{
    /// <summary>
    /// Repository for managing branch client accounts and transactions using SQL Server.
    /// </summary>
    public class SqlBranchClientRepository : IBranchClientRepository
    {
        private readonly string _connString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlBranchClientRepository"/> class.
        /// </summary>
        /// <param name="connString">The SQL Server connection string.</param>
        public SqlBranchClientRepository(string connString) => _connString = connString;

        /// <summary>
        /// Creates and returns a new SQL database connection.
        /// </summary>
        private IDbConnection Connection() => new SqlConnection(_connString);

        /// <inheritdoc />
        public decimal GetBalance(int centralClientId)
        {
            using var db = Connection();
            db.Open();
            // Retrieve the current balance for the specified central client.
            return db.QuerySingle<decimal>(
                "SELECT Balance FROM dbo.BranchClients WHERE CentralClientId = @cid",
                new { cid = centralClientId });
        }

        /// <inheritdoc />
        public void Debit(int centralClientId, decimal amount, string description)
        {
            using var db = Connection();
            db.Open();

            // 1) Check current balance
            var bal = db.QuerySingle<decimal>(
                "SELECT Balance FROM dbo.BranchClients WHERE CentralClientId = @cid",
                new { cid = centralClientId });
            if (bal < amount)
                throw new InvalidOperationException("Niewystarczające środki w oddziale.");

            // 2) Debit the account
            db.Execute(
                "UPDATE dbo.BranchClients SET Balance = Balance - @amt WHERE CentralClientId = @cid",
                new { amt = amount, cid = centralClientId });

            // 3) Record the transaction in history
            db.Execute(
                @"INSERT INTO dbo.BranchTransactions(BranchClientId, Amount, TxnType, Description)
                      SELECT BranchClientId, @amt, 'DEBIT', @desc
                      FROM dbo.BranchClients WHERE CentralClientId = @cid",
                new { amt = amount, cid = centralClientId, desc = description });
        }

        /// <inheritdoc />
        public void Credit(int centralClientId, decimal amount, string description)
        {
            using var db = Connection();
            db.Open();

            // Credit the account
            db.Execute(
                "UPDATE dbo.BranchClients SET Balance = Balance + @amt WHERE CentralClientId = @cid",
                new { amt = amount, cid = centralClientId });

            // Record the transaction in history
            db.Execute(
                @"INSERT INTO dbo.BranchTransactions(BranchClientId, Amount, TxnType, Description)
                      SELECT BranchClientId, @amt, 'CREDIT', @desc
                      FROM dbo.BranchClients WHERE CentralClientId = @cid",
                new { amt = amount, cid = centralClientId, desc = description });
        }
    }
}
