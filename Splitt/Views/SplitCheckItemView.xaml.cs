namespace Splitt.Views;

using Splitt.ViewModels.Wrappers;
using Splitt.ViewModels;

public partial class SplitCheckItemView : ContentPage
{

    public SplitCheckItemView()
    {
        InitializeComponent();
    }

    private void OnMemberSelectionChanged(object sender, SelectionChangedEventArgs e)
    {
        if (sender is not CollectionView cv)
            return;

        // The BindingContext of the CollectionView is the current CheckItemWrapper
        if (cv.BindingContext is not CheckItemWrapper wrapper)
            return;

        var item = wrapper.Item;
        var newlySelected = e.CurrentSelection
            .Select(x => x as CheckMemberWrapper)
            .Where(x => x != null)
            .ToList();
        var previouslySelected = e.PreviousSelection
            .Select(x => x as CheckMemberWrapper)
            .Where(x => x != null)
            .ToList();

        int totalCents = (int)(item.TotalPrice * 100); // Convert to cents for precision

        // 1. Undo previous split
        if (previouslySelected.Count > 0)
        {
            // decimal previousSplit = item.TotalPrice / previouslySelected.Count;
            int prevBaseCents = totalCents / previouslySelected.Count;
            int prevRemainderCents = totalCents % previouslySelected.Count;

            decimal prevSplitAmount = prevBaseCents / 100m; // Convert back to decimal
            decimal prevFirstMemberAmount = prevRemainderCents / 100m; // First member gets the remainder
            foreach (var oldMember in previouslySelected)
            {
                // If statement to silence nullability warnings
                if (oldMember != null)
                {
                    oldMember.AmountOwed -= prevSplitAmount;
                }
            }
            previouslySelected[0].AmountOwed -= prevFirstMemberAmount; // Adjust first member's amount

        }

        // 2. Apply new split
        if (newlySelected.Count > 0)
        {
            int baseCents = totalCents / newlySelected.Count;
            int remainderCents = totalCents % newlySelected.Count;

            decimal splitAmount = baseCents / 100m; // Convert back to decimal
            decimal firstMemberAmount = remainderCents / 100m; // First member gets the remainder
            foreach (var newMember in newlySelected)
            {
                // If statement to silence nullability warnings
                if (newMember != null)
                {
                    newMember.AmountOwed += splitAmount;
                }
            }
            newlySelected[0].AmountOwed += firstMemberAmount; // Adjust first member's amount

        }
    }

}