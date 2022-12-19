using FootballLogicLib;
using UIApplication.Connection;

namespace UIApplication.Views;

public partial class LobbyPage : ContentPage
{
	public LobbyPage(string team)
	{
		InitializeComponent();

		ConnectionManager.Team = team;
		Task.Run(ConnectionManager.ConnectAndRunAsync);

		ConnectionManager.OnConnect += ProccessConnectAsync;
		ConnectionManager.OnPlayersList += ProccessPlayersList;
	}

    private void ProccessPlayersList(Player[] players)
    {
        Shell.Current.Dispatcher.Dispatch(() =>
        {
			foreach (var player in players)
				Players.Add(GetPlayerLabel(player));
        });
    }

    private void ProccessConnectAsync(Player player)
    {
		Shell.Current.Dispatcher.Dispatch(() =>
		{
			Players.Add(GetPlayerLabel(player));
		});
    }

	private Label GetPlayerLabel(Player player)
	{
        var label = new Label();
        label.Text = $"{player.Id}: {player.TeamName}";
        return label;
    }
}