using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Threading.Tasks;
using TransDemo.Logic.Services;
using TransDemo.Models;

namespace TransDemo.UI.ViewModels
{
    /// <summary>
    /// ViewModel responsible for managing and exposing transaction history data for a selected branch.
    /// </summary>
    public class HistoryViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Service used to query transaction history from the database.
        /// </summary>
        private readonly HistoryQueryService _historyService;

        /// <summary>
        /// Collection of history entries for the currently selected branch.
        /// </summary>
        public ObservableCollection<HistoryEntry> Entries { get; } = new();

        private int _selectedBranch = 0;

        /// <summary>
        /// Gets or sets the currently selected branch number.
        /// When changed, triggers reloading of the history entries.
        /// </summary>
        public int SelectedBranch
        {
            get => _selectedBranch;
            set
            {
                if (SetProperty(ref _selectedBranch, value))
                    _ = LoadHistoryAsync();
            }
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryViewModel"/> class.
        /// Loads the history for the default branch on creation.
        /// </summary>
        /// <param name="historyService">Service for querying history data.</param>
        public HistoryViewModel(HistoryQueryService historyService)
        {
            _historyService = historyService;
            _ = LoadHistoryAsync();
        }

        /// <summary>
        /// Asynchronously loads the transaction history for the currently selected branch.
        /// Clears the existing entries and populates the collection with new data.
        /// </summary>
        /// <returns>A task representing the asynchronous operation.</returns>
        public async Task LoadHistoryAsync()
        {
            Entries.Clear();
            var entries = await _historyService.GetBranchHistoryAsync(SelectedBranch);
            foreach (var entry in entries)
                Entries.Add(entry);
        }

        /// <summary>
        /// Event raised when a property value changes.
        /// </summary>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Sets the property to the specified value and raises the <see cref="PropertyChanged"/> event if the value has changed.
        /// </summary>
        /// <typeparam name="T">Type of the property.</typeparam>
        /// <param name="field">Reference to the backing field.</param>
        /// <param name="value">New value to set.</param>
        /// <param name="name">Name of the property (automatically supplied by the compiler).</param>
        /// <returns>True if the value was changed; otherwise, false.</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string? name = null)
        {
            if (Equals(field, value)) return false;
            field = value;
            PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));
            return true;
        }
    }
}
