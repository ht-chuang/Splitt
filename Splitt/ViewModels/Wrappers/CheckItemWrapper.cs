using SplittLib.Models;
using System.Collections.ObjectModel;

namespace Splitt.ViewModels.Wrappers

{

    public class CheckItemWrapper
    {
        public CheckItem Item { get; set; } = null!;
        public ObservableCollection<CheckMemberWrapper> AvailableMembers { get; set; } = [];
    }
}