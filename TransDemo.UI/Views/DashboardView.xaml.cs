// DashboardView.xaml.cs
using System.Windows.Controls;

namespace TransDemo.UI.Views
{
    public partial class DashboardView : UserControl
    {
        public DashboardView()
        {
            InitializeComponent();
            // DataContext jest wstrzykiwany z DashboardTabViewModel przez IoC/Prism
        }
    }
}
