using UIApplication.Models;

namespace UIApplication.Views;

public partial class MainPage : ContentPage
{
	int count = 0;

	public MainPage()
	{
		InitializeComponent();
	}

	private void OnCounterClicked(object sender, EventArgs e)
	{
		count++;

		if (count == 1)
			CounterBtn.Text = $"Clicked {count} time";
		else
			CounterBtn.Text = $"Clicked {count} times";

		SemanticScreenReader.Announce(CounterBtn.Text);
	}

    private async void OnStartClicked(object sender, EventArgs e)
    {
		if (YourPck.SelectedItem is null || OpponentPck.SelectedItem is null)
			ErrorLbl.IsVisible = true;
		else
        {
            await Navigation.PushAsync(new GamePage(new Game
            {
                TeamA = YourPck.SelectedItem.ToString(),
                TeamB = OpponentPck.SelectedItem.ToString()
            }));
        }
    }
}

