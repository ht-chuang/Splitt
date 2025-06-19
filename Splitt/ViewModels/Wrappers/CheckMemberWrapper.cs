using CommunityToolkit.Mvvm.ComponentModel;
using SplittLib.Models;

namespace Splitt.ViewModels

{
    public partial class CheckMemberWrapper : ObservableObject
    {
        private readonly CheckMember _model;

        public CheckMemberWrapper(CheckMember model)
        {
            _model = model;
            name = model.Name;
            amountOwed = model.AmountOwed;
            Id = model.Id;
        }

        public int Id { get; }

        [ObservableProperty]
        private string name;

        [ObservableProperty]
        private decimal amountOwed;
    }
}
