using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TransDemo.Models;

namespace TransDemo.Data.Repositories
{
    public class SqlAccountRepository : IAccountRepository
    {
        private readonly string _connString;
        public SqlAccountRepository(string connString) => _connString = connString;

        private IDbConnection Connection() => new SqlConnection(_connString);

        public IEnumerable<Account> GetAll()
        {
            using var db = Connection();
            db.Open();
            // pobieramy z tabeli Accounts, żeby mieć CustomerId i BranchId
            return db.Query<Account>(
               @"SELECT 
           AccountId,
           AccountNumber,
           Balance,
           CustomerId,
           BranchId
         FROM dbo.Accounts");
        }


        public void Debit(int accountId, decimal amount)
        {
            using var db = Connection();
            db.Open();
            db.Execute("UPDATE Accounts SET Balance = Balance - @amt WHERE AccountId = @id",
                       new { amt = amount, id = accountId });
        }

        public void Credit(int accountId, decimal amount)
        {
            using var db = Connection();
            db.Open();
            db.Execute("UPDATE Accounts SET Balance = Balance + @amt WHERE AccountId = @id",
                       new { amt = amount, id = accountId });
        }
    }
}
