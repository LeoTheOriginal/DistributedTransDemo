using Microsoft.Extensions.Configuration;
using Microsoft.Extensions.DependencyInjection;
using System;
using System.IO;
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

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 0) Zbuduj konfigurację z appsettings.json
            var configuration = new ConfigurationBuilder()
                .SetBasePath(AppDomain.CurrentDomain.BaseDirectory)
                .AddJsonFile("appsettings.json", optional: false, reloadOnChange: true)
                .Build();

            // 1) Wczytaj własny obiekt ustawień (opcjonalnie)
            var settings = configuration.Get<AppSettings>();

            // 2) Zarejestruj w DI
            var services = new ServiceCollection();

            // rejestruj IConfiguration, dzięki czemu HistoryQueryService je dostanie
            services.AddSingleton<IConfiguration>(configuration);

            // zarejestruj swój AppSettings, jeśli dalej go używasz
            services.AddSingleton(settings);

            var connCentral = configuration.GetConnectionString("CentralDB")!;

            // repozytorium centralne
            services.AddSingleton<IHistoryRepository>(sp =>
                new SqlHistoryRepository(
                    configuration.GetConnectionString("Branch1DB")!,
                    configuration.GetConnectionString("Branch2DB")!));

            services.AddTransient<DistributedTransactionService>();
            services.AddTransient<HistoryQueryService>();

            // 2.2 ViewModels + Views
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

            // 3) resolve i start
            var mainWin = _provider.GetRequiredService<MainWindow>();
            mainWin.Show();
        }
    }
}
