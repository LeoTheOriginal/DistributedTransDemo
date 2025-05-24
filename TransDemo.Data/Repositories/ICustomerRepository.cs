using System.Collections.Generic;
using TransDemo.Models;

namespace TransDemo.Data.Repositories
{
    public interface ICustomerRepository
    {
        IEnumerable<Customer> GetAll(string? filter = null);
    }
}
