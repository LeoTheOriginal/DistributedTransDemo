using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TransDemo.Models;

namespace TransDemo.Data.Repositories
{
    public class SqlCustomerRepository : ICustomerRepository
    {
        private readonly string _connString;
        public SqlCustomerRepository(string connString) => _connString = connString;

        private IDbConnection Connection() => new SqlConnection(_connString);

        public IEnumerable<Customer> GetAll(string? filter = null)
        {
            using var db = Connection();
            db.Open();
            var baseSql = "SELECT CustomerId, FirstName + ' ' + LastName AS FullName FROM Customers";
                if (string.IsNullOrWhiteSpace(filter))
                        return db.Query<Customer>(baseSql);
                return db.Query<Customer>(
            baseSql + " WHERE FirstName + ' ' + LastName LIKE @f",
            new { f = $"%{filter}%" });
        }
    }
}
