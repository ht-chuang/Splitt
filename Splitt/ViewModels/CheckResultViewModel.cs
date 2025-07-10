using System.Collections.ObjectModel;
using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using Splitt.Views;
using Splitt.Services;
using SplittLib.Models;

namespace Splitt.ViewModels
{
    [QueryProperty(nameof(CheckId), "checkId")]
    public partial class CheckResultViewModel : ObservableObject
    {

        private readonly CheckMemberClient CheckMemberClient = new CheckMemberClient();

        [ObservableProperty]
        private int _checkId = 0;

        [ObservableProperty]
        private ObservableCollection<CheckMember> _checkMembers = [];

        partial void OnCheckIdChanged(int value)
        {
            // Called automatically when Shell sets the CheckId
            _ = LoadResultAsync();
        }

        public async Task LoadResultAsync()
        {
            try
            {
                List<CheckMember> checkMemberList = await CheckMemberClient.GetCheckMembersByCheck(CheckId);

                if (checkMemberList != null)
                {
                    CheckMembers.Clear();
                    foreach (var checkMember in checkMemberList)
                    {
                        CheckMembers.Add(checkMember);
                    }
                }
            }
            catch (Exception ex)
            {
                Console.WriteLine($"Error loading check members: {ex.Message}");
            }
        }



        [RelayCommand]
        private async Task Submit()
        {
            await Shell.Current.GoToAsync(nameof(ManageCheckView));
        }
    }

}