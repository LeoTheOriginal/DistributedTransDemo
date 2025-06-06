namespace TransDemo.Models
{
    /// <summary>
    /// Represents a customer entity with an identifier and full name.
    /// </summary>
    public class Customer
    {
        /// <summary>
        /// Gets or sets the unique identifier for the customer.
        /// </summary>
        public int CustomerId { get; set; }

        /// <summary>
        /// Gets or sets the full name of the customer.
        /// </summary>
        public string FullName { get; set; } = "";
    }
}
    