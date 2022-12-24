using UIApplication.Connection;
using Dapplo.Windows.Input.Enums;
using Dapplo.Windows.Input.Keyboard;
using System.Reactive.Linq;
using Point = Microsoft.Maui.Graphics.Point;
using UIApplication.ViewModels;
using FootballLogicLib;
using Windows.ApplicationModel.Activation;
using Protocol.Packets;

namespace UIApplication.Views;

public partial class GamePage : ContentPage
{
    //private readonly IDispatcherTimer _timer;
    //private TimeSpan _time = TimeSpan.FromMinutes(3);
    private IDisposable _subscription;

    private const int _translationWithBall = 4;
    private const int _translationWithOutBall = 6;
    private const double _angle = Math.PI / 10;
    private const int _shotDistanceCoeff = 6;

    private event Action<Image> _goalScored;

    private GameViewModel _viewModel;
    private Logic _logic;
    public GamePage(GameViewModel viewModel)
    {
        InitializeComponent();

        _subscription = KeyboardHook.KeyboardEvents
            .Where(h => h.IsKeyDown)
            .Subscribe(KeyDown);

        _logic = new Logic(imgTeamA.WidthRequest, imgBall.WidthRequest);
        _goalScored += OnGoalScored;
        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);

        Window.Width = 1152;
        Window.Height = 592;

