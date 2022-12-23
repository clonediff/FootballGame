using UIApplication.Connection;
using Dapplo.Windows.Input.Enums;
using Dapplo.Windows.Input.Keyboard;
using System.Reactive.Linq;
using Point = Microsoft.Maui.Graphics.Point;
using UIApplication.ViewModels;
using FootballLogicLib;

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

        //timer.Text = $"{_time.Minutes}:{_time.Seconds:00}";

        //_timer = Dispatcher.CreateTimer();
        //_timer.Interval = TimeSpan.FromMilliseconds(1000);
        //_timer.Tick += (s, e) =>
        //{
        //    timer.Text = $"{_time.Minutes}:{_time.Seconds:00}";

        //    if (_time == TimeSpan.Zero)
        //    {
        //        _timer.Stop();
        //    }
        //    else
        //        _time = _time.Add(TimeSpan.FromSeconds(-1));
        //};
        //_timer.Start();

        _goalScored += OnGoalScored;

        Task.Run(ConnectionManager.ConnectAndRunAsync);

        ConnectionManager.OnCantConnect += OnCantConnect;

        BindingContext = viewModel;
        _viewModel = viewModel;
    }

    protected override void OnNavigatedTo(NavigatedToEventArgs args)
    {
        base.OnNavigatedTo(args);
        imgTeamA.Source = $"{_viewModel.Game.FirstPlayer.TeamName.ToLower()}.jpg";
        imgTeamB.Source = $"{_viewModel.Game.SecondPlayer.TeamName.ToLower()}.jpg";
        _viewModel.SetUp();
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

    public void OnCantConnect()
    {
        Dispatcher.Dispatch(async () =>
        {
            await DisplayAlert("�� ������� ������������", "AAAAAAAAAAAAA", "����");
            await Shell.Current.GoToAsync("..");
        });
        
    }

    protected override void OnDisappearing()
    {
        ConnectionManager.Disconnect();
        ConnectionManager.OnCantConnect -= OnCantConnect;

        //if (_timer.IsRunning)
        //    _timer.Stop();

        _subscription.Dispose();
        _goalScored -= OnGoalScored;
    }

    private void KeyDown(KeyboardHookEventArgs args)
    {
        switch (args.Key)
        {
            case VirtualKeyCode.KeyA:
                //Move(imgTeamA, imgTeamB, imgBall,
                //    (deltaX) => imgTeamA.TranslationX -= deltaX,
                //    (deltaX) => imgBall.TranslationX -= deltaX,
                //    (player, item) => player.X > item.X,
                //    Direction.Left, args.IsShift);
                var translation = _logic.MovePlayer(
                    GetItemCenter(imgTeamA).ToSystemPoint(),
                    GetItemCenter(imgTeamB).ToSystemPoint(),
                    GetItemCenter(imgBall).ToSystemPoint(),
                    (item1, item2) => item1.X > item2.X,
                    Direction.Left, args.IsShift);
                imgTeamA.TranslationX -= translation.player;
                imgBall.TranslationX -= translation.ball;
                break;
            case VirtualKeyCode.KeyD:
                //Move(imgTeamA, imgTeamB, imgBall,
                //    (deltaX) => imgTeamA.TranslationX += deltaX,
                //    (deltaX) => imgBall.TranslationX += deltaX,
                //    (player, item) => player.X < item.X,
                //    Direction.Right, args.IsShift);
                translation = _logic.MovePlayer(
                    GetItemCenter(imgTeamA).ToSystemPoint(),
                    GetItemCenter(imgTeamB).ToSystemPoint(),
                    GetItemCenter(imgBall).ToSystemPoint(),
                    (item1, item2) => item1.X < item2.X,
                    Direction.Right, args.IsShift);
                imgTeamA.TranslationX += translation.player;
                imgBall.TranslationX += translation.ball;
                break;
            case VirtualKeyCode.KeyW:
                //Move(imgTeamA, imgTeamB, imgBall,
                //    (deltaY) => imgTeamA.TranslationY -= deltaY,
                //    (deltaY) => imgBall.TranslationY -= deltaY,
                //    (player, item) => player.Y > item.Y,
                //    Direction.Up, args.IsShift);
                translation = _logic.MovePlayer(
                    GetItemCenter(imgTeamA).ToSystemPoint(),
                    GetItemCenter(imgTeamB).ToSystemPoint(),
                    GetItemCenter(imgBall).ToSystemPoint(),
                    (item1, item2) => item1.Y > item2.Y,
                    Direction.Up, args.IsShift);
                imgTeamA.TranslationY -= translation.player;
                imgBall.TranslationY -= translation.ball;
                break;
            case VirtualKeyCode.KeyS:
                //Move(imgTeamA, imgTeamB, imgBall,
                //    (deltaY) => imgTeamA.TranslationY += deltaY,
                //    (deltaY) => imgBall.TranslationY += deltaY,
                //    (player, item) => player.Y < item.Y,
                //    Direction.Down, args.IsShift);
                translation = _logic.MovePlayer(
                    GetItemCenter(imgTeamA).ToSystemPoint(),
                    GetItemCenter(imgTeamB).ToSystemPoint(),
                    GetItemCenter(imgBall).ToSystemPoint(),
                    (item1, item2) => item1.Y < item2.Y,
                    Direction.Down, args.IsShift);
                imgTeamA.TranslationY += translation.player;
                imgBall.TranslationY += translation.ball;
                break;
            case VirtualKeyCode.Up:
                //HandleRotateCondition(-_angle, imgTeamA, imgTeamB, imgBall);
                var rotation = _logic.RotateBall(-_angle,
                    GetItemCenter(imgTeamA).ToSystemPoint(),
                    GetItemCenter(imgTeamB).ToSystemPoint(),
                    GetItemCenter(imgBall).ToSystemPoint(),
                    imgBall.X, imgBall.Y, imgBall.TranslationX, imgBall.TranslationY);
                imgBall.TranslationX = rotation.dx;
                imgBall.TranslationY = rotation.dy;
                break;
            case VirtualKeyCode.Down:
                //HandleRotateCondition(_angle, imgTeamA, imgTeamB, imgBall);
                rotation = _logic.RotateBall(_angle,
                    GetItemCenter(imgTeamA).ToSystemPoint(),
                    GetItemCenter(imgTeamB).ToSystemPoint(),
                    GetItemCenter(imgBall).ToSystemPoint(),
                    imgBall.X, imgBall.Y, imgBall.TranslationX, imgBall.TranslationY);
                imgBall.TranslationX = rotation.dx;
                imgBall.TranslationY = rotation.dy;
                break;
            case VirtualKeyCode.Space:
                Shot(imgTeamA, imgTeamB, imgBall);
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

    private void Move(
        Image player, Image opponent, Image ball, 
        Action<int> playerTranslator, Action<int> ballTranslator, Func<Point, Point, bool> isItemTranslating,
        Direction direction, bool isBallCaptured)
    {
        var ballCenter = GetItemCenter(ball);
        var playerCenter = GetItemCenter(player);
        var opponentCenter = GetItemCenter(opponent);
        
        // игрок касается мяча
        if (playerCenter.Distance(ballCenter) <= (player.WidthRequest + ball.WidthRequest) / 2)
        {
            // игрок касается соперника
            if (playerCenter.Distance(opponentCenter) <= player.WidthRequest)
            {
                HandlePlayerTouchesOpponent(player, ball, playerTranslator, ballTranslator, isItemTranslating,
                    playerCenter, ballCenter, opponentCenter, direction, isBallCaptured);
            }
            // игрок НЕ касается соперника и соперник НЕ касается мяча
            else if (opponentCenter.Distance(ballCenter) > (opponent.WidthRequest + ball.WidthRequest) / 2)
            {
                // игрок двигается в сторону мяча
                if (isItemTranslating(playerCenter, ballCenter))
                {
                    // мяч и игрок двигаются НЕ в сторону границы
                    HandlePlayerMoveWithBall(player, ball, playerTranslator, ballTranslator,
                        isItemTranslating, playerCenter, ballCenter, direction);
                }
                else
                {
                    // игрок в поле
                    HandlePlayerMoveWithOutBall(player, playerTranslator, ballTranslator, isItemTranslating, playerCenter, direction, isBallCaptured);
                }
            }
            // игрок НЕ касается соперника и соперник касается мяча
            else
            {
                // игрок двигается в сторону мяча
                if (isItemTranslating(playerCenter, ballCenter))
                {
                    // игрок двигается НЕ в сторону соперника
                    if (isItemTranslating(opponentCenter, ballCenter))
                    {
                        // мяч и игрок двигаются НЕ в сторону границы
                        HandlePlayerMoveWithBall(player, ball, playerTranslator, ballTranslator,
                            isItemTranslating, playerCenter, ballCenter, direction);
                    }
                }
                else
                {
                    // игрок в поле
                    HandlePlayerMoveWithOutBall(player, playerTranslator, ballTranslator, isItemTranslating, playerCenter, direction, isBallCaptured);
                }
            }
        }
        // игрок НЕ касается мяча и касается соперника
        else if (playerCenter.Distance(opponentCenter) <= player.WidthRequest)
        {
            // игрок двигается НЕ в сторону соперника
            if (!isItemTranslating(playerCenter, opponentCenter))
            {
                // игрок в поле
                HandlePlayerMoveWithOutBall(player, playerTranslator, ballTranslator, isItemTranslating, playerCenter, direction, false);
            }
        }
        // просто ходим
        else
        {
            // игрок в поле
            HandlePlayerMoveWithOutBall(player, playerTranslator, ballTranslator, isItemTranslating, playerCenter, direction, false);
        }

        CheckGoal();
    }

    private void HandlePlayerMoveWithBall(
        Image player, Image ball,
        Action<int> playerTranslator, Action<int> ballTranslator, Func<Point, Point, bool> isItemTranslating,
        Point playerCenter, Point ballCenter, Direction direction)
    {
        // игрок и мяч НЕ покидает пределов поля
        if (!(IsOnBorder(ball.X + ball.TranslationX, ball.Y + ball.TranslationY, ball.WidthRequest) && 
              isItemTranslating(ballCenter, GetBorderPoint(ball.X + ball.TranslationX, ball.Y + ball.TranslationY, ball.WidthRequest / 2, direction))) &&
            !(IsOnBorder(player.X + player.TranslationX, player.Y + player.TranslationY, player.WidthRequest) &&
              isItemTranslating(playerCenter, GetBorderPoint(player.X + player.TranslationX, player.Y + player.TranslationY, player.WidthRequest / 2, direction))))
        {
            playerTranslator(_translationWithBall);
            ballTranslator(_translationWithBall);
        }
    }

    private void HandlePlayerMoveWithOutBall(
        Image player, Action<int> playerTranslator, Action<int> ballTranslator, Func<Point, Point, bool> isItemTranslating,
        Point playerCenter, Direction direction, bool isBallCaptured)
    {
        // игрок НЕ покидает пределов поля
        if (!(IsOnBorder(player.X + player.TranslationX, player.Y + player.TranslationY, player.WidthRequest) &&
              isItemTranslating(playerCenter, GetBorderPoint(player.X + player.TranslationX, player.Y + player.TranslationY, player.WidthRequest / 2, direction))))
        {
            if (isBallCaptured)
            {
                playerTranslator(_translationWithBall);
                ballTranslator(_translationWithBall);
            }
            else
                playerTranslator(_translationWithOutBall);
        }
    }

    private void HandlePlayerTouchesOpponent(
        Image player, Image ball,
        Action<int> playerTranslator, Action<int> ballTranslator, Func<Point, Point, bool> isItemTranslating,
        Point playerCenter, Point ballCenter, Point opponentCenter, Direction direction, bool isBallCaptured)
    {
        // игрок двигается в сторону мяча
        if (isItemTranslating(playerCenter, ballCenter))
        {
            // игрок двигается НЕ в сторону соперника
            if (!isItemTranslating(playerCenter, opponentCenter))
            {
                // мяч и игрок двигаются НЕ в сторону границы
                HandlePlayerMoveWithBall(player, ball, playerTranslator, ballTranslator,
                    isItemTranslating, playerCenter, ballCenter, direction);
            }
        }
        // игрок двигается НЕ в сторону мяча и НЕ в сторону соперника
        else if (!isItemTranslating(playerCenter, opponentCenter))
        {
            // игрок в поле
            HandlePlayerMoveWithOutBall(player, playerTranslator, ballTranslator, isItemTranslating, playerCenter, direction, isBallCaptured);
        }
    }

    private bool IsOnBorder(double x, double y, double diameter)
    {
        return (x <= 180 && y > 205 && y < 300 - diameter) || (x <= 235 && (y <= 205 || y >= 300 - diameter)) || 
               (x >= 955 - diameter && y > 205 && y < 300 - diameter) || (x >= 905 - diameter && (y <= 205 || y >= 300 - diameter)) ||
               (y <= 30) || (y <= 206 && (x < 230 || x > 910 - diameter)) ||
               (y >= 475 - diameter) || (y >= 299 - diameter && (x < 230 || x > 910 - diameter));
    }

    private Point GetBorderPoint(double x, double y, double radius, Direction direction)
    {
        var candidates = GetBorderPointCandidates(x, y, radius);

        if (candidates.Count == 1)
            return candidates[0];

        switch (direction)
        {
            case Direction.Up:
                var point = candidates.FirstOrDefault(p => p.Y == 30 || p.Y == 206);
                return point == Point.Zero ? candidates[0] : point;
            case Direction.Down:
                point = candidates.FirstOrDefault(p => p.Y == 475 || p.Y == 299);
                return point == Point.Zero ? candidates[0] : point;
            case Direction.Right:
                point = candidates.FirstOrDefault(p => p.X == 905 || p.X == 955);
                return point == Point.Zero ? candidates[0] : point;
            case Direction.Left:
                point = candidates.FirstOrDefault(p => p.X == 235 || p.X == 180);
                return point == Point.Zero ? candidates[0] : point;
        }

        return Point.Zero;
    }

    private List<Point> GetBorderPointCandidates(double x, double y, double radius)
    {
        var candidates = new List<Point>();

        // вверхняя граница вне ворот
        if (y <= 30)
            candidates.Add(new Point(x + radius, 30));

        // левая граница вне ворот
        if (x <= 235 && (y <= 205 || y >= 300 - radius * 2))
            candidates.Add(new Point(235, y + radius));

        // верхняя граница в воротах
        if (y <= 206 && (x < 230 || x > 910 - radius * 2))
            candidates.Add(new Point(x + radius, 206));

        // левая граница в воротах
        if (x <= 180 && y > 205 && y < 300 - radius * 2)
            candidates.Add(new Point(180, y + radius));

        // нижняя граница в воротах
        if (y >= 299 - radius * 2 && (x < 230 || x > 910 - radius * 2))
            candidates.Add(new Point(x + radius, 299));
        
        // нижняя граница вне ворот
        if (y >= 475 - radius * 2)
            candidates.Add(new Point(x + radius, 475));

        // правая граница вне ворот
        if (x >= 905 - radius * 2 && (y <= 205 || y >= 300 - radius * 2))
            candidates.Add(new Point(905, y + radius));

        // правая граница в воротах
        if (x >= 955 - radius * 2 && y > 205 && y < 300 - radius * 2)
            candidates.Add(new Point(955, y + radius));

        return candidates;
    }

    private void HandleRotateCondition(double angle, Image player, Image opponent, Image ball)
    {
        var ballCenter = GetItemCenter(ball);
        var playerCenter = GetItemCenter(player);
        var opponentCenter = GetItemCenter(opponent);

        // игрок касается мяча
        if (playerCenter.Distance(ballCenter) <= (player.WidthRequest + ball.WidthRequest) / 2)
        {
            TryRotate(ballCenter, playerCenter, opponentCenter, angle,
                    opponentCenter.Distance(ballCenter) <= (opponent.WidthRequest + ball.WidthRequest) / 2);
        }

        CheckGoal();
    }

    private void TryRotate(Point ballCenter, Point playerCenter, Point opponentCenter, double angle, bool checkForOpponent)
    {
        var dx = (ballCenter.X - playerCenter.X) * Math.Cos(angle) -
                 (ballCenter.Y - playerCenter.Y) * Math.Sin(angle) + playerCenter.X;
        var dy = (ballCenter.X - playerCenter.X) * Math.Sin(angle) +
                 (ballCenter.Y - playerCenter.Y) * Math.Cos(angle) + playerCenter.Y;

        var newBallCenter = new Point(dx, dy);

        // мяч на краю поля
        if (IsOnBorder(ballCenter.X - imgBall.WidthRequest / 2, ballCenter.Y - imgBall.WidthRequest / 2, imgBall.WidthRequest))
        {
            if (checkForOpponent)
            {
                // мяч ушёл от края поля и соперник НЕ касается мяча
                if (newBallCenter.Distance(opponentCenter) > ballCenter.Distance(opponentCenter) &&
                    !IsOnBorder(newBallCenter.X - imgBall.WidthRequest / 2, newBallCenter.Y - imgBall.WidthRequest / 2, imgBall.WidthRequest))
                {
                    UpdateTranslation();
                }
            }
            else
            {
                // мяч ушёл от края поля
                if (!IsOnBorder(newBallCenter.X - imgBall.WidthRequest / 2, newBallCenter.Y - imgBall.WidthRequest / 2, imgBall.WidthRequest))
                {
                    UpdateTranslation();
                }
            }
        }
        else
        {
            if (checkForOpponent)
            {
                // соперник НЕ касается мяча
                if (newBallCenter.Distance(opponentCenter) > ballCenter.Distance(opponentCenter))
                {
                    UpdateTranslation();
                }
            }
            else
            {
                UpdateTranslation();
            }
        }

        void UpdateTranslation()
        {
            imgBall.TranslationX = dx - imgBall.X - imgBall.WidthRequest / 2;
            imgBall.TranslationY = dy - imgBall.Y - imgBall.HeightRequest / 2;
        }
    }

    private Point GetItemCenter(Image item)
    {
        return new Point(
            item.X + item.TranslationX + item.WidthRequest / 2,
            item.Y + item.TranslationY + item.WidthRequest / 2);
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