using System.Windows;
using TransDemo.UI.ViewModels;

namespace TransDemo.UI.Views
{
    public partial class SettingsView : Window
    {
        public SettingsView(SettingsViewModel vm)
        {
            InitializeComponent();
            DataContext = vm;
        }
    }
}
