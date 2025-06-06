using TransDemo.UI.Views;
using TransDemo.UI.ViewModels;

namespace TransDemo.UI.ViewModels
{
    /// <summary>
    /// ViewModel for the Transfers tab in the application.
    /// Provides the header and content for the Transfers tab.
    /// </summary>
    public class TransfersTabViewModel : TabItemViewModel
    {
        /// <summary>
        /// Gets the header text displayed on the Transfers tab.
        /// </summary>
        public override string Header => "Transfers";

        /// <summary>
        /// Gets the content to be displayed within the Transfers tab.
        /// Typically, this is a <see cref="TransfersView"/> instance.
        /// </summary>
        public override object Content { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransfersTabViewModel"/> class.
        /// Sets up the view and its corresponding ViewModel.
        /// </summary>
        /// <param name="view">The <see cref="TransfersView"/> to be displayed in the tab.</param>
        /// <param name="vm">The <see cref="TransfersViewModel"/> to be used as the DataContext for the view.</param>
        public TransfersTabViewModel(
            TransfersView view,
            TransfersViewModel vm)
        {
            view.DataContext = vm;
            Content = view;
        }
    }
}
