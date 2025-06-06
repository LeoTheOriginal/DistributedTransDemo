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
    /// <summary>
    /// ViewModel responsible for managing application database connection settings.
    /// Handles loading, saving, testing, and editing connection configurations.
    /// </summary>
    public class SettingsViewModel : BaseViewModel
    {
        /// <summary>
        /// Path to the application settings file.
        /// </summary>
        private readonly string _settingsPath = "appsettings.json";

        private ObservableCollection<DbConnectionSetting> _connections = [];

        /// <summary>
        /// Gets or sets the collection of database connection settings.
        /// </summary>
        public ObservableCollection<DbConnectionSetting> Connections
        {
            get => _connections;
            set => SetProperty(ref _connections, value);
        }

        /// <summary>
        /// Gets the list of available databases for the selected connection.
        /// </summary>
        public ObservableCollection<string> AvailableDatabases { get; } = [];

        /// <summary>
        /// Command to test the selected database connection.
        /// </summary>
        public ICommand TestConnectionCommand { get; }

        /// <summary>
        /// Command to apply and save the current connection settings.
        /// </summary>
        public ICommand ApplyCommand { get; }

        /// <summary>
        /// Command to cancel and close the settings window.
        /// </summary>
        public ICommand CancelCommand { get; }

        private DbConnectionSetting _selectedConnection;

        /// <summary>
        /// Gets or sets the currently selected database connection setting.
        /// When changed, reloads the list of available databases for the new connection.
        /// </summary>
        public DbConnectionSetting? SelectedConnection
        {
            get => _selectedConnection;
            set
            {
                if (SetProperty(ref _selectedConnection, value) && value != null)
                {
                    // Reload the list of databases whenever the connection changes
                    LoadDatabasesForConnection(value);
                }
            }
        }

        /// <summary>
        /// Command to add a new database connection setting.
        /// </summary>
        public ICommand AddConnectionCommand { get; }

        /// <summary>
        /// Command to remove the selected database connection setting.
        /// </summary>
        public ICommand RemoveConnectionCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsViewModel"/> class.
        /// Loads connection settings and sets up commands.
        /// </summary>
        public SettingsViewModel()
        {
            // 1) Load the dictionary of connection strings from settings
            var raw = AppSettings.Load(_settingsPath).ConnectionStrings;

            // 2) Parse each entry into a DbConnectionSetting
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

            Connections = [.. list];
            // 4) Set the default SelectedConnection and immediately load its databases
            SelectedConnection = Connections.FirstOrDefault();

            // 3) Initialize commands
            TestConnectionCommand = new RelayCommand<DbConnectionSetting>(async c =>
                await TestConnectionAsync(c)
            );

            ApplyCommand = new RelayCommand<object>(_ =>
            {
                // 4) Flatten back to Dictionary<string,string>
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
                // Add a new, empty entry
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

        /// <summary>
        /// Tests the database connection for the specified connection setting.
        /// Shows a message box with the result.
        /// </summary>
        /// <param name="c">The connection setting to test.</param>
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
        /// Loads the list of database names from the server specified in the given connection setting.
        /// Updates the <see cref="AvailableDatabases"/> collection.
        /// </summary>
        /// <param name="c">The connection setting for which to load databases.</param>
        private void LoadDatabasesForConnection(DbConnectionSetting c)
        {
            AvailableDatabases.Clear();
            // Build connection string to master database
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

            // Set the default database to the one in configuration, or the first available
            if (AvailableDatabases.Contains(c.Database))
                c.Database = c.Database;
            else if (AvailableDatabases.Count > 0)
                c.Database = AvailableDatabases[0];
        }
    }
}
