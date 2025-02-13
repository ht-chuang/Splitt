using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splitt.Views;
using Splitt.Services;

namespace Splitt.ViewModels
{
    public partial class NewCheckViewModel : ObservableObject
    {
        private readonly CheckClient CheckClient = new CheckClient();

        private readonly UserClient UserClient = new UserClient();

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
            // var current_user = await UserClient.GetUser(13);
            Dictionary<string, object> parameters = new Dictionary<string, object>
            {
                { "title", Title },
                { "ownerId", 1 },
                { "date", DateTime.SpecifyKind(Date, DateTimeKind.Utc) },
                { "subtotal", Subtotal },
                { "tax", Tax },
                { "tip", Tip },
                { "total", Total }
            };

            var response = await CheckClient.CreateCheck(parameters);

            await Shell.Current.GoToAsync(nameof(NewCheckItemView));
        }
    }

}