// DashboardView.xaml.cs
using System.Windows.Controls;

namespace TransDemo.UI.Views
{
    /// <summary>
    /// Interaction logic for <see cref="DashboardView"/>.
    /// This view is responsible for displaying the dashboard UI in the application.
    /// The DataContext is injected via IoC/Prism and is expected to be an instance of DashboardTabViewModel.
    /// </summary>
    public partial class DashboardView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardView"/> class.
        /// Sets up the component and prepares it for use in the UI.
        /// </summary>
        public DashboardView()
        {
            InitializeComponent();
            // DataContext is injected from DashboardTabViewModel via IoC/Prism
        }
    }
}
    