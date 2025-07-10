using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using SplittLib.Models;
using Splitt.Services;
using Splitt.Views;
using Splitt.ViewModels.Wrappers;

namespace Splitt.ViewModels

{
    [QueryProperty(nameof(CheckId), "checkId")]
    [QueryProperty(nameof(Tip), "tip")]
    [QueryProperty(nameof(Tax), "tax")]

    public partial class SplitCheckItemViewModel : ObservableObject
    {
        private readonly CheckItemClient CheckItemClient = new CheckItemClient();
        private readonly CheckMemberClient CheckMemberClient = new CheckMemberClient();

        [ObservableProperty]
        private int _checkId = 0;

        [ObservableProperty]
        private decimal _tip = 0;
        [ObservableProperty]
        private decimal _tax = 0;

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

        // This method could be improved. Placeholder for now.
        private void UpdateMemberTipTax(List<CheckMemberWrapper> currentMembers, List<CheckMemberWrapper> previousMembers)
        {
            if (currentMembers.Count == 0)
            {
                return; // No members to update
            }

            int totalTipTaxCents = (int)(Tip * 100) + (int)(Tax * 100); // Assuming Tip includes tax for simplicity

            // 1. Undo previous split
            if (previousMembers.Count > 0)
            {

                int prevBaseCents = totalTipTaxCents / previousMembers.Count;
                int prevRemainderCents = totalTipTaxCents % previousMembers.Count;

                decimal prevSplitAmount = prevBaseCents / 100m; // Convert back to decimal
                decimal prevFirstMemberAmount = prevRemainderCents / 100m; // First member gets the remainder
                foreach (var oldMember in previousMembers)
                {
                    // If statement to silence nullability warnings
                    if (oldMember != null)
                    {
                        oldMember.AmountOwed -= prevSplitAmount;
                    }
                }
                previousMembers[0].AmountOwed -= prevFirstMemberAmount; // Adjust first member's amount

            }

            // 2. Apply new split
            if (currentMembers.Count > 0)
            {
                int baseCents = totalTipTaxCents / currentMembers.Count;
                int remainderCents = totalTipTaxCents % currentMembers.Count;

                decimal splitAmount = baseCents / 100m; // Convert back to decimal
                decimal firstMemberAmount = remainderCents / 100m; // First member gets the remainder
                foreach (var newMember in currentMembers)
                {
                    // If statement to silence nullability warnings
                    if (newMember != null)
                    {
                        newMember.AmountOwed += splitAmount;
                    }
                }
                currentMembers[0].AmountOwed += firstMemberAmount; // Adjust first member's amount

            }
        }

        private async Task SyncCheckMembers()
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
                List<CheckMemberWrapper> previousMembers = CheckMembers.ToList(); // Store previous members for tip update
                var vm = new CheckMemberWrapper(checkMember);
                CheckMembers.Add(vm);
                MemberName = "New Check Member"; // Reset the member name for the next input
                foreach (var wrapper in CheckItemWrapper)
                {
                    wrapper.AvailableMembers.Add(vm);
                }
                UpdateMemberTipTax(CheckMembers.ToList(), previousMembers);
                await SyncCheckMembers();
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
            List<CheckMemberWrapper> previousMembers = CheckMembers.ToList(); // Store previous members for tip update
            CheckMembers.Remove(checkMember);
            foreach (var wrapper in CheckItemWrapper)
            {
                wrapper.AvailableMembers.Remove(checkMember);

            }
            UpdateMemberTipTax(CheckMembers.ToList(), previousMembers);
            await SyncCheckMembers();

            int response = await CheckMemberClient.DeleteCheckMember(checkMember.Id);
            if (response == -1)
            {
                await Shell.Current.DisplayAlert("Error", "Failed to delete check member", "OK");
                return;
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
            await Shell.Current.GoToAsync($"{nameof(CheckResultView)}?checkId={CheckId}");

        }


    }
}