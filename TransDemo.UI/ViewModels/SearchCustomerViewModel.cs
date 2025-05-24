using System;
using System.Collections.ObjectModel;
using System.Linq;
using System.Windows;
using System.Windows.Input;
using TransDemo.Data.Repositories;
using TransDemo.Models;

namespace TransDemo.UI.ViewModels
{
    public class SearchCustomerViewModel : BaseViewModel
    {
        private readonly ICustomerRepository _custRepo;
        public ObservableCollection<Customer> Customers { get; } = new();
        public ICommand ConfirmCommand { get; }
        public ICommand CancelCommand { get; }

        private string _filterText = "";
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
        public Customer? SelectedCustomer
        {
            get => _selected;
            set => SetProperty(ref _selected, value);
        }

        public SearchCustomerViewModel(ICustomerRepository custRepo)
        {
            _custRepo = custRepo;
            ConfirmCommand = new RelayCommand(_ => Confirm(), _ => SelectedCustomer != null);
            CancelCommand = new RelayCommand(_ => Cancel());
            LoadCustomers();
        }

        private void LoadCustomers()
        {
            Customers.Clear();
            foreach (var c in _custRepo.GetAll(FilterText))
                Customers.Add(c);
        }

        private void Confirm()
        {
            CloseDialog(true);
        }

        private void Cancel()
        {
            CloseDialog(false);
        }

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
