using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransDemo.Models
{
    /// <summary>
    /// Represents daily statistics for transactions, including the date, 
    /// the number of transactions, and the total transaction amount.
    /// </summary>
    public class DailyTransactionStat
    {
        /// <summary>
        /// Gets or sets the date of the transactions.
        /// </summary>
        public DateTime TxnDate { get; set; }

        /// <summary>
        /// Gets or sets the total number of transactions for the specified date.
        /// </summary>
        public int TxnCount { get; set; }

        /// <summary>
        /// Gets or sets the total amount of all transactions for the specified date.
        /// </summary>
        public decimal TotalAmount { get; set; }
    }
}
