using TransDemo.Logic.Services;
using TransDemo.UI.Views;

namespace TransDemo.UI.ViewModels
{
    /// <summary>
    /// ViewModel for the History tab in the application's tab control.
    /// Provides the header and content (view) for the history tab.
    /// </summary>
    public class HistoryTabViewModel : TabItemViewModel
    {
        /// <summary>
        /// Gets the header text for the history tab.
        /// </summary>
        public override string Header => "History";

        /// <summary>
        /// Gets the content (view) associated with the history tab.
        /// </summary>
        public override object Content { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryTabViewModel"/> class.
        /// Sets up the view and binds it to the provided <see cref="HistoryViewModel"/>.
        /// </summary>
        /// <param name="vm">The view model for the history view.</param>
        public HistoryTabViewModel(HistoryViewModel vm)
        {
            var view = new HistoryView
            {
                DataContext = vm
            };
            Content = view;
        }
    }
}
