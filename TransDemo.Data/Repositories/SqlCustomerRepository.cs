using System.Collections.Generic;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;
using TransDemo.Models;

namespace TransDemo.Data.Repositories
{
    // Repository implementation for accessing Customer data from SQL database
    public class SqlCustomerRepository : ICustomerRepository
    {
        private readonly string _connString;

        // Constructor accepting connection string
        public SqlCustomerRepository(string connString) => _connString = connString;

        // Helper method to create a new SQL connection
        private IDbConnection Connection() => new SqlConnection(_connString);

        // Retrieves all customers, optionally filtered by full name
        public IEnumerable<Customer> GetAll(string? filter = null)
        {
            using var db = Connection();
            db.Open();
            // Base SQL query to select customer ID and full name
            var baseSql = "SELECT CustomerId, FirstName + ' ' + LastName AS FullName FROM Customers";
            if (string.IsNullOrWhiteSpace(filter))
                // No filter provided, return all customers
                return db.Query<Customer>(baseSql);
            // Filter provided, return customers matching the filter
            return db.Query<Customer>(
                baseSql + " WHERE FirstName + ' ' + LastName LIKE @f",
                new { f = $"%{filter}%" });
        }
    }
}
