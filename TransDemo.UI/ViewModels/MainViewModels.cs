using MaterialDesignThemes.Wpf;
using Microsoft.Extensions.DependencyInjection;
using System.Collections.ObjectModel;
using System.ComponentModel;
using System.Runtime.CompilerServices;
using System.Windows.Input;
using TransDemo.Logic.Services;
using TransDemo.UI.Views;
using static System.Net.Mime.MediaTypeNames;

namespace TransDemo.UI.ViewModels
{
    /// <summary>
    /// Main ViewModel for the application. Handles tab navigation, commands, and status information.
    /// </summary>
    public class MainViewModel : INotifyPropertyChanged
    {
        /// <summary>
        /// Service provider for resolving dependencies.
        /// </summary>
        private readonly IServiceProvider _provider;

        /// <summary>
        /// Service responsible for transfer operations.
        /// </summary>
        private readonly TransferService _transferSvc;

        /// <summary>
        /// Collection of tab view models displayed in the main window.
        /// </summary>
        public ObservableCollection<TabItemViewModel> Tabs { get; }

        private TabItemViewModel _selectedTab;

        /// <summary>
        /// Gets or sets the currently selected tab.
        /// </summary>
        public TabItemViewModel SelectedTab
        {
            get => _selectedTab;
            set => SetProperty(ref _selectedTab, value);
        }

        /// <summary>
        /// Gets the name of the current user.
        /// </summary>
        public string CurrentUser { get; } = "Operator";

        private string _status = "Ready";

        /// <summary>
        /// Gets the current status message.
        /// </summary>
        public string StatusMessage
        {
            get => _status;
            private set => SetProperty(ref _status, value);
        }

        /// <summary>
        /// Command to open the settings window.
        /// </summary>
        public ICommand OpenSettingsCommand { get; }

        /// <summary>
        /// Command to exit the application.
        /// </summary>
        public ICommand ExitCommand { get; }

        /// <summary>
        /// Command to open the about window.
        /// </summary>
        public ICommand OpenAboutCommand { get; }

        /// <summary>
        /// Constructor for design-time or test usage.
        /// </summary>
        /// <param name="svc">The transfer service.</param>
        public MainViewModel(TransferService svc) => _transferSvc = svc;

        /// <summary>
        /// Initializes a new instance of the <see cref="MainViewModel"/> class.
        /// </summary>
        /// <param name="provider">The service provider for dependency resolution.</param>
        /// <param name="svc">The transfer service.</param>
        public MainViewModel(IServiceProvider provider, TransferService svc)
        {
            _provider = provider;
            _transferSvc = svc;

            OpenSettingsCommand = new RelayCommand(_ =>
            {
                // Retrieve SettingsView from the service provider and show it as a dialog.
                var win = _provider.GetRequiredService<SettingsView>();
                win.Owner = System.Windows.Application.Current.MainWindow;
                win.ShowDialog();
            });

            ExitCommand = new RelayCommand(_ => System.Windows.Application.Current.Shutdown());

            OpenAboutCommand = new RelayCommand(_ =>
            {
                // TODO: Implement about window display logic.
            });

            Tabs = new ObservableCollection<TabItemViewModel>
                {
                    _provider.GetRequiredService<DashboardTabViewModel>(),
                    _provider.GetRequiredService<TransfersTabViewModel>(),
                    _provider.GetRequiredService<HistoryTabViewModel>()
                };
            SelectedTab = Tabs[0];
        }

        #region INotifyPropertyChanged

        /// <inheritdoc/>
        public event PropertyChangedEventHandler? PropertyChanged;

        /// <summary>
        /// Raises the <see cref="PropertyChanged"/> event.
        /// </summary>
        /// <param name="name">The name of the property that changed.</param>
        protected void OnPropertyChanged([CallerMemberName] string name = null!)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

        /// <summary>
        /// Sets the property and raises <see cref="PropertyChanged"/> if the value changes.
        /// </summary>
        /// <typeparam name="T">The type of the property.</typeparam>
        /// <param name="field">Reference to the backing field.</param>
        /// <param name="value">The new value.</param>
        /// <param name="name">The name of the property.</param>
        /// <returns>True if the value was changed; otherwise, false.</returns>
        protected bool SetProperty<T>(ref T field, T value, [CallerMemberName] string name = null!)
        {
            if (Equals(field, value)) return false;
            field = value;
            OnPropertyChanged(name);
            return true;
        }
        #endregion
    }
}
