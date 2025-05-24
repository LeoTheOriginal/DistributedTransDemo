using TransDemo.Logic.Services;
using TransDemo.UI.Views;  // zakładamy, że masz DashboardView.xaml

namespace TransDemo.UI.ViewModels
{
    public class DashboardTabViewModel : TabItemViewModel
    {
        public override string Header => "Dashboard";
        public override object Content { get; }

        public DashboardTabViewModel(DashboardView view, DashboardViewModel vm)
        {
            view.DataContext = vm;
            Content = view;
        }


    }
}
