using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splitt.Views;
using Splitt.Services;
using SplittLib.Models;
using System.Collections.ObjectModel;


namespace Splitt.ViewModels
{
    public partial class NewCheckViewModel : ObservableObject
    {
        private readonly CheckClient CheckClient = new CheckClient();

        // private readonly CheckItemClient CheckItemClient = new CheckItemClient();

        // public ObservableCollection<CheckItem> CheckItems { get; set; }

        // public NewCheckViewModel()
        // {
        //     CheckItems = new ObservableCollection<CheckItem>();
        //     _ = LoadChecksAsync(); // Fetch data on initialization
        // }

        // private async Task LoadChecksAsync()
        // {
        //     try
        //     {
        //         var checkItemList = await CheckItemClient.GetCheckItems();

        //         if (checkItemList != null)
        //         {
        //             CheckItems.Clear();
        //             foreach (var check in checkItemList)
        //             {
        //                 CheckItems.Add(check);
        //             }
        //         }
        //     }
        //     catch (Exception ex)
        //     {
        //         Console.WriteLine($"Error fetching checks: {ex.Message}");
        //     }
        // }

        [ObservableProperty]
        private string _title = "New Check";

        [ObservableProperty]
        private int _ownerId = 1;

        [ObservableProperty]
        private DateTime _date = DateTime.UtcNow;

        [ObservableProperty]
        private decimal _subtotal = 0;

        [ObservableProperty]
        private decimal _tax = 0;

        [ObservableProperty]
        private decimal _tip = 0;

        [ObservableProperty]
        private decimal _total = 0;

        [RelayCommand]
        private async Task Submit()
        {
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "title", Title },
                { "ownerId", 1 },
                { "date", DateTime.SpecifyKind(Date, DateTimeKind.Utc) },
            };

            int newCheckId = await CheckClient.CreateCheck(parameters);

            if (newCheckId == -1)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to create check.", "OK");
                return; // Don't navigate
            }

            // Only navigate if creation was successful
            await Shell.Current.GoToAsync($"{nameof(SelectCheckItemView)}?checkId={newCheckId}");

        }
    }

}