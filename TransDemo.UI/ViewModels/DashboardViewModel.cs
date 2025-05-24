using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using System.Windows.Input;
using TransDemo.Logic.Services;
using TransDemo.Models;

namespace TransDemo.UI.ViewModels
{
    public class DashboardViewModel : INotifyPropertyChanged
    {
        private readonly DistributedTransactionService _demoService;
        private readonly HistoryQueryService _historyQuery;

        public ObservableCollection<HistoryEntry> Branch1History { get; } = new();
        public ObservableCollection<HistoryEntry> Branch2History { get; } = new();

        public ICommand RefreshCommand { get; }
        public ICommand RunDemoCommand { get; }

        public DashboardViewModel(
            DistributedTransactionService demoService,
            HistoryQueryService historyQuery)
        {
            _demoService = demoService;
            _historyQuery = historyQuery;

            RefreshCommand = new RelayCommand(async _ => await LoadDataAsync());
            RunDemoCommand = new RelayCommand(async _ =>
            {
                _demoService.RunDemoTransaction();
                await LoadDataAsync();
            });

            // automatyczne załadowanie przy starcie
            _ = LoadDataAsync();
        }

        private async Task LoadDataAsync()
        {
            var b1 = await _historyQuery.GetBranchHistoryAsync(1);
            Branch1History.Clear();
            foreach (var e in b1) Branch1History.Add(e);

            var b2 = await _historyQuery.GetBranchHistoryAsync(2);
            Branch2History.Clear();
            foreach (var e in b2) Branch2History.Add(e);
        }

        public event PropertyChangedEventHandler? PropertyChanged;
        private void OnPropertyChanged([CallerMemberName] string? n = null)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(n));
    }
}
