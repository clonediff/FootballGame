using FootballLogicLib;
using Protocol.Packets;
using Protocol.Protocol;
using UIApplication.Connection;
using UIApplication.ViewModels;

namespace UIApplication.Views;

public partial class LobbyPage : ContentPage
{
	private bool _gameStarted = false;
    private bool _isReady = false;

	private Dictionary<string, Label> labels = new();

	public LobbyPage(LobbyViewModel viewModel)
	{
		InitializeComponent();
        BindingContext = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        Task.Run(() => ConnectionManager.ConnectAndRunAsync((BindingContext as LobbyViewModel).Players[0].TeamName));

        ConnectionManager.OnConnect += ProccessConnectAsync;
        ConnectionManager.OnPlayersList += ProccessPlayersList;
        ConnectionManager.OnCantConnect += ProccesCantConnect;

        ConnectionManager.OnPlayerDisconnect += ProccessPlayerDisconnect;
        ConnectionManager.OnReadyStateChanged += ProccessStateChanged;
        ConnectionManager.OnGameReady += ProccessGameState;

        ConnectionManager.OnGameStart += ProccessGameStartAsync;
    }

    private void ProccessGameStartAsync(Player player1, Player player2)
    {
        _gameStarted = true;
        var game = new Game(player1, player2);
        Dispatcher.Dispatch(async() =>
        {
            Shell.Current.Navigation.RemovePage(this);
            await Shell.Current.GoToAsync(nameof(GamePage), true, new Dictionary<string, object>()
            {
                ["Game"] = game
            });
        });
    }

    private void ProccessGameState(bool gameReady)
    {
        Dispatcher.Dispatch(() =>
        {
            StartGameBtn.IsVisible = gameReady;
            StartGameBtn.IsEnabled = gameReady;
        });
    }

    private void ProccessStateChanged(string id, bool ready)
    {
        if (labels.TryGetValue(id, out Label label))
        {
            Dispatcher.Dispatch(() =>
            {
                var color = ready ? "Green" : "Red";
                label.BackgroundColor = Color.Parse(color);
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

    private void ProccessPlayersList(PlayerIsReadyStruct[] players)
    {
        var result = Dispatcher.Dispatch(() =>
        {
			foreach (var playerInfo in players)
			{
				var label = GetPlayerLabel(playerInfo.Player, playerInfo.IsReady);
                Players.Add(label);
				labels[playerInfo.Player.Id] = label;
			}
        });
    }

    private void ProccessConnectAsync(Player player)
    {
		var result = Dispatcher.Dispatch(() =>
		{
			var label = GetPlayerLabel(player, false);
			labels[player.Id] = label;
            Players.Add(label);
		});
    }

	private Label GetPlayerLabel(Player player, bool ready)
	{
        var label = new Label();
        label.Text = $"{player.Id}: {player.TeamName}";
        var color = ready ? "GREEN" : "RED";
        label.BackgroundColor = Color.Parse(color);
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
        ConnectionManager.OnGameReady -= ProccessGameState;
    }
}