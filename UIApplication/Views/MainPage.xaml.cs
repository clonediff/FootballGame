using FootballLogicLib;
using UIApplication.Connection;

namespace UIApplication.Views;

public partial class MainPage : ContentPage
{
	public MainPage()
	{
		InitializeComponent();
	}

    private async void OnStartClicked(object sender, EventArgs e)
    {
		if (YourPck.SelectedItem is null/* || OpponentPck.SelectedItem is null*/)
			ErrorLbl.IsVisible = true;
		else
        {
			await Navigation.PushAsync(new LobbyPage(YourPck.SelectedItem.ToString()));
            //await Navigation.PushAsync(new GamePage(new Game
            //{
            //    TeamA = YourPck.SelectedItem.ToString(),
            //    TeamB = OpponentPck.SelectedItem.ToString()
            //}));
        }
    }
}
