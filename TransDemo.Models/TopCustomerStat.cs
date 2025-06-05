using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

// 
namespace TransDemo.Models
{
    /// <summary>
    /// Represents statistics for a top customer, including their full name and total transaction amount.
    /// </summary>
    public class TopCustomerStat
    {
        /// <summary>
        /// Gets or sets the full name of the customer.
        /// </summary>
        public string FullName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the total transaction amount for the customer.
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
