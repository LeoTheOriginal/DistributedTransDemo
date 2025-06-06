using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TransDemo.Data.Repositories;
using TransDemo.Models;

namespace TransDemo.UI.ViewModels
{
    /// <summary>
    /// ViewModel responsible for searching and selecting customers.
    /// Provides filtering, selection, and dialog result logic for customer search dialogs.
    /// </summary>
    public class SearchCustomerViewModel : BaseViewModel
    {
        /// <summary>
        /// Repository for accessing customer data.
        /// </summary>
        private readonly ICustomerRepository _custRepo;

        /// <summary>
        /// Gets the collection of customers matching the current filter.
        /// </summary>
        public ObservableCollection<Customer> Customers { get; } = new();

        /// <summary>
        /// Command to confirm the selection of a customer.
        /// </summary>
        public ICommand ConfirmCommand { get; }

        /// <summary>
        /// Command to cancel the customer selection dialog.
        /// </summary>
        public ICommand CancelCommand { get; }

        private string _filterText = "";

        /// <summary>
        /// Gets or sets the filter text used to search for customers.
        /// Setting this property reloads the customer list.
        /// </summary>
        public string FilterText
        {
            get => _filterText;
            set
            {
                SetProperty(ref _filterText, value);
                LoadCustomers();
            }
        }

        private Customer? _selected;

        /// <summary>
        /// Gets or sets the currently selected customer.
        /// </summary>
        public Customer? SelectedCustomer
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        /// <summary>
        /// Initializes a new instance of the <see cref="SearchCustomerViewModel"/> class.
        /// </summary>
        /// <param name="custRepo">The customer repository to use for data access.</param>
        public SearchCustomerViewModel(ICustomerRepository custRepo)
        {
            _custRepo = custRepo;
            ConfirmCommand = new RelayCommand(_ => Confirm(), _ => SelectedCustomer != null);
            CancelCommand = new RelayCommand(_ => Cancel());
            LoadCustomers();
        }

        /// <summary>
        /// Loads customers from the repository using the current filter text.
        /// Clears and repopulates the <see cref="Customers"/> collection.
        /// </summary>
        private void LoadCustomers()
        {
            Customers.Clear();
            foreach (var c in _custRepo.GetAll(FilterText))
                Customers.Add(c);
        }

        /// <summary>
        /// Confirms the selection and closes the dialog with a positive result.
        /// </summary>
        private void Confirm()
        {
            CloseDialog(true);
        }

        /// <summary>
        /// Cancels the selection and closes the dialog with a negative result.
        /// </summary>
        private void Cancel()
        {
            CloseDialog(false);
        }

        /// <summary>
        /// Closes the dialog window associated with this ViewModel.
        /// Sets the dialog result to the specified value.
        /// </summary>
        /// <param name="result">The dialog result to set (true for confirm, false for cancel).</param>
        private void CloseDialog(bool result)
        {
            if (Application.Current.Windows
                   .OfType<Window>()
                   .FirstOrDefault(w => w.DataContext == this)
                is Window wnd)
            {
                wnd.DialogResult = result;
                wnd.Close();
            }
        }
    }
}
