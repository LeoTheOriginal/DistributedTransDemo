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


public class DashboardViewModel : INotifyPropertyChanged
{
    private readonly DashboardStatsService _statsService;

    public enum DashboardChartType
    {
        DailyTxnCount,
        DailyTxnAmount,
        DailyCumulativeBalance,
        BranchPie,
        TopCustomers
    }


    public ObservableCollection<ISeries> ChartSeries { get; } = new();
    public Axis[] XAxes { get; } =
    {
        new Axis { Labels = [] }
    };
    public ObservableCollection<Axis> YAxes { get; } = new()
    {
        new Axis { Labeler = value => value.ToString("N0") }
    };


    public ObservableCollection<int> DayOptions { get; } = new() { 7, 14, 30, 60 };
    public ObservableCollection<DashboardChartType> ChartTypes { get; } = new()
    {
        DashboardChartType.DailyTxnCount,
        DashboardChartType.DailyTxnAmount,
        DashboardChartType.DailyCumulativeBalance,
        DashboardChartType.BranchPie,
        DashboardChartType.TopCustomers
    };


    private int _selectedDays = 7;
    public int SelectedDays
    {
        get => _selectedDays;
        set { if (SetProperty(ref _selectedDays, value)) _ = LoadChartAsync(); }
    }

    private DashboardChartType _selectedChartType = DashboardChartType.DailyTxnCount;
    public DashboardChartType SelectedChartType
    {
        get => _selectedChartType;
        set { if (SetProperty(ref _selectedChartType, value)) _ = LoadChartAsync(); }
    }


    public ICommand RefreshCommand { get; }

    public DashboardViewModel(DashboardStatsService statsService)
    {
        _statsService = statsService;
        RefreshCommand = new RelayCommand(async _ => await LoadChartAsync());
        _ = LoadChartAsync();
    }

    private async Task LoadChartAsync()
    {
        var rawStats = (await _statsService.GetStatsAsync(SelectedDays)).ToList();

        // Utwórz listę wszystkich dat od dzisiaj do -N
        var allDates = Enumerable.Range(0, SelectedDays)
            .Select(offset => DateTime.Today.AddDays(-offset))
            .OrderBy(d => d)
            .ToList();

        // Słownik statystyk wg daty
        var statsDict = rawStats.ToDictionary(s => s.TxnDate.Date);

        // Wyrównaj dane: wstaw zera jeśli brak transakcji danego dnia
        var filledStats = allDates.Select(date =>
            statsDict.TryGetValue(date, out var stat)
                ? new DailyTransactionStat { TxnDate = date, TxnCount = stat.TxnCount, TotalAmount = stat.TotalAmount }
                : new DailyTransactionStat { TxnDate = date, TxnCount = 0, TotalAmount = 0 }
        ).ToList();

        // Zaktualizuj etykiety osi X
        XAxes[0].Labels = filledStats.Select(s => s.TxnDate.ToString("MM-dd")).ToArray();
        OnPropertyChanged(nameof(XAxes));

        // Wyczyść poprzednią serię
        ChartSeries.Clear();

        // Zbuduj wykresy zależnie od wybranego trybu
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


    // PropertyChanged boilerplate
    public event PropertyChangedEventHandler? PropertyChanged;
    protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
    {
        if (Equals(field, value)) return false;
        field = value;
        PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
        return true;
    }
    protected void OnPropertyChanged(string name) => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
}
