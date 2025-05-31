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
    public partial class App : Application
    {
        private IServiceProvider? _provider;
        public static IServiceProvider Services { get; private set; } = null!;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // in App.OnStartup, before any service resolution…
            AppContext.SetSwitch("System.Transactions.EnableDistributedTransactions", true);
            TransactionManager.ImplicitDistributedTransactions = true;

            // 0) Konfiguracja z appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 1) Opcjonalnie własne ustawienia
            var settings = configuration.Get<AppSettings>();

            // 2) DI
            var services = new ServiceCollection();
            services.AddSingleton<IConfiguration>(configuration);
            services.AddSingleton(settings);

            // Connection strings
            var connCentral = configuration.GetConnectionString("CentralDB")!;
            var connB1 = configuration.GetConnectionString("Branch1DB")!;
            var connB2 = configuration.GetConnectionString("Branch2DB")!;

            // --- centralne repozytorium procedury sp_TransferCentral
            services.AddSingleton<ICentralBankRepository>(_ =>
                new SqlCentralBankRepository(connCentral));

            // --- mapowanie oddziałów BranchId→repozytorium
            services.AddSingleton<IDictionary<int, IBranchClientRepository>>(sp => new Dictionary<int, IBranchClientRepository>
            {
                [1] = new SqlBranchClientRepository(connB1),
                [2] = new SqlBranchClientRepository(connB2)
            });

            services.AddSingleton<IHistoryRepository>(sp =>
                new SqlHistoryRepository(connCentral, connB1, connB2));

            services.AddTransient<HistoryQueryService>();

            // konto i klient
            services.AddSingleton<IAccountRepository>(_ => new SqlAccountRepository(connCentral));
            services.AddSingleton<ICustomerRepository>(_ => new SqlCustomerRepository(connCentral));

            // search dialog
            services.AddTransient<SearchCustomerView>();
            services.AddTransient<SearchCustomerViewModel>();

            services.AddSingleton<IDistributedTransferRepository>(
                _ => new SqlDistributedTransferRepository(connCentral));
            // transfer service
            services.AddTransient<TransferService>();
            services.AddTransient<StatisticsQueryService>();
            services.AddTransient<TransactionStatsService>();
            services.AddTransient<DashboardStatsService>();


            // 2.2) ViewModels + Views
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

            _provider = services.BuildServiceProvider();
            Services = _provider;

            // 3) start
            var mainWin = _provider.GetRequiredService<MainWindow>();
            mainWin.Show();
        }
    }
}
