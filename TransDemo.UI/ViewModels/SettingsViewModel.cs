using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Threading.Tasks;
using System.Windows;
using System.Windows.Input;
using Microsoft.Data.SqlClient;
using TransDemo.UI.Models;
using System.Data;

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

        public ObservableCollection<string> AvailableDatabases { get; } = new ObservableCollection<string>();

        public ICommand TestConnectionCommand { get; }
        public ICommand ApplyCommand { get; }
        public ICommand CancelCommand { get; }

        private DbConnectionSetting _selectedConnection;
        public DbConnectionSetting? SelectedConnection
        {
            get => _selectedConnection;
            set
            {
                if (SetProperty(ref _selectedConnection, value) && value != null)
                {
                    // 3) przy każdej zmianie połączenia przeładuj listę baz
                    LoadDatabasesForConnection(value);
                }
            }
        }

        public ICommand AddConnectionCommand { get; }
        public ICommand RemoveConnectionCommand { get; }

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
            // 4) ustaw domyślone SelectedConnection i od razu załaduj jego bazy
            SelectedConnection = Connections.FirstOrDefault();

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

            AddConnectionCommand = new RelayCommand<object>(_ =>
            {
                // Dodajemy nowy, pusty entry
                var newConn = new DbConnectionSetting
                {
                    Key = "NewConnection",
                    Server = "",
                    Database = "",
                    UserName = "",
                    Password = "",
                    RememberPassword = false,
                    KeepConnectionAlive = false,
                    Encrypt = false,
                    TrustServerCertificate = false
                };
                Connections.Add(newConn);
            });

            RemoveConnectionCommand = new RelayCommand<DbConnectionSetting>(conn =>
            {
                if (conn != null)
                    Connections.Remove(conn);
            }, conn => conn != null);

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

        /// <summary>
        /// Ładuje listę nazw baz z serwera wskazanego w c.Server.
        /// </summary>
        private void LoadDatabasesForConnection(DbConnectionSetting c)
        {
            AvailableDatabases.Clear();
            // zbuduj łańcuch do master
            var sb = new SqlConnectionStringBuilder
            {
                DataSource = c.Server,
                InitialCatalog = "master",
                UserID = c.UserName,
                Password = c.Password,
                Encrypt = c.Encrypt,
                TrustServerCertificate = c.TrustServerCertificate,
                ConnectTimeout = 5
            };
            using var conn = new SqlConnection(sb.ConnectionString);
            conn.Open();
            using var cmd = conn.CreateCommand();
            cmd.CommandText = "SELECT name FROM sys.databases ORDER BY name";
            using var rdr = cmd.ExecuteReader();
            while (rdr.Read())
                AvailableDatabases.Add(rdr.GetString(0));

            // ustaw domyślnie na tę, którą mamy w konfiguracji
            if (AvailableDatabases.Contains(c.Database))
                c.Database = c.Database;
            else if (AvailableDatabases.Count > 0)
                c.Database = AvailableDatabases[0];
        }
    }
}
