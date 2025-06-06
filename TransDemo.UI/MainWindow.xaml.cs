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
    /// <summary>
    /// Interaction logic for MainWindow.
    /// This is the main window of the application, responsible for initializing the UI and setting up the theme.
    /// </summary>
    public partial class MainWindow : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="MainWindow"/> class.
        /// Sets the DataContext to the provided <see cref="MainViewModel"/> and configures the Material Design theme.
        /// </summary>
        /// <param name="vm">The main view model for the application.</param>
        public MainWindow(MainViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;

            // Theme configuration using MaterialDesignThemes.Wpf
            // Sets the base theme to Light, primary color to Indigo, and secondary color to Lime.
            var palette = new PaletteHelper();
            var theme = palette.GetTheme();
            theme.SetBaseTheme(BaseTheme.Light);
            theme.SetPrimaryColor(Colors.Indigo);
            theme.SetSecondaryColor(Colors.Lime);
            palette.SetTheme(theme);
        }
    }
}
