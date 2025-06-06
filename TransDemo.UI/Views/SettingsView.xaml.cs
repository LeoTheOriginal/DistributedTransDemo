using System.Windows;
using TransDemo.UI.ViewModels;

namespace TransDemo.UI.Views
{
    /// <summary>
    /// Interaction logic for <see cref="SettingsView"/>.
    /// This window allows the user to view and modify application database connection settings.
    /// </summary>
    public partial class SettingsView : Window
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="SettingsView"/> class.
        /// Sets the data context to the provided <see cref="SettingsViewModel"/>.
        /// </summary>
        /// <param name="vm">The view model containing logic and data for the settings view.</param>
        public SettingsView(SettingsViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
