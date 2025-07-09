using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SplittLib.Models;
using Splitt.Services;
using Splitt.ViewModels.Wrappers;
using Microsoft.Maui.Controls.Shapes;

namespace Splitt.ViewModels

{
    [QueryProperty(nameof(CheckId), "checkId")]

    public partial class SplitCheckItemViewModel : ObservableObject
    {
        private readonly CheckItemClient CheckItemClient = new CheckItemClient();
        private readonly CheckMemberClient CheckMemberClient = new CheckMemberClient();

        [ObservableProperty]
        private int _checkId = 0;

        [ObservableProperty]
        private string _memberName = "New Payee";

        [ObservableProperty]
        private ObservableCollection<CheckMemberWrapper> _checkMembers = [];

        [ObservableProperty]
        public ObservableCollection<CheckItemWrapper> checkItemWrapper = [];

        [ObservableProperty]
        public ObservableCollection<CheckItem> checkItems = [];
        partial void OnCheckIdChanged(int value)
        {
            // Called automatically when Shell sets the CheckId
            _ = LoadCheckAsync();
        }

        private async Task LoadCheckAsync()
        {
            await LoadCheckItemsAsync();
            await LoadCheckMembersAsync();
        }

        public async Task LoadCheckItemsAsync()
        {
            try
            {
                List<CheckItem> checkItemList = await CheckItemClient.GetCheckItems(CheckId);

                if (checkItemList != null)
                {
                    CheckItems.Clear();
                    foreach (var checkItem in checkItemList)
                    {
                        CheckItems.Add(checkItem);
                        CheckItemWrapper.Add(new CheckItemWrapper
                        {
                            Item = checkItem,
                            AvailableMembers = new ObservableCollection<CheckMemberWrapper>(),
                        });
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching checks: {ex.Message}");
            }

        }

        public async Task LoadCheckMembersAsync()
        {
            try
            {
                List<CheckMember> checkMemberList = await CheckMemberClient.GetCheckMembersByCheck(CheckId);

                if (checkMemberList != null)
                {
                    CheckMembers.Clear();
                    foreach (var checkMember in checkMemberList)
                    {
                        var wrapper = new CheckMemberWrapper(checkMember);
                        CheckMembers.Add(wrapper);
                        foreach (var itemWrapper in CheckItemWrapper)
                        {
                            itemWrapper.AvailableMembers.Add(wrapper);
                        }
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error fetching check members: {ex.Message}");
            }
        }

        [RelayCommand]
        private async Task AddCheckMember()
        {
            if (string.IsNullOrWhiteSpace(MemberName))
            {
                await Shell.Current.DisplayAlert("Error", "Please enter member name", "OK");
                return;
            }
            Dictionary<string, object?> parameters = new Dictionary<string, object?>
            {
                { "checkId", CheckId },
                { "Name", MemberName },
                { "userId", null }, // Assuming userId is not required for now
                { "amountOwed", 0m } // Default amount owed
            };

            var checkMember = await CheckMemberClient.CreateCheckMember(parameters);

            if (checkMember != null)
            {
                var vm = new CheckMemberWrapper(checkMember);
                CheckMembers.Add(vm);
                MemberName = "New Check Member"; // Reset the member name for the next input
                foreach (var wrapper in CheckItemWrapper)
                {
                    wrapper.AvailableMembers.Add(vm);
                }
            }
            else
            {
                await Shell.Current.DisplayAlert("Error", "Failed to add check member", "OK");
                return;
            }

        }

        [RelayCommand]
        private async Task DeleteCheckMember(CheckMemberWrapper checkMember)
        {
            if (checkMember == null)
            {
                await Shell.Current.DisplayAlert("Error", "Check member not found", "OK");
                return;
            }
            int response = await CheckMemberClient.DeleteCheckMember(checkMember.Id);
            if (response == -1)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to delete check member", "OK");
                return;
            }
            CheckMembers.Remove(checkMember);
            foreach (var wrapper in CheckItemWrapper)
            {
                wrapper.AvailableMembers.Remove(checkMember);

            }
        }



        [RelayCommand]
        private async Task AssignItemToMember()
        {
            var updateParameters = new List<Dictionary<string, object>>();
            foreach (var member in CheckMembers)
            {
                var parameters = new Dictionary<string, object>
                {
                    { "id", member.Id },
                    { "amountOwed", member.AmountOwed }
                };
                updateParameters.Add(parameters);
            }

            var updated = await CheckMemberClient.UpdateCheckMembers(updateParameters);
            if (updated == null)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to update check members.", "OK");
                return;
            }

        }


    }
}