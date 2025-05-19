// MainWindow.xaml.cs
using System;
using System.Windows;
using System.Windows.Media;
using Microsoft.Extensions.Configuration;
using MaterialDesignThemes.Wpf;
using TransDemo.Data.Repositories;
using TransDemo.Logic.Services;
using TransDemo.UI.ViewModels;


namespace TransDemo.UI
{
    public partial class MainWindow : Window
    {
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            // teraz konfiguracja motywu:
            var palette = new PaletteHelper();
            var theme = palette.GetTheme();
            theme.SetBaseTheme(BaseTheme.Light);
            theme.SetPrimaryColor(Colors.Indigo);
            theme.SetSecondaryColor(Colors.Lime);
            palette.SetTheme(theme);
        }
    }
}
