using TransDemo.Logic.Services;
using TransDemo.UI.Views;  // zakładamy, że masz DashboardView.xaml

namespace TransDemo.UI.ViewModels
{
    public class DashboardTabViewModel : TabItemViewModel
    {
        public override string Header => "Dashboard";
        public override object Content { get; }

        public DashboardTabViewModel(DistributedTransactionService svc)
        {
            // tutaj w przyszłości możesz przekazać dane do DashboardViewModel
            Content = new DashboardView();
        }
    }
}
