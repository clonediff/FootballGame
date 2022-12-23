using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FootballLogicLib.Models;
using UIApplication.Views;
using Point = System.Drawing.Point;

namespace UIApplication.ViewModels
{
    public partial class MainViewModel : ObservableObject
    {
        [ObservableProperty]
        private bool _isPickerEmpty; 

        [RelayCommand]
        private async Task GoToLobbyAsync(object team)
        {
            if (team is null)
            {
                IsPickerEmpty = true;
                return;
            }    

            var player = new Player
            {
                Id = Guid.NewGuid().ToString(),
                TeamName = team.ToString(),
                Location = Point.Empty,
                Score = 0,
            };

            await Shell.Current.GoToAsync(nameof(LobbyPage), true, new Dictionary<string, object>()
            {
                ["Players"] = new List<Player>
                {
                    player
                }
            });
        }
    }
}
