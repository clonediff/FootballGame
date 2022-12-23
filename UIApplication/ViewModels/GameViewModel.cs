using CommunityToolkit.Mvvm.ComponentModel;
using CommunityToolkit.Mvvm.Input;
using FootballLogicLib.Models;

namespace UIApplication.ViewModels
{
    [QueryProperty("Game", "Game")]
    public partial class GameViewModel : ObservableObject
    {
        [ObservableProperty]
        private Game _game;

        public GameViewModel()
        {

        }
    }
}
