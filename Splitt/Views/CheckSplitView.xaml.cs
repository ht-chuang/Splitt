namespace Splitt.Views;

public partial class CheckSplitView : ContentPage
{
	int count = 0;
	public CheckSplitView()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times2";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}
}