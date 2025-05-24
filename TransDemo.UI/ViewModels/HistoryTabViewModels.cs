using TransDemo.Logic.Services;
using TransDemo.UI.Views;

namespace TransDemo.UI.ViewModels
{
    public class HistoryTabViewModel : TabItemViewModel
    {
        public override string Header => "History";
        public override object Content { get; }

        public HistoryTabViewModel(TransferService svc)
        {
            Content = new HistoryView();
        }
    }
}
