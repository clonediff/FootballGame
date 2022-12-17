using UIApplication.Models;
using Point = Microsoft.Maui.Graphics.Point;

namespace UIApplication.Views;

public partial class GamePage : ContentPage
{
    private readonly IDispatcherTimer _timer;
    private TimeSpan _time = TimeSpan.FromMinutes(3);

    private const int _collisionEps = 3;
    private const int _translationWithBall = 4;
    private const int _translationWithOutBall = 6;
    private const double _angle = Math.PI / 10;
    
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

        game.TeamAImageSource = $"{game.TeamA.ToLower()}.jpg";
        game.TeamBImageSource = $"{game.TeamB.ToLower()}.jpg";

        BindingContext = game;
    }

    protected override void OnDisappearing()
    {
        if (_timer.IsRunning)
            _timer.Stop();
    }

    private void OnUpClickedTeam(object sender, EventArgs e)
    {
        if (((ImageButton)sender).StyleId.EndsWith("A"))
        {
            Go(imgTeamA, imgTeamB, imgBall, 
                (deltaY) => imgTeamA.TranslationY -= deltaY, 
                (deltaY) => imgBall.TranslationY -= deltaY,
                (player, item) => player.Y > item.Y);
        }
        else
        {
            Go(imgTeamB, imgTeamA, imgBall, 
                (deltaY) => imgTeamB.TranslationY -= deltaY, 
                (deltaY) => imgBall.TranslationY -= deltaY,
                (player, item) => player.Y > item.Y);
        }
    }

    private void OnDownClickedTeam(object sender, EventArgs e)
    {
        if (((ImageButton)sender).StyleId.EndsWith("A"))
        {
            Go(imgTeamA, imgTeamB, imgBall, 
                (deltaY) => imgTeamA.TranslationY += deltaY, 
                (deltaY) => imgBall.TranslationY += deltaY,
                (player, item) => player.Y < item.Y);
        }
        else
        {
            Go(imgTeamB, imgTeamA, imgBall, 
                (deltaY) => imgTeamB.TranslationY += deltaY, 
                (deltaY) => imgBall.TranslationY += deltaY,
                (player, item) => player.Y < item.Y);
        }
    }

    private void OnLeftClickedTeam(object sender, EventArgs e)
    {
        if (((ImageButton)sender).StyleId.EndsWith("A"))
        {
            Go(imgTeamA, imgTeamB, imgBall, 
                (deltaX) => imgTeamA.TranslationX -= deltaX, 
                (deltaX) => imgBall.TranslationX -= deltaX,
                (player, item) => player.X > item.X);
        }
        else
        {
            Go(imgTeamB, imgTeamA, imgBall, 
                (deltaX) => imgTeamB.TranslationX -= deltaX, 
                (deltaX) => imgBall.TranslationX -= deltaX,
                (player, ball) => player.X > ball.X);
        }
    }

    private void OnRightClickedTeam(object sender, EventArgs e)
    {
        if (((ImageButton)sender).StyleId.EndsWith("A"))
        {
            Go(imgTeamA, imgTeamB, imgBall, 
                (deltaX) => imgTeamA.TranslationX += deltaX, 
                (deltaX) => imgBall.TranslationX += deltaX,
                (player, item) => player.X < item.X);
        }
        else
        {
            Go(imgTeamB, imgTeamA, imgBall, 
                (deltaX) => imgTeamB.TranslationX += deltaX, 
                (deltaX) => imgBall.TranslationX += deltaX,
                (player, item) => player.X < item.X);
        }
    }

    private void Go(
        Image player, Image opponent, Image ball, 
        Action<int> playerTranslator, Action<int> ballTranslator, Func<Point, Point, bool> isItemTranslating)
    {
        var ballCenter = GetItemCenter(ball);
        var playerCenter = GetItemCenter(player);
        var opponentCenter = GetItemCenter(opponent);

        // игрок касается мяча
        //if (Math.Abs((player.WidthRequest + ball.WidthRequest) / 2 - playerCenter.Distance(ballCenter)) <= _collisionEps)
        if (playerCenter.Distance(ballCenter) <= (player.WidthRequest + ball.WidthRequest) / 2)
        {
            // игрок касается соперника
            //if (Math.Abs(player.WidthRequest - playerCenter.Distance(opponentCenter)) <= _collisionEps)
            if (playerCenter.Distance(opponentCenter) <= player.WidthRequest)
            {
                // соперник НЕ касается мяча
                //if (Math.Abs((ball.WidthRequest + opponent.WidthRequest) / 2 - opponentCenter.Distance(ballCenter)) > _collisionEps)
                if (opponentCenter.Distance(ballCenter) > (opponent.WidthRequest + ball.WidthRequest) / 2)
                {
                    // игрок двигается в сторону мяча
                    if (isItemTranslating(playerCenter, ballCenter))
                    {
                        // игрок двигается НЕ в сторону соперника
                        if (!isItemTranslating(playerCenter, opponentCenter))
                        {
                            playerTranslator(_translationWithBall);
                            ballTranslator(_translationWithBall);
                        }
                    }
                    // игрок двигается НЕ в сторону мяча и НЕ в сторону соперника
                    else if (!isItemTranslating(playerCenter, opponentCenter))
                    {
                        playerTranslator(_translationWithOutBall);
                    }
                }
                // соперник касается мяча
                else
                {
                    // игрок двигается в сторону мяча
                    if (isItemTranslating(playerCenter, ballCenter))
                    {
                        // игрок двигается НЕ в сторону соперника
                        if (!isItemTranslating(playerCenter, opponentCenter))
                        {
                            playerTranslator(_translationWithBall);
                            ballTranslator(_translationWithBall);
                        }
                    }
                    // игрок двигается НЕ в сторону мяча и НЕ в сторону соперника
                    else if (!isItemTranslating(playerCenter, opponentCenter))
                    {
                        playerTranslator(_translationWithOutBall);
                    }
                }
            }
            // игрок НЕ касается соперника и соперник НЕ касается мяча
            //else if (Math.Abs((ball.WidthRequest + opponent.WidthRequest) / 2 - opponentCenter.Distance(ballCenter)) > _collisionEps)
            else if (opponentCenter.Distance(ballCenter) > (opponent.WidthRequest + ball.WidthRequest) / 2)
            {
                // игрок двигается в сторону мяча
                if (isItemTranslating(playerCenter, ballCenter))
                {
                    playerTranslator(_translationWithBall);
                    ballTranslator(_translationWithBall);
                }
                else
                {
                    playerTranslator(_translationWithOutBall);
                }
            }
            // игрок НЕ касается соперника и соперник касается мяча
            else
            {
                // игрок двигается в сторону мяча
                if (isItemTranslating(playerCenter, ballCenter))
                {
                    // игрок двигается НЕ в сторону соперника
                    if (!isItemTranslating(playerCenter, opponentCenter))
                    {
                        playerTranslator(_translationWithBall);
                        ballTranslator(_translationWithBall);
                    }
                }
                // игрок двигается НЕ в сторону мяча и НЕ в сторону соперника
                else if (!isItemTranslating(playerCenter, opponentCenter))
                {
                    playerTranslator(_translationWithOutBall);
                }
            }
        }
        // игрок НЕ касается мяча и касается соперника
        //else if (Math.Abs(player.WidthRequest - playerCenter.Distance(opponentCenter)) <= _collisionEps)
        else if (playerCenter.Distance(opponentCenter) <= player.WidthRequest)
        {
            // игрок НЕ двигается в сторону соперника
            if (!isItemTranslating(playerCenter, opponentCenter))
            {
                playerTranslator(_translationWithOutBall);
            }
        }
        // просто ходим
        else
        {
            playerTranslator(_translationWithOutBall);
        }
    }

    private void OnRightRotate(object sender, EventArgs e)
    {
        if (((ImageButton)sender).StyleId.EndsWith("A"))
            HandleRotateCondition(_angle, imgTeamA, imgTeamB, imgBall);
        else
            HandleRotateCondition(_angle, imgTeamB, imgTeamA, imgBall);
    }

    private void OnLeftRotate(object sender, EventArgs e)
    {
        if (((ImageButton)sender).StyleId.EndsWith("A"))
            HandleRotateCondition(-_angle, imgTeamA, imgTeamB, imgBall);
        else
            HandleRotateCondition(-_angle, imgTeamB, imgTeamA, imgBall);
    }

    private void HandleRotateCondition(double angle, Image player, Image opponent, Image ball)
    {
        var ballCenter = GetItemCenter(ball);
        var playerCenter = GetItemCenter(player);
        var opponentCenter = GetItemCenter(opponent);

        // игрок касается мяча
        if (playerCenter.Distance(ballCenter) <= (player.WidthRequest + ball.WidthRequest) / 2)
        {
            // соперник НЕ касается мяча
            if (opponentCenter.Distance(ballCenter) > (opponent.WidthRequest + ball.WidthRequest) / 2)
            {
                Rotate(angle, ballCenter, playerCenter);
            }
            // соперник касается мяча
            else
            {
                var dx = (ballCenter.X - playerCenter.X) * Math.Cos(angle) -
                         (ballCenter.Y - playerCenter.Y) * Math.Sin(angle) + playerCenter.X;
                var dy = (ballCenter.X - playerCenter.X) * Math.Sin(angle) +
                         (ballCenter.Y - playerCenter.Y) * Math.Cos(angle) + playerCenter.Y;

                var newBallCenter = ballCenter with { X = dx, Y = dy };

                // мяч двигается НЕ в сторону соперника
                if (newBallCenter.Distance(opponentCenter) > ballCenter.Distance(opponentCenter))
                {
                    imgBall.TranslationX = dx - imgBall.X - imgBall.WidthRequest / 2;
                    imgBall.TranslationY = dy - imgBall.Y - imgBall.HeightRequest / 2;
                }
            }
        }
    }

    private void Rotate(double angle, Point ballCenter, Point playerCenter)
    {
        var dx = (ballCenter.X - playerCenter.X) * Math.Cos(angle) -
                 (ballCenter.Y - playerCenter.Y) * Math.Sin(angle) + playerCenter.X;
        var dy = (ballCenter.X - playerCenter.X) * Math.Sin(angle) +
                 (ballCenter.Y - playerCenter.Y) * Math.Cos(angle) + playerCenter.Y;

        imgBall.TranslationX = dx - imgBall.X - imgBall.WidthRequest / 2;
        imgBall.TranslationY = dy - imgBall.Y - imgBall.HeightRequest / 2;
    }

    private Point GetItemCenter(Image item)
    {
        return new Point(
            item.X + item.TranslationX + item.WidthRequest / 2,
            item.Y + item.TranslationY + item.WidthRequest / 2);
    }
}