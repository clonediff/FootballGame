using FootballLogicLib;
using UIApplication.Connection;

namespace UIApplication.Views;

public partial class GamePage : ContentPage
{
    private readonly IDispatcherTimer _timer;
    private TimeSpan _time = TimeSpan.FromMinutes(3);

    public GamePage(Game game)
    {
        InitializeComponent();

        timer.Text = $"{_time.Minutes}:{_time.Seconds}";

        _timer = Dispatcher.CreateTimer();
        _timer.Interval = TimeSpan.FromMilliseconds(1000);
        _timer.Tick += (s, e) =>
        {
            timer.Text = $"{_time.Minutes}:{_time.Seconds}";

            if (_time == TimeSpan.Zero)
                _timer.Stop();
            else
                _time = _time.Add(TimeSpan.FromSeconds(-1));
        };

        _timer.Start();

        //game.TeamAImageSource = $"{game.TeamA.ToLower()}.jpg";
        //game.TeamBImageSource = $"{game.TeamB.ToLower()}.jpg";

        BindingContext = game;

        Task.Run(ConnectionManager.ConnectAndRunAsync);

        ConnectionManager.OnCantConnect += OnCantConnect;
    }

    public void OnCantConnect()
    {
        Dispatcher.Dispatch(async () =>
        {
            await DisplayAlert("Не удалось подключиться", "AAAAAAAAAAAAA", "Пока");
            await Shell.Current.Navigation.PopAsync();
        });
        
    }

    protected override void OnDisappearing()
    {
        ConnectionManager.Disconnect();
        ConnectionManager.OnCantConnect -= OnCantConnect;

        if (_timer.IsRunning)
            _timer.Stop();
    }

    private void OnUpClickedTeamA(object sender, EventArgs e) => imgTeamA.TranslationY -= 10;

    private void OnDownClickedTeamA(object sender, EventArgs e) => imgTeamA.TranslationY += 10;

    private void OnLeftClickedTeamA(object sender, EventArgs e) => imgTeamA.TranslationX -= 10;

    private void OnRightClickedTeamA(object sender, EventArgs e) => imgTeamA.TranslationX += 10;

    private void OnUpClickedTeamB(object sender, EventArgs e) => imgTeamB.TranslationY -= 10;

    private void OnDownClickedTeamB(object sender, EventArgs e) => imgTeamB.TranslationY += 10;

    private void OnLeftClickedTeamB(object sender, EventArgs e) => imgTeamB.TranslationX -= 10;

    private void OnRightClickedTeamB(object sender, EventArgs e) => imgTeamB.TranslationX += 10;
}