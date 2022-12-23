using FootballLogicLib;
using Protocol.Packets;
using Protocol.Protocol;
using UIApplication.Connection;

namespace UIApplication.Views;

public partial class LobbyPage : ContentPage
{
	private bool _gameStarted = false;
    private bool _isReady = false;

	private Dictionary<string, Label> labels = new();

	public LobbyPage(string team)
	{
		InitializeComponent();

		Task.Run(() => ConnectionManager.ConnectAndRunAsync(team));

		ConnectionManager.OnConnect += ProccessConnectAsync;
		ConnectionManager.OnPlayersList += ProccessPlayersList;
		ConnectionManager.OnCantConnect += ProccesCantConnect;

        ConnectionManager.OnPlayerDisconnect += ProccessPlayerDisconnect;
        ConnectionManager.OnReadyStateChanged += ProccessStateChanged;
	}

    private void ProccessStateChanged(string id, bool ready)
    {
        if (labels.TryGetValue(id, out Label label))
        {
            Dispatcher.Dispatch(() =>
            {
                if (ready)
                    label.BackgroundColor = Color.Parse("Green");
                else
                    label.BackgroundColor = Color.Parse("Red");
            });
        }
        else
            throw new ArgumentException("Unknown id");
    }

    private async void ChangeReadyState(object sender, EventArgs args)
    {
        _isReady = !_isReady;

        if (!_isReady)
        {
            ReadyBtn.Text = "Ready";
            ReadyBtn.BackgroundColor = Color.Parse("green");
        }
        else
        {
            ReadyBtn.Text = "Not Ready";
            ReadyBtn.BackgroundColor = Color.Parse("RED");
        }

        var playerReadyState = new PlayerReadyState { IsReady = _isReady };
        await ConnectionManager.SendPacketAsync(PacketType.ReadyState, playerReadyState);
    }

    private void ProccessPlayerDisconnect(string id)
    {
        RemoveLabel(id);
    }

    private void ProccesCantConnect(string id)
    {
        Dispatcher.Dispatch(async () =>
        {
            await DisplayAlert("Can't connect", "Can't connect, game is full", "Cancel");
            await Shell.Current.Navigation.PopAsync();
        });
    }

	private void RemoveLabel(string id)
	{
        if (labels.TryGetValue(id, out var label))
        {
            Dispatcher.Dispatch(() =>
            {
                Players.Remove(label);
            });
        }
    }

    private void ProccessPlayersList(Player[] players)
    {
        var result = Dispatcher.Dispatch(() =>
        {
			foreach (var player in players)
			{
				var label = GetPlayerLabel(player);
                Players.Add(label);
				labels[player.Id] = label;
			}
        });
    }

    private void ProccessConnectAsync(Player player)
    {
		var result = Dispatcher.Dispatch(() =>
		{
			var label = GetPlayerLabel(player);
			labels[player.Id] = label;
            Players.Add(label);
		});
    }

	private Label GetPlayerLabel(Player player)
	{
        var label = new Label();
        label.Text = $"{player.Id}: {player.TeamName}";
        return label;
    }

    protected override void OnDisappearing()
    {
		if (!_gameStarted)
		{
			ConnectionManager.Disconnect();
        }
        ConnectionManager.OnConnect -= ProccessConnectAsync;
        ConnectionManager.OnPlayersList -= ProccessPlayersList;
        ConnectionManager.OnCantConnect -= ProccesCantConnect;
        ConnectionManager.OnPlayerDisconnect -= ProccessPlayerDisconnect;
        ConnectionManager.OnReadyStateChanged -= ProccessStateChanged;
    }
}