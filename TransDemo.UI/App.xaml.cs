using Microsoft.Extensions.DependencyInjection;
using System;
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
        private IServiceProvider _provider;

        protected override void OnStartup(StartupEventArgs e)
        {
            base.OnStartup(e);

            // 1) Wczytaj ustawienia i conn-strings
            var settings = AppSettings.Load("appsettings.json");
            var cs = settings.ConnectionStrings;
            var connCentral = cs["CentralDB"];
            var connB1 = cs["Branch1DB"];
            var connB2 = cs["Branch2DB"];

            // 2) Zarejestruj w Microsoft DI
            var services = new ServiceCollection();
            services.AddSingleton(settings);
            services.AddSingleton<IHistoryRepository>(sp =>
                new SqlHistoryRepository(connB1, connB2));
            services.AddTransient<DistributedTransactionService>();
            services.AddTransient<SettingsViewModel>();
            services.AddTransient<SettingsView>();
            services.AddTransient<MainViewModel>();
            services.AddTransient<MainWindow>();

            _provider = services.BuildServiceProvider();

            // 3) Resolve i Show
            var mainWin = _provider.GetRequiredService<MainWindow>();
            // Opcjonalnie: ustaw theme tutaj, przed Show
            mainWin.Show();
        }
    }
}
