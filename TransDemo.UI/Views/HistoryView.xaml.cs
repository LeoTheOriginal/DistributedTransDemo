using System.Windows.Controls;

namespace TransDemo.UI.Views
{
    /// <summary>
    /// Interaction logic for HistoryView.xaml.
    /// This class represents the view for displaying history in the application.
    /// </summary>
    public partial class HistoryView : UserControl
    {
        /// <summary>
        /// Initializes a new instance of the <see cref="HistoryView"/> class.
        /// </summary>
        public HistoryView()
        {
            InitializeComponent();
        }

        /// <summary>
        /// Handles the SelectionChanged event of the DataGrid control.
        /// This method is called whenever the selection changes in the associated DataGrid.
        /// </summary>
        /// <param name="sender">The source of the event, typically the DataGrid.</param>
        /// <param name="e">The event data containing information about the selection change.</param>
        private void DataGrid_SelectionChanged(object sender, SelectionChangedEventArgs e)
        {

        }
    }
}
