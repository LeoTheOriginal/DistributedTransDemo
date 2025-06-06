using System.Collections.Generic;
using TransDemo.Models;

namespace TransDemo.Data.Repositories
{
    /// <summary>
    /// Defines methods for accessing customer data.
    /// </summary>
    public interface ICustomerRepository
    {
        /// <summary>
        /// Retrieves all customers, optionally filtered by a search string.
        /// </summary>
        /// <param name="filter">An optional filter string to search customers by name or other criteria.</param>
        /// <returns>An enumerable collection of <see cref="Customer"/> objects.</returns>
        IEnumerable<Customer> GetAll(string? filter = null);
    }
}
