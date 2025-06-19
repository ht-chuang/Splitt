using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splitt.Views;
using SplittLib.Models;
using Splitt.Services;

namespace Splitt.ViewModels

{
    [QueryProperty(nameof(CheckId), "checkId")]

    public partial class SelectCheckItemViewModel : ObservableObject
    {

        private readonly CheckItemClient CheckItemClient = new CheckItemClient();

        [ObservableProperty]
        private int _checkId = 0;
        [ObservableProperty]
        private ObservableCollection<CheckItem> _checkItems = [];

        [ObservableProperty]
        private string _name = "New Check Item";

        [ObservableProperty]
        private string _description = "Check Item Description";

        [ObservableProperty]
        private int _quantity = 0;

        [ObservableProperty]
        private decimal _unitPrice = 0;

        [ObservableProperty]
        private decimal _totalPrice = 0;

        [ObservableProperty]
        private decimal _subtotal = 0;

        [ObservableProperty]
        private decimal _tax = 0;

        [ObservableProperty]
        private decimal _tip = 0;
        [ObservableProperty]
        private decimal _total = 0;


        [RelayCommand]
        private async Task AddCheckItem()
        {
            if (string.IsNullOrWhiteSpace(Name) || string.IsNullOrWhiteSpace(Description) || Quantity <= 0 || UnitPrice <= 0)
            {
                await Shell.Current.DisplayAlert("Error", "Please fill in all fields correctly", "OK");
                return;
            }
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "name", Name },
                { "description", Description },
                { "quantity", Quantity },
                { "unitPrice", UnitPrice },
                { "totalPrice", TotalPrice },
                { "checkId", CheckId }
            };

            CheckItem? checkItem = await CheckItemClient.CreateCheckItem(parameters);

            if (checkItem == null)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to create check.", "OK");
                return;
            }

            CheckItems.Add(checkItem);
            UpdateSubtotalAndTax();
            UpdateTotal();
        }

        [RelayCommand]
        private async Task RemoveItem(CheckItem item)
        {
            int response = await CheckItemClient.DeleteCheckItem(item.Id);
            if (response == -1)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to delete check item.", "OK");
                return;
            }
            CheckItems.Remove(item);
            UpdateSubtotalAndTax();
            UpdateTotal();
        }

        partial void OnQuantityChanged(int oldValue, int newValue)
        {
            UpdateTotalPrice();
        }
        partial void OnUnitPriceChanged(decimal oldValue, decimal newValue)
        {
            UpdateTotalPrice();
        }

        private void UpdateTotalPrice()
        {
            TotalPrice = Quantity * UnitPrice;
        }

        private void UpdateSubtotalAndTax()
        {
            Subtotal = CheckItems.Sum(item => item.TotalPrice);
            Tax = Math.Round(Subtotal * 0.0725m, 2);
        }

        partial void OnTipChanged(decimal oldValue, decimal newValue)
        {
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            Total = Subtotal + Tax + Tip;
        }

        [RelayCommand]
        private async Task Submit()
        {
            await Shell.Current.GoToAsync($"{nameof(SplitCheckItemView)}?checkId={CheckId}");
        }
    }

}