using System;
using System.Collections.Generic;
using System.Linq;
using System.Text;
using System.Threading.Tasks;

namespace TransDemo.Models
{
    /// <summary>
    /// Represents the share percentage of a transaction for a specific branch.
    /// </summary>
    public class BranchTxnShare
    {
        /// <summary>
        /// Gets or sets the name of the branch.
        /// </summary>
        public string BranchName { get; set; } = null!;

        /// <summary>
        /// Gets or sets the share percentage of the transaction for the branch.
        /// </summary>
        public decimal SharePercent { get; set; }
    }
}
        