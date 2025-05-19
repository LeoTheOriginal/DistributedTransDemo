using TransDemo.Logic.Services;
using TransDemo.UI.Views;

namespace TransDemo.UI.ViewModels
{
    public class TransfersTabViewModel : TabItemViewModel
    {
        public override string Header => "Transfers";
        public override object Content { get; }

        public TransfersTabViewModel(DistributedTransactionService svc)
        {
            Content = new TransfersView();
            // zakładamy, że TransfersView ma swój ViewModel
        }
    }
}
