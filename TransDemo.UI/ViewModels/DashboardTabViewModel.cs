using TransDemo.Logic.Services;
using TransDemo.UI.Views;  // zakładamy, że masz DashboardView.xaml

namespace TransDemo.UI.ViewModels
{
    /// <summary>
    /// ViewModel for the Dashboard tab in the main application window.
    /// Provides the header and content (UserControl) for the dashboard tab.
    /// </summary>
    public class DashboardTabViewModel : TabItemViewModel
    {
        /// <summary>
        /// Gets the header text displayed on the tab.
        /// </summary>
        public override string Header => "Dashboard";

        /// <summary>
        /// Gets the content of the tab, which is the DashboardView UserControl.
        /// </summary>
        public override object Content { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="DashboardTabViewModel"/> class.
        /// Sets the DataContext of the provided DashboardView to the given DashboardViewModel.
        /// </summary>
        /// <param name="view">The DashboardView UserControl to be displayed in the tab.</param>
        /// <param name="vm">The DashboardViewModel to be used as the DataContext for the view.</param>
        public DashboardTabViewModel(DashboardView view, DashboardViewModel vm)
        {
            view.DataContext = vm;
            Content = view;
        }
    }
}
