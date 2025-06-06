using LiveChartsCore;
using LiveChartsCore.SkiaSharpView;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TransDemo.Logic.Services;
using TransDemo.Models;
using TransDemo.UI.ViewModels;

using LiveChartsCore.SkiaSharpView.Painting;
using SkiaSharp;


/// <summary>
/// ViewModel for the dashboard, providing chart data and options for the dashboard view.
/// Handles loading and formatting of statistics for various chart types.
/// </summary>
public class DashboardViewModel : INotifyPropertyChanged
{
    private readonly DashboardStatsService _statsService;

    /// <summary>
    /// Enum representing the available chart types for the dashboard.
    /// </summary>
    public enum DashboardChartType
    {
        /// <summary>
        /// Daily transaction count chart.
        /// </summary>
        DailyTxnCount,
        /// <summary>
        /// Daily transaction amount chart.
        /// </summary>
        DailyTxnAmount,
        /// <summary>
        /// Daily cumulative balance chart.
        /// </summary>
        DailyCumulativeBalance,
        /// <summary>
        /// Pie chart of branch transaction shares.
        /// </summary>
        BranchPie,
        /// <summary>
        /// Column chart of top customers by transaction amount.
        /// </summary>
        TopCustomers
    }

    /// <summary>
    /// Gets the collection of chart series to be displayed.
    /// </summary>
    public ObservableCollection<ISeries> ChartSeries { get; } = new();

    /// <summary>
    /// Gets the X axes for the chart.
    /// </summary>
    public Axis[] XAxes { get; } =
    {
        new Axis { Labels = [] }
    };

    /// <summary>
    /// Gets the Y axes for the chart.
    /// </summary>
    public ObservableCollection<Axis> YAxes { get; } = new()
    {
        new Axis { Labeler = value => value.ToString("N0") }
    };

    /// <summary>
    /// Gets the available day options for the dashboard statistics.
    /// </summary>
    public ObservableCollection<int> DayOptions { get; } = new() { 7, 14, 30, 60 };

    /// <summary>
    /// Gets the available chart types for the dashboard.
    /// </summary>
    public ObservableCollection<DashboardChartType> ChartTypes { get; } = new()
    {
        DashboardChartType.DailyTxnCount,
        DashboardChartType.DailyTxnAmount,
        DashboardChartType.DailyCumulativeBalance,
        DashboardChartType.BranchPie,
        DashboardChartType.TopCustomers
    };

    private int _selectedDays = 7;
    /// <summary>
    /// Gets or sets the number of days to display in the chart.
    /// </summary>
    public int SelectedDays
    {
        get => _selectedDays;
        set { if (SetProperty(ref _selectedDays, value)) _ = LoadChartAsync(); }
    }

    private DashboardChartType _selectedChartType = DashboardChartType.DailyTxnCount;
    /// <summary>
    /// Gets or sets the selected chart type.
    /// </summary>
    public DashboardChartType SelectedChartType
    {
        get => _selectedChartType;
        set { if (SetProperty(ref _selectedChartType, value)) _ = LoadChartAsync(); }
    }

    /// <summary>
    /// Gets the command to refresh the chart data.
    /// </summary>
    public ICommand RefreshCommand { get; }

    /// <summary>
    /// Initializes a new instance of the <see cref="DashboardViewModel"/> class.
    /// </summary>
    /// <param name="statsService">The service used to retrieve dashboard statistics.</param>
    public DashboardViewModel(DashboardStatsService statsService)
    {
        _statsService = statsService;
        RefreshCommand = new RelayCommand(async _ => await LoadChartAsync());
        _ = LoadChartAsync();
    }

