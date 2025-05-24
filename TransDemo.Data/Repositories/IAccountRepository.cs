using System.Collections.Generic;
using TransDemo.Models;

namespace TransDemo.Data.Repositories
{
    public interface IAccountRepository
    {
        IEnumerable<Account> GetAll();
        void Debit(int accountId, decimal amount);
        void Credit(int accountId, decimal amount);
    }
}
