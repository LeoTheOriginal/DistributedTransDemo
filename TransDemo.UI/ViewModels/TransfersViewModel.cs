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
    /// <summary>
    /// ViewModel responsible for handling transfer operations between accounts.
    /// Provides properties and commands for selecting accounts, entering transfer details, and executing transfers.
    /// </summary>
    public class TransfersViewModel : BaseViewModel
    {
        private readonly IAccountRepository _acctRepo;
        private readonly ICustomerRepository _custRepo;
        private readonly TransferService _transferSvc;

        /// <summary>
        /// Gets the collection of all available accounts.
        /// </summary>
        public ObservableCollection<Account> Accounts { get; } = new();

        private Account? _from;
        /// <summary>
        /// Gets or sets the selected source account for the transfer.
        /// </summary>
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
        /// <summary>
        /// Gets or sets the selected destination account for the transfer.
        /// </summary>
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
        /// <summary>
        /// Gets or sets the transfer amount as a string. Validates and updates the internal decimal value.
        /// </summary>
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
        /// <summary>
        /// Gets or sets the description for the transfer.
        /// </summary>
        public string TransferDescription
        {
            get => _description;
            set => SetProperty(ref _description, value);
        }

        private string _error = "";
        /// <summary>
        /// Gets or sets the error message to display in the UI.
        /// </summary>
        public string ErrorMessage
        {
            get => _error;
            set => SetProperty(ref _error, value);
        }

        /// <summary>
        /// Gets a value indicating whether the transfer can be executed based on current selections and amount.
        /// </summary>
        public bool CanExecuteTransfer =>
               SelectedFromAccount != null
            && SelectedToAccount != null
            && SelectedFromAccount.AccountId != SelectedToAccount.AccountId
            && _amount > 0
            && SelectedFromAccount.Balance >= _amount;

        /// <summary>
        /// Command to search and select the source account.
        /// </summary>
        public ICommand SearchFromAccountCommand { get; }
        /// <summary>
        /// Command to search and select the destination account.
        /// </summary>
        public ICommand SearchToAccountCommand { get; }
        /// <summary>
        /// Command to execute the transfer.
        /// </summary>
        public ICommand ExecuteTransferCommand { get; }
        /// <summary>
        /// Command to execute the transfer with simulated error.
        /// </summary>
        public ICommand ExecuteTransferWithErrorCommand { get; }

        /// <summary>
        /// Initializes a new instance of the <see cref="TransfersViewModel"/> class.
        /// </summary>
        /// <param name="acctRepo">The account repository.</param>
        /// <param name="custRepo">The customer repository.</param>
        /// <param name="transferSvc">The transfer service.</param>
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

        /// <summary>
        /// Loads all accounts from the repository into the <see cref="Accounts"/> collection.
        /// </summary>
        private void LoadAccounts()
        {
            Accounts.Clear();
            foreach (var a in _acctRepo.GetAll())
                Accounts.Add(a);
        }

        /// <summary>
        /// Opens a dialog to select a customer and assigns the corresponding account using the provided action.
        /// </summary>
        /// <param name="assign">The action to assign the selected account.</param>
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

        /// <summary>
        /// Executes the transfer operation using the transfer service.
        /// Optionally simulates an error if <paramref name="simulateError"/> is true.
        /// Updates the error message and reloads accounts after execution.
        /// </summary>
        /// <param name="simulateError">If true, simulates an error during transfer.</param>
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
