using Splitt.Views;

namespace Splitt;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

		Routing.RegisterRoute(nameof(NewCheckView), typeof(NewCheckView));
		Routing.RegisterRoute(nameof(NewCheckItemView), typeof(NewCheckItemView));
		Routing.RegisterRoute(nameof(SelectCheckItemView), typeof(SelectCheckItemView));
	}
}
