using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using TransDemo.UI.Models;

namespace TransDemo.UI.ViewModels
{
    public class SettingsViewModel : BaseViewModel
    {
        private readonly string _settingsPath = "appsettings.json";

        private ObservableCollection<DbConnectionSetting> _connections = new();
        public ObservableCollection<DbConnectionSetting> Connections
        {
            get => _connections;
            set => SetProperty(ref _connections, value);
        }

        public ICommand TestConnectionCommand { get; }
        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }

        public SettingsViewModel()
        {
            // 1) Wczytujemy słownik string→connectionString
            var raw = AppSettings.Load(_settingsPath).ConnectionStrings;

            // 2) Parsujemy każdy entry do DbConnectionSetting
            var list = raw.Select(kvp =>
            {
                var sb = new SqlConnectionStringBuilder(kvp.Value);
                return new DbConnectionSetting
                {
                    Key = kvp.Key,
                    Server = sb.DataSource,
                    Database = sb.InitialCatalog,
                    UserName = sb.UserID,
                    Password = sb.Password,
                    Encrypt = sb.Encrypt,
                    TrustServerCertificate = sb.TrustServerCertificate,
                    RememberPassword = !string.IsNullOrEmpty(sb.Password),
                    KeepConnectionAlive = false
                };
            }).ToList();

            Connections = new ObservableCollection<DbConnectionSetting>(list);

            // 3) Komendy
            TestConnectionCommand = new RelayCommand<DbConnectionSetting>(async c =>
                await TestConnectionAsync(c)
            );

            ApplyCommand = new RelayCommand<object>(_ =>
            {
                // 4) Spłaszczamy z powrotem do Dictionary<string,string>
                var dict = Connections.ToDictionary(
                    c => c.Key,
                    c =>
                    {
                        var sb = new SqlConnectionStringBuilder
                        {
                            DataSource = c.Server,
                            InitialCatalog = c.Database,
                            UserID = c.UserName,
                            Password = c.RememberPassword ? c.Password : "",
                            Encrypt = c.Encrypt,
                            TrustServerCertificate = c.TrustServerCertificate,
                            ConnectTimeout = 30
                        };
                        return sb.ConnectionString;
                    });

                var updated = new AppSettings { ConnectionStrings = dict };
                updated.Save(_settingsPath);

                MessageBox.Show("Ustawienia zapisane.",
                                "OK",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            });

            CancelCommand = new RelayCommand<Window>(w => w?.Close());
        }

        private async Task TestConnectionAsync(DbConnectionSetting c)
        {
            var sb = new SqlConnectionStringBuilder
            {
                DataSource = c.Server,
                InitialCatalog = c.Database,
                UserID = c.UserName,
                Password = c.Password,
                Encrypt = c.Encrypt,
                TrustServerCertificate = c.TrustServerCertificate,
                ConnectTimeout = 5
            };

            using var conn = new SqlConnection(sb.ConnectionString);
            try
            {
                await conn.OpenAsync();
                MessageBox.Show($"Połączono z „{c.Key}”.",
                                "Test OK",
                                MessageBoxButton.OK,
                                MessageBoxImage.Information);
            }
            catch (Exception ex)
            {
                MessageBox.Show(ex.Message,
                                "Błąd połączenia",
                                MessageBoxButton.OK,
                                MessageBoxImage.Error);
            }
        }
    }
}
