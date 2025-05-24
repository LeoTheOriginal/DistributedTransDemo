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
    public class MainViewModel : INotifyPropertyChanged
    {
        private readonly IServiceProvider _provider;
        private readonly TransferService _transferSvc;

        public ObservableCollection<TabItemViewModel> Tabs { get; }
        private TabItemViewModel _selectedTab;
        public TabItemViewModel SelectedTab
        {
            get => _selectedTab;
            set => SetProperty(ref _selectedTab, value);
        }

        public string CurrentUser { get; } = "Operator";
        public string StatusMessage { get => _status; private set => SetProperty(ref _status, value); }
        private string _status = "Ready";

        public ICommand OpenSettingsCommand { get; }
        public ICommand ExitCommand { get; }
        public ICommand OpenAboutCommand { get; }

        public MainViewModel(TransferService svc) => _transferSvc = svc;
        public MainViewModel(IServiceProvider provider, TransferService svc)
        {
            _provider = provider;
            _transferSvc = svc;

            OpenSettingsCommand = new RelayCommand(_ => {
                // Wyciągamy SettingsView z kontenera
                var win = _provider.GetRequiredService<SettingsView>();
                win.Owner = System.Windows.Application.Current.MainWindow;
                win.ShowDialog();
            });

            ExitCommand = new RelayCommand(_ => System.Windows.Application.Current.Shutdown());
            OpenAboutCommand = new RelayCommand(_ => { /* TODO: okno o aplikacji */ });

            Tabs = new ObservableCollection<TabItemViewModel>
            {
                _provider.GetRequiredService<DashboardTabViewModel>(),
                _provider.GetRequiredService<TransfersTabViewModel>(),
                _provider.GetRequiredService<HistoryTabViewModel>()
            };
            SelectedTab = Tabs[0];
        }

        #region INotifyPropertyChanged
        public event PropertyChangedEventHandler? PropertyChanged;
        protected void OnPropertyChanged([CallerMemberName] string name = null!)
            => PropertyChanged?.Invoke(this, new PropertyChangedEventArgs(name));

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
