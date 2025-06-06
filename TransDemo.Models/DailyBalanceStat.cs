namespace TransDemo.Models;

/// <summary>
/// Represents the daily cumulative balance statistics for a specific date.
/// </summary>
public class DailyBalanceStat
{
    /// <summary>
    /// Gets or sets the date for which the balance is calculated.
    /// </summary>
    public DateTime BalanceDate { get; set; }

    /// <summary>
    /// Gets or sets the cumulative balance for the specified date.
    /// </summary>
    public decimal CumulativeBalance { get; set; }
}