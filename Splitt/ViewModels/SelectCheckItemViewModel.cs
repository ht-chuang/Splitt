using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SplittLib.Models;
using SplittLib.Services;

namespace Splitt.ViewModels

{
    [QueryProperty(nameof(CheckId), "checkId")]

    public partial class SelectCheckItemViewModel : ObservableObject
    {

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
        private void AddCheckItem()
        {
            var checkItem = CheckItemService.CreateCheckItem(Name, Description, Quantity, UnitPrice, TotalPrice, CheckId);
            CheckItems.Add(checkItem);
            UpdateSubtotalAndTax();
            UpdateTotal();
        }
        [RelayCommand]
        private void RemoveItem(CheckItem item)
        {
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
            Tax = Subtotal * 0.0725m;
        }

        partial void OnTipChanged(decimal oldValue, decimal newValue)
        {
            UpdateTotal();
        }

        private void UpdateTotal()
        {
            Total = Subtotal + Tax + Tip;
        }
    }

}