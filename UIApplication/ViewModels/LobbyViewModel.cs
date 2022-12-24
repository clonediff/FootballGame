using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FootballLogicLib;
using Protocol.Packets;
using Protocol.Protocol;
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

        }

        [RelayCommand]
        private async Task GoToGameAsync(List<Player> players)
        {
            await ConnectionManager.SendPacketAsync(PacketType.StartGame,
                new GameStart());
        }
    }
}
