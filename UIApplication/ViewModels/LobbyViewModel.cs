using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FootballLogicLib.Models;
using UIApplication.Connection;
using UIApplication.Views;
using Point = System.Drawing.Point;

namespace UIApplication.ViewModels
{
    [QueryProperty("Players", "Players")]
    public partial class LobbyViewModel : ObservableObject
    {
        [ObservableProperty]
        private List<Player> _players;

        public LobbyViewModel()
        {
            ConnectionManager.OnConnect += ProccessConnectAsync;
            ConnectionManager.OnPlayersList += ProccessPlayersList;
        }

        [RelayCommand]
        private async Task GoToGameAsync(List<Player> players)
        {
            // создаём второго игрока, т.к. локально список состоит из одного
            // TODO: обновить свойство Players, когда приходит второй игрок
            var secondPlayer = new Player
            {
                Id = Guid.NewGuid().ToString(),
                TeamName = "AUS",
                Location = Point.Empty,
                Score = 0
            };

            var game = new Game(players[0], secondPlayer);

            await Shell.Current.GoToAsync(nameof(GamePage), true, new Dictionary<string, object>()
            {
                ["Game"] = game
            });
        }

        private void ProccessPlayersList(Player[] players)
        {
            Shell.Current.Dispatcher.Dispatch(() =>
            {
                foreach (var player in players)
                    _players.Add(player);
            });
        }

        private void ProccessConnectAsync(Player player)
        {
            Shell.Current.Dispatcher.Dispatch(() =>
            {
                _players.Add(player);
            });
        }
    }
}
