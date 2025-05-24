using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows.Input;
using TransDemo.Data.Repositories;
using TransDemo.Logic.Services;
using TransDemo.Models;
using TransDemo.UI.Views;
using Microsoft.Extensions.DependencyInjection;
using System.Windows;


namespace TransDemo.UI.ViewModels
{
    public class TransfersViewModel : BaseViewModel
    {
        private readonly IAccountRepository _acctRepo;
        private readonly ICustomerRepository _custRepo;
        private readonly TransferService _transferSvc;

        public ObservableCollection<Account> Accounts { get; } = new();

        private Account? _from;
        public Account? SelectedFromAccount
        {
            get => _from;
            set
            {
                if (SetProperty(ref _from, value))
                    OnPropertyChanged(nameof(CanExecuteTransfer));
            }
        }

        private Account? _to;
        public Account? SelectedToAccount
        {
            get => _to;
            set
            {
                SetProperty(ref _to, value);
                OnPropertyChanged(nameof(CanExecuteTransfer));
            }
        }

        private decimal _amount;
        public string TransferAmount
        {
            get => _amount.ToString();
            set
            {
                if (decimal.TryParse(value, out var d))
                {
                    _amount = d;
                    ErrorMessage = "";
                }
                else
                {
                    ErrorMessage = "Nieprawidłowa kwota.";
                }
                OnPropertyChanged(nameof(TransferAmount));
                OnPropertyChanged(nameof(CanExecuteTransfer));
            }
        }

        private string _description = "";
        public string TransferDescription
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _error = "";
        public string ErrorMessage
        {
            get => _error;
            set => SetProperty(ref _error, value);
        }

        public bool CanExecuteTransfer =>
               SelectedFromAccount != null
            && SelectedToAccount != null
            && SelectedFromAccount.AccountId != SelectedToAccount.AccountId
            && _amount > 0
            && SelectedFromAccount.Balance >= _amount;

        public ICommand SearchFromAccountCommand { get; }
        public ICommand SearchToAccountCommand { get; }
        public ICommand ExecuteTransferCommand { get; }
        public ICommand ExecuteTransferWithErrorCommand { get; }

        public TransfersViewModel(
            IAccountRepository acctRepo,
            ICustomerRepository custRepo,
            TransferService transferSvc)
        {
            _acctRepo = acctRepo;
            _custRepo = custRepo;
            _transferSvc = transferSvc;

            SearchFromAccountCommand = new RelayCommand(_ => PickClientAndAccount(a => SelectedFromAccount = a));
            SearchToAccountCommand = new RelayCommand(_ => PickClientAndAccount(a => SelectedToAccount = a));
            ExecuteTransferCommand = new RelayCommand(_ => Execute(false), _ => CanExecuteTransfer);
            ExecuteTransferWithErrorCommand = new RelayCommand(_ => Execute(true), _ => CanExecuteTransfer);

            LoadAccounts();
        }

        private void LoadAccounts()
        {
            Accounts.Clear();
            foreach (var a in _acctRepo.GetAll())
                Accounts.Add(a);
        }

        private void PickClientAndAccount(Action<Account> assign)
        {
            // Pobieramy VM i widok z DI
            var vm = App.Services.GetRequiredService<SearchCustomerViewModel>();
            var dlg = App.Services.GetRequiredService<SearchCustomerView>();

            // Podłączamy ViewModel
            dlg.DataContext = vm;

            // Ustawiamy właściciela (MainWindow) i dzięki CenterOwner OKNO będzie wyśrodkowane
            dlg.Owner = Application.Current.MainWindow;

            // Pokaż dialog modalnie
            if (dlg.ShowDialog() == true && vm.SelectedCustomer != null)
            {
                var acc = Accounts.FirstOrDefault(x => x.CustomerId == vm.SelectedCustomer.CustomerId);
                if (acc != null) assign(acc);
            }
        }


        private void Execute(bool simulateError)
        {
            ErrorMessage = "";
            try
            {
                _transferSvc.ExecuteDistributedTransfer(
                    SelectedFromAccount!, SelectedToAccount!, _amount, simulateError);

                // odświeżamy salda
                LoadAccounts();
            }
            catch (Exception ex)
            {
                ErrorMessage = ex.Message;
            }
        }
    }
}
