using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.Collections.Generic;
using System.Transactions;
using System.Windows;
using TransDemo.Data.Repositories;
using TransDemo.Logic.Services;
using TransDemo.UI.Models;
using TransDemo.UI.ViewModels;
using TransDemo.UI.Views;

namespace TransDemo.UI
{
    /// <summary>
    /// Interaction logic for the WPF application.
    /// Handles application startup, dependency injection configuration, and service provider setup.
    /// </summary>
    public partial class App : Application
    {
        /// <summary>
        /// Holds the application's root service provider instance.
        /// </summary>
        private IServiceProvider? _provider;

        /// <summary>
        /// Gets the application's root service provider for resolving dependencies.
        /// </summary>
        public static IServiceProvider Services { get; private set; } = null!;

        /// <summary>
        /// Handles application startup logic, including configuration loading, dependency injection setup, and main window initialization.
        /// </summary>
        /// <param name="e">Startup event arguments.</param>
        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // Enable distributed transactions for System.Transactions
            AppContext.SetSwitch("System.Transactions.EnableDistributedTransactions", true);
            TransactionManager.ImplicitDistributedTransactions = true;

            // 0) Load configuration from appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 1) Load custom application settings
            var settings = configuration.Get<AppSettings>();

            // 2) Configure dependency injection
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton(settings);

            // Retrieve connection strings
            var connCentral = configuration.GetConnectionString("CentralDB")!;
            var connB1 = configuration.GetConnectionString("Branch1DB")!;
            var connB2 = configuration.GetConnectionString("Branch2DB")!;

            // Register central bank repository (sp_TransferCentral)
            services.AddSingleton<ICentralBankRepository>(_ =>
                new SqlCentralBankRepository(connCentral));

            // Register branch repositories mapping (BranchId → repository)
            services.AddSingleton<IDictionary<int, IBranchClientRepository>>(sp => new Dictionary<int, IBranchClientRepository>
            {
                [1] = new SqlBranchClientRepository(connB1),
                [2] = new SqlBranchClientRepository(connB2)
            });

            // Register history repository (central and branches)
            services.AddSingleton<IHistoryRepository>(sp =>
                new SqlHistoryRepository(connCentral, connB1, connB2));

            // Register history query service
            services.AddTransient<HistoryQueryService>();

            // Register account and customer repositories
            services.AddSingleton<IAccountRepository>(_ => new SqlAccountRepository(connCentral));
            services.AddSingleton<ICustomerRepository>(_ => new SqlCustomerRepository(connCentral));

            // Register search dialog and view model
            services.AddTransient<SearchCustomerView>();
            services.AddTransient<SearchCustomerViewModel>();

            // Register distributed transfer repository
            services.AddSingleton<IDistributedTransferRepository>(
                _ => new SqlDistributedTransferRepository(connCentral));

            // Register transfer and statistics services
            services.AddTransient<TransferService>();
            services.AddTransient<StatisticsQueryService>();
            services.AddTransient<TransactionStatsService>();
            services.AddTransient<DashboardStatsService>();

            // Register ViewModels and Views for the application
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsView>();
            services.AddTransient<DashboardView>();
            services.AddTransient<DashboardViewModel>();
            services.AddTransient<DashboardTabViewModel>();
            services.AddTransient<TransfersView>();
            services.AddTransient<TransfersViewModel>();
            services.AddTransient<TransfersTabViewModel>();
            services.AddTransient<HistoryView>();
            services.AddTransient<HistoryViewModel>();
            services.AddTransient<HistoryTabViewModel>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();

            // Build the service provider and assign to static property
            _provider = services.BuildServiceProvider();
            Services = _provider;

            // 3) Start the main window
            var mainWin = _provider.GetRequiredService<MainWindow>();
            mainWin.Show();
        }
    }
}
    