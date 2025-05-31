using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TransDemo.Logic.Services;
using TransDemo.Models;

namespace TransDemo.UI.ViewModels
{
    public class HistoryViewModel : INotifyPropertyChanged
    {
        private readonly HistoryQueryService _historyService;

        public ObservableCollection<HistoryEntry> Entries { get; } = new();

        private int _selectedBranch = 0;
        public int SelectedBranch
        {
            get => _selectedBranch;
            set
            {
                if (SetProperty(ref _selectedBranch, value))
                    _ = LoadHistoryAsync();
            }
        }

        public HistoryViewModel(HistoryQueryService historyService)
        {
            _historyService = historyService;
            _ = LoadHistoryAsync();
        }

        public async Task LoadHistoryAsync()
        {
            Entries.Clear();
            var entries = await _historyService.GetBranchHistoryAsync(SelectedBranch);
            foreach (var entry in entries)
                Entries.Add(entry);
        }



        public event PropertyChangedEventHandler? PropertyChanged;

        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return true;
        }
    }
}
