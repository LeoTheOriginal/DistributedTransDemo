namespace TransDemo.Models
{
    /// <summary>
    /// Represents a bank account entity.
    /// </summary>
    public class Account
    {
        /// <summary>
        /// Gets or sets the unique identifier for the account.
        /// </summary>
        public int AccountId { get; set; }

        /// <summary>
        /// Gets or sets the account number.
        /// </summary>
        public string AccountNumber { get; set; } = "";

        /// <summary>
        /// Gets or sets the current balance of the account.
        /// </summary>
        public decimal Balance { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the customer who owns the account.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the identifier of the branch where the account is held.
        /// </summary>
        public int BranchId { get; set; }
    }
}
