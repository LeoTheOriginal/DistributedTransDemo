using TransDemo.UI.Views;
using TransDemo.UI.ViewModels;

namespace TransDemo.UI.ViewModels
{
    public class TransfersTabViewModel : TabItemViewModel
    {
        public override string Header => "Transfers";
        public override object Content { get; }

        public TransfersTabViewModel(
            TransfersView view,
            TransfersViewModel vm)
        {
            view.DataContext = vm;
            Content = view;
        }
    }
}
