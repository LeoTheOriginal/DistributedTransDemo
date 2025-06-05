// TransDemo.Data/Repositories/SqlBranchClientRepository.cs
using System;
using System.Data;
using Dapper;
using Microsoft.Data.SqlClient;


namespace TransDemo.Data.Repositories
{
    public class SqlBranchClientRepository : IBranchClientRepository
    {
        private readonly string _connString;
        public SqlBranchClientRepository(string connString) => _connString = connString;

        private IDbConnection Connection() => new SqlConnection(_connString);

        public decimal GetBalance(int centralClientId)
        {
            using var db = Connection();
            db.Open();
            return db.QuerySingle<decimal>(
                "SELECT Balance FROM dbo.BranchClients WHERE CentralClientId = @cid",
                new { cid = centralClientId });
        }

        public void Debit(int centralClientId, decimal amount, string description)
        {
            using var db = Connection();
            db.Open();

            // 1) Sprawdź saldo
            var bal = db.QuerySingle<decimal>(
                "SELECT Balance FROM dbo.BranchClients WHERE CentralClientId = @cid",
                new { cid = centralClientId });
            if (bal < amount)
                throw new InvalidOperationException("Niewystarczające środki w oddziale.");

            // 2) Obciąż konto
            db.Execute(
                "UPDATE dbo.BranchClients SET Balance = Balance - @amt WHERE CentralClientId = @cid",
                new { amt = amount, cid = centralClientId });

            // 3) Zapisz historię
            db.Execute(
                @"INSERT INTO dbo.BranchTransactions(BranchClientId, Amount, TxnType, Description)
                  SELECT BranchClientId, @amt, 'DEBIT', @desc
                  FROM dbo.BranchClients WHERE CentralClientId = @cid",
                new { amt = amount, cid = centralClientId, desc = description });
        }

        public void Credit(int centralClientId, decimal amount, string description)
        {
            using var db = Connection();
            db.Open();

            db.Execute(
                "UPDATE dbo.BranchClients SET Balance = Balance + @amt WHERE CentralClientId = @cid",
                new { amt = amount, cid = centralClientId });

            db.Execute(
                @"INSERT INTO dbo.BranchTransactions(BranchClientId, Amount, TxnType, Description)
                  SELECT BranchClientId, @amt, 'CREDIT', @desc
                  FROM dbo.BranchClients WHERE CentralClientId = @cid",
                new { amt = amount, cid = centralClientId, desc = description });
        }
    }
}
