namespace TransDemo.UI.ViewModels
{
    /// <summary>
    /// Abstract base ViewModel for tab items in the application.
    /// Provides properties for the tab header and the content (typically a UserControl).
    /// </summary>
    public abstract class TabItemViewModel
    {
        /// <summary>
        /// Gets the header text displayed on the tab.
        /// </summary>
        public abstract string Header { get; }

        /// <summary>
        /// Gets the content to be displayed within the tab.
        /// Typically, this is a UserControl instance.
        /// </summary>
        public abstract object Content { get; }
    }
}