        imgTeamA.Source = $"{_viewModel.Game.FirstPlayer.TeamName.ToLower()}.jpg";
        imgTeamB.Source = $"{_viewModel.Game.SecondPlayer.TeamName.ToLower()}.jpg";
        _viewModel.StartTimer();
    }

    private void OnGoalScored(Image goal)
    {
        imgBall.TranslateTo(0, 0, 1000);
        imgTeamA.TranslateTo(0, 0, 1000);
        imgTeamB.TranslateTo(0, 0, 1000);

        if (goal.StyleId == "imgGoalLeft")
        {
            var score = int.Parse(scoreTeamB.Text);
            scoreTeamB.Text = (++score).ToString();
        }
        else
        {
            var score = int.Parse(scoreTeamA.Text);
            scoreTeamA.Text = (++score).ToString();
        }
    }

    protected override void OnDisappearing()
    {
        ConnectionManager.Disconnect();
    }



    private void KeyDown(KeyboardHookEventArgs args)
    {
        Image yourPlayer;
        Image opponentPlayer;

        if (ConnectionManager.Id == _viewModel.Game.FirstPlayer.Id)
        {
            yourPlayer = imgTeamA;
            opponentPlayer = imgTeamB;
        }
        else if (ConnectionManager.Id == _viewModel.Game.SecondPlayer.Id)
        {
            yourPlayer = imgTeamB;
            opponentPlayer = imgTeamA;
        }
        else
            throw new ArgumentException();

        switch (args.Key)
        {
            case VirtualKeyCode.KeyA:
                var translation = _logic.MovePlayer(GetItemCenter(yourPlayer).ToSystemPoint(), GetItemCenter(opponentPlayer).ToSystemPoint(), GetItemCenter(imgBall).ToSystemPoint(),
                    (item1, item2) => item1.X > item2.X,
                    Direction.Left, args.IsShift);
                yourPlayer.TranslationX -= translation.player;
                imgBall.TranslationX -= translation.ball;
                CheckGoal();
                break;
            case VirtualKeyCode.KeyD:
                translation = _logic.MovePlayer(GetItemCenter(yourPlayer).ToSystemPoint(), GetItemCenter(opponentPlayer).ToSystemPoint(), GetItemCenter(imgBall).ToSystemPoint(),
                    (item1, item2) => item1.X < item2.X,
                    Direction.Right, args.IsShift);
                yourPlayer.TranslationX += translation.player;
                imgBall.TranslationX += translation.ball;
                CheckGoal();
                break;
            case VirtualKeyCode.KeyW:
                translation = _logic.MovePlayer(GetItemCenter(yourPlayer).ToSystemPoint(), GetItemCenter(opponentPlayer).ToSystemPoint(), GetItemCenter(imgBall).ToSystemPoint(),
                    (item1, item2) => item1.Y > item2.Y,
                    Direction.Up, args.IsShift);
                yourPlayer.TranslationY -= translation.player;
                imgBall.TranslationY -= translation.ball;
                CheckGoal();
                break;
            case VirtualKeyCode.KeyS:
                translation = _logic.MovePlayer(GetItemCenter(yourPlayer).ToSystemPoint(), GetItemCenter(opponentPlayer).ToSystemPoint(), GetItemCenter(imgBall).ToSystemPoint(),
                    (item1, item2) => item1.Y < item2.Y,
                    Direction.Down, args.IsShift);
                yourPlayer.TranslationY += translation.player;
                imgBall.TranslationY += translation.ball;
                CheckGoal();
                break;
            case VirtualKeyCode.Up:
                var rotation = _logic.RotateBall(-_angle, GetItemCenter(yourPlayer).ToSystemPoint(), GetItemCenter(opponentPlayer).ToSystemPoint(), GetItemCenter(imgBall).ToSystemPoint(),
                    imgBall.X, imgBall.Y, imgBall.TranslationX, imgBall.TranslationY);
                imgBall.TranslationX = rotation.dx;
                imgBall.TranslationY = rotation.dy;
                CheckGoal();
                break;
            case VirtualKeyCode.Down:
                rotation = _logic.RotateBall(_angle, GetItemCenter(yourPlayer).ToSystemPoint(), GetItemCenter(opponentPlayer).ToSystemPoint(), GetItemCenter(imgBall).ToSystemPoint(),
                    imgBall.X, imgBall.Y, imgBall.TranslationX, imgBall.TranslationY);
                imgBall.TranslationX = rotation.dx;
                imgBall.TranslationY = rotation.dy;
                CheckGoal();
                break;
            case VirtualKeyCode.Space:
                Shot(yourPlayer, opponentPlayer, imgBall);
                break;
        }
    }

    private bool CheckGoal()
    {
        if (imgBall.X + imgBall.TranslationX + imgBall.WidthRequest < 235)
        {
            _goalScored(imgGoalLeft);
            return true;
        }
        else if (imgBall.X + imgBall.TranslationX > 905)
        {
            _goalScored(imgGoalRight);
            return true;
        }

        return false;
    }

    private Point GetItemCenter(Image item)
    {
        return new Point(
            item.X + item.TranslationX + item.WidthRequest / 2,
            item.Y + item.TranslationY + item.WidthRequest / 2);
    }
    private bool IsOnBorder(double x, double y, double diameter)
    {
        return (x <= 180 && y > 205 && y < 300 - diameter) || (x <= 235 && (y <= 205 || y >= 300 - diameter)) ||
               (x >= 955 - diameter && y > 205 && y < 300 - diameter) || (x >= 905 - diameter && (y <= 205 || y >= 300 - diameter)) ||
               (y <= 30) || (y <= 206 && (x < 230 || x > 910 - diameter)) ||
               (y >= 475 - diameter) || (y >= 299 - diameter && (x < 230 || x > 910 - diameter));
    }
    private async void Shot(Image player, Image opponent, Image ball)
    {
        var ballCenter = GetItemCenter(ball);
        var playerCenter = GetItemCenter(player);
        var opponentCenter = GetItemCenter(opponent);

        // игрок касается мяча
        if (playerCenter.Distance(ballCenter) <= (player.WidthRequest + ball.WidthRequest) / 2)
        {
            var relativeBallCenter = new Point(ballCenter.X - playerCenter.X, ballCenter.Y - playerCenter.Y);
            var dx = ball.TranslationX;
            var dy = ball.TranslationY;
            var completedDistance = 0.0;
            var isBorderTouched = false;

            // пока мяч не касается соперника
            for (var i = 0.1; i < _shotDistanceCoeff - completedDistance && opponentCenter.Distance(ballCenter) > (opponent.WidthRequest + ball.WidthRequest) / 2; i += 0.1)
            {
                await ball.TranslateTo(relativeBallCenter.X * i + dx, relativeBallCenter.Y * i + dy, 5);
                ballCenter = GetItemCenter(ball);

                if (CheckGoal())
                    return;

                // мяч касается границы поля
                if (IsOnBorder(ball.X + ball.TranslationX, ball.Y + ball.TranslationY, ball.WidthRequest))
                {
                    dx = ball.TranslationX;
                    dy = ball.TranslationY;
                    completedDistance += i;
                    i = 0.1;
                    isBorderTouched = true;

                    if (ball.Y + ball.TranslationY <= 30)
                    {
                        var cos = GetCosine(relativeBallCenter, 35, 0);
                        var sin = GetSine(cos, 1);
                        relativeBallCenter = GetNewBallCenter(relativeBallCenter, _shotDistanceCoeff - i, cos, sin);
                    }
                    if (ball.Y + ball.TranslationY >= 445)
                    {
                        var cos = GetCosine(relativeBallCenter, 35, 0);
                        var sin = GetSine(cos, -1);
                        relativeBallCenter = GetNewBallCenter(relativeBallCenter, _shotDistanceCoeff - i, cos, sin);
                    }
                    if (ball.X + ball.TranslationX <= 235)
                    {
                        var cos = GetCosine(relativeBallCenter, 0, 35);
                        var sin = GetSine(cos, 1);
                        relativeBallCenter = GetNewBallCenter(relativeBallCenter, _shotDistanceCoeff - i, sin, cos);
                    }
                    if (ball.X + ball.TranslationX >= 875)
                    {
                        var cos = GetCosine(relativeBallCenter, 0, 35);
                        var sin = GetSine(cos, -1);
                        relativeBallCenter = GetNewBallCenter(relativeBallCenter, _shotDistanceCoeff - i, sin, cos);
                    }
                }

                // мяч касается игрока после отскока от границы
                if (isBorderTouched && playerCenter.Distance(ballCenter) <= (player.WidthRequest + ball.WidthRequest) / 2)
                    break;
            }
        }

        Point GetNewBallCenter(Point oldBallCenter, double distanceLeft, double dirX, double dirY)
        {
            var normalizeLength = Math.Sqrt(Math.Pow(oldBallCenter.X * distanceLeft, 2) +
                                            Math.Pow(oldBallCenter.Y * distanceLeft, 2)) / distanceLeft;

            return new Point(dirX * normalizeLength, dirY * normalizeLength);
        }

        double GetCosine(Point vector, double x, double y)
        {
            return (vector.X * x + vector.Y * y) / (Math.Sqrt(vector.X * vector.X + vector.Y * vector.Y) * 35);
        }

        double GetSine(double cos, int sign)
        {
            return Math.Sqrt(1 - cos * cos) * sign;
        }
    }
}