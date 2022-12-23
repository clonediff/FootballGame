using UIApplication.Connection;
using UIApplication.ViewModels;

namespace UIApplication.Views;

public partial class LobbyPage : ContentPage
{
	public LobbyPage(LobbyViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        ConnectionManager.Team = (BindingContext as LobbyViewModel).Players[0].TeamName;
        Task.Run(ConnectionManager.ConnectAndRunAsync);
    }
}