    /// <summary>
    /// Loads and prepares chart data asynchronously based on the selected chart type and days.
    /// </summary>
    private async Task LoadChartAsync()
    {
        var rawStats = (await _statsService.GetStatsAsync(SelectedDays)).ToList();

        // Create a list of all dates from today to -N days
        var allDates = Enumerable.Range(0, SelectedDays)
            .Select(offset => DateTime.Today.AddDays(-offset))
            .OrderBy(d => d)
            .ToList();

        // Dictionary of statistics by date
        var statsDict = rawStats.ToDictionary(s => s.TxnDate.Date);

        // Fill missing days with zero values
        var filledStats = allDates.Select(date =>
            statsDict.TryGetValue(date, out var stat)
                ? new DailyTransactionStat { TxnDate = date, TxnCount = stat.TxnCount, TotalAmount = stat.TotalAmount }
                : new DailyTransactionStat { TxnDate = date, TxnCount = 0, TotalAmount = 0 }
        ).ToList();

        // Update X axis labels
        XAxes[0].Labels = filledStats.Select(s => s.TxnDate.ToString("MM-dd")).ToArray();
        OnPropertyChanged(nameof(XAxes));

        // Clear previous series
        ChartSeries.Clear();

        // Build chart series based on selected chart type
        switch (SelectedChartType)
        {
            case DashboardChartType.DailyTxnCount:
                ChartSeries.Add(new LineSeries<int>
                {
                    Values = filledStats.Select(s => s.TxnCount).ToArray(),
                    Name = "Liczba transakcji dziennie"
                });
                break;

            case DashboardChartType.DailyTxnAmount:
                ChartSeries.Add(new LineSeries<decimal>
                {
                    Values = filledStats.Select(s => s.TotalAmount).ToArray(),
                    Name = "Suma transakcji dziennie"
                });
                break;

            case DashboardChartType.DailyCumulativeBalance:
                {
                    var rawBalances = (await _statsService.GetDailyBalanceAsync(SelectedDays)).ToList();
                    ChartSeries.Add(new LineSeries<decimal>
                    {
                        Values = rawBalances.OrderBy(b => b.BalanceDate).Select(b => b.CumulativeBalance).ToArray(),
                        Name = "Saldo sumaryczne dziennie"
                    });

                    XAxes[0].Labels = rawBalances.OrderBy(b => b.BalanceDate).Select(b => b.BalanceDate.ToString("MM-dd")).ToArray();

                    break;
                }

            case DashboardChartType.BranchPie:
                {
                    var shares = (await _statsService.GetBranchShareAsync()).ToList();

                    foreach (var s in shares)
                    {
                        ChartSeries.Add(new PieSeries<decimal>
                        {
                            Values = [s.SharePercent],
                            Name = s.BranchName,
                            DataLabelsSize = 14,
                            DataLabelsPosition = LiveChartsCore.Measure.PolarLabelsPosition.Middle,
                            DataLabelsFormatter = chartPoint => $"{chartPoint.Model:F2} %",
                            DataLabelsPaint = new SolidColorPaint(SKColors.Black),
                        });
                    }

                    // Pie charts don't need axes
                    XAxes[0].Labels = [];
                    YAxes.Clear();
                    YAxes.Add(new Axis { Labeler = value => value.ToString("N0") });
                    break;
                }

            case DashboardChartType.TopCustomers:
                {
                    var top = (await _statsService.GetTopCustomersAsync()).ToList();
                    ChartSeries.Add(new ColumnSeries<decimal>
                    {
                        Values = top.Select(c => c.TotalAmount).ToArray(),
                        Name = "Top klienci"
                    });

                    XAxes[0].Labels = top.Select(c => c.FullName).ToArray();
                    break;
                }
        }
    }

    /// <summary>
    /// Occurs when a property value changes.
    /// </summary>
    public event PropertyChangedEventHandler? PropertyChanged;

    /// <summary>
    /// Sets the property and raises the PropertyChanged event if the value changes.
    /// </summary>
    /// <typeparam name="T">The type of the property.</typeparam>
    /// <param name="field">The backing field reference.</param>
    /// <param name="value">The new value.</param>
    /// <param name="name">The property name.</param>
    /// <returns>True if the value was changed; otherwise, false.</returns>
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        return true;
    }

    /// <summary>
    /// Raises the PropertyChanged event for the specified property.
    /// </summary>
    /// <param name="name">The property name.</param>
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
