using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TransDemo.Models;

namespace TransDemo.Data.Repositories
{
    /// <summary>
    /// Provides SQL Server-based implementation of IAccountRepository for managing accounts.
    /// </summary>
    public class SqlAccountRepository : IAccountRepository
    {
        private readonly string _connString;

        /// <summary>
        /// Initializes a new instance of the <see cref="SqlAccountRepository"/> class.
        /// </summary>
        /// <param name="connString">The connection string to the SQL Server database.</param>
        public SqlAccountRepository(string connString) => _connString = connString;

        /// <summary>
        /// Creates and returns a new SQL database connection.
        /// </summary>
        /// <returns>An <see cref="IDbConnection"/> instance.</returns>
        private IDbConnection Connection() => new SqlConnection(_connString);

        /// <summary>
        /// Retrieves all accounts from the database.
        /// </summary>
        /// <returns>An enumerable collection of <see cref="Account"/> objects.</returns>
        public IEnumerable<Account> GetAll()
        {
            using var db = Connection();
            db.Open();
            // Retrieve all accounts with their details from the Accounts table
            return db.Query<Account>(
               @"SELECT 
               AccountId,
               AccountNumber,
               Balance,
               CustomerId,
               BranchId
             FROM dbo.Accounts");
        }

        /// <summary>
        /// Debits the specified amount from the account with the given ID.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account.</param>
        /// <param name="amount">The amount to debit.</param>
        public void Debit(int accountId, decimal amount)
        {
            using var db = Connection();
            db.Open();
            // Subtract the specified amount from the account's balance
            db.Execute("UPDATE Accounts SET Balance = Balance - @amt WHERE AccountId = @id",
                       new { amt = amount, id = accountId });
        }

        /// <summary>
        /// Credits the specified amount to the account with the given ID.
        /// </summary>
        /// <param name="accountId">The unique identifier of the account.</param>
        /// <param name="amount">The amount to credit.</param>
        public void Credit(int accountId, decimal amount)
        {
            using var db = Connection();
            db.Open();
            // Add the specified amount to the account's balance
            db.Execute("UPDATE Accounts SET Balance = Balance + @amt WHERE AccountId = @id",
                       new { amt = amount, id = accountId });
        }
    }
}
