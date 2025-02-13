using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splitt.Views;

namespace Splitt.ViewModels
{
    public partial class ManageCheckViewModel : ObservableObject
    {

        [RelayCommand]
        private async Task Submit()
        {
            await Shell.Current.GoToAsync(nameof(NewCheckView));
        }
    }

}