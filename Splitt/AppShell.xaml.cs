using Splitt.Views;

namespace Splitt;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(NewCheckView), typeof(NewCheckView));
		Routing.RegisterRoute(nameof(SelectCheckItemView), typeof(SelectCheckItemView));
		Routing.RegisterRoute(nameof(SplitCheckItemView), typeof(SplitCheckItemView));
	}
}
