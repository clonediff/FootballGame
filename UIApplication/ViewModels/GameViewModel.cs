using CommunityToolkit.Mvvm.ComponentModel;
using FootballLogicLib;

namespace UIApplication.ViewModels
{
    [QueryProperty("Game", "Game")]
    public partial class GameViewModel : ObservableObject
    {
        [ObservableProperty] 
        private Game _game;

        private IDispatcherTimer _timer;
        private TimeSpan _time = TimeSpan.FromMinutes(3);

        public GameViewModel()
        {

        }

        public void StartTimer()
        {
            Game.Time = $"{_time.Minutes}:{_time.Seconds:00}";
            OnPropertyChanged("Game");

            _timer = Shell.Current.Dispatcher.CreateTimer();
            _timer.Interval = TimeSpan.FromMilliseconds(1000);
            _timer.Tick += (s, e) =>
            {
                Game.Time = $"{_time.Minutes}:{_time.Seconds:00}";
                OnPropertyChanged("Game");

                if (_time == TimeSpan.Zero)
                {
                    _timer.Stop();
                }
                else
                    _time = _time.Add(TimeSpan.FromSeconds(-1));
            };
            _timer.Start();
        }

        public void StopTimer()
        {
            if (_timer.IsRunning)
               _timer.Stop();
        }
    }
}
