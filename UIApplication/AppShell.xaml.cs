using UIApplication.Views;

namespace UIApplication;

public partial class AppShell : Shell
{
	public AppShell()
	{
		InitializeComponent();

        Routing.RegisterRoute(nameof(LobbyPage), typeof(LobbyPage));
        Routing.RegisterRoute(nameof(GamePage), typeof(GamePage));
    }
}
