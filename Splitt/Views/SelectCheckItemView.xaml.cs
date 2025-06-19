namespace Splitt.Views;

using System.Text.RegularExpressions;

public partial class SelectCheckItemView : ContentPage
{
    public SelectCheckItemView()
    {
        InitializeComponent();
    }

    private static readonly Regex decimalRegex = new(@"^\d+(\.\d{0,2})?$");

    private void OnUnitPriceChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;

        // Allow empty (in case user is deleting)
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
            return;

        // Reject invalid decimal format
        if (!decimalRegex.IsMatch(e.NewTextValue))
        {
            entry.Text = e.OldTextValue; // revert to previous valid value
        }
    }

    private void OnTipChanged(object sender, TextChangedEventArgs e)
    {
        var entry = (Entry)sender;

        // Allow empty (in case user is deleting)
        if (string.IsNullOrWhiteSpace(e.NewTextValue))
            return;

        // Reject invalid decimal format
        if (!decimalRegex.IsMatch(e.NewTextValue))
        {
            entry.Text = e.OldTextValue; // revert to previous valid value
        }
    }

}