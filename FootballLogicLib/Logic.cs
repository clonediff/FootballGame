using System.ComponentModel;
using System.Drawing;
using System.Numerics;

namespace FootballLogicLib
{
    public class Logic
    {
        private const int _translationWithBall = 4;
        private const int _translationWithOutBall = 6;

        public double PlayerWidth { get; }
        public double BallWidth { get; }

        public Logic(double playerWidth, double ballWidth)
        {
            PlayerWidth = playerWidth;
            BallWidth = ballWidth;
        }

        #region MOVING
        public (int player, int ball) MovePlayer(
            PointF player, PointF opponent, PointF ball, Func<PointF, PointF, bool> isItemTranslating,
            Direction direction, bool isBallCaptured)
        {
            // игрок касается мяча
            if (player.Distance(ball) <= (PlayerWidth + BallWidth) / 2)
            {
                // игрок касается соперника
                if (player.Distance(opponent) <= PlayerWidth)
                {
                    return HandlePlayerTouchesOpponent(player, ball, opponent, isItemTranslating, direction, isBallCaptured);
                }
                // игрок НЕ касается соперника и соперник НЕ касается мяча
                else if (opponent.Distance(ball) > (PlayerWidth + BallWidth) / 2)
                {
                    // игрок двигается в сторону мяча
                    if (isItemTranslating(player, ball))
                    {
                        // мяч и игрок двигаются НЕ в сторону границы
                        return HandlePlayerMoveWithBall(player, ball, direction, isItemTranslating);
                    }
                    else
                    {
                        // игрок в поле
                        return HandlePlayerMoveWithOutBall(player, isItemTranslating, direction, isBallCaptured);
                    }
                }
                // игрок НЕ касается соперника и соперник касается мяча
                else
                {
                    // игрок двигается в сторону мяча
                    if (isItemTranslating(player, ball))
                    {
                        // игрок двигается НЕ в сторону соперника
                        if (isItemTranslating(opponent, ball))
                        {
                            // мяч и игрок двигаются НЕ в сторону границы
                            return HandlePlayerMoveWithBall(player, ball, direction, isItemTranslating);
                        }
                    }
                    else
                    {
                        // игрок в поле
                        return HandlePlayerMoveWithOutBall(player, isItemTranslating, direction, isBallCaptured);
                    }
                }
            }
            // игрок НЕ касается мяча и касается соперника
            else if (player.Distance(opponent) <= PlayerWidth)
            {
                // игрок двигается НЕ в сторону соперника
                if (!isItemTranslating(player, opponent))
                {
                    // игрок в поле
                    return HandlePlayerMoveWithOutBall(player, isItemTranslating, direction, false);
                }
            }
            // просто ходим
            else
            {
                // игрок в поле
                return HandlePlayerMoveWithOutBall(player, isItemTranslating, direction, false);
            }

            return (0, 0);
        }

        private (int, int) HandlePlayerTouchesOpponent(
            PointF player, PointF ball, PointF opponent, Func<PointF, PointF, bool> isItemTranslating,
            Direction direction, bool isBallCaptured)
        {
            // игрок двигается в сторону мяча
            if (isItemTranslating(player, ball))
            {
                // игрок двигается НЕ в сторону соперника
                if (!isItemTranslating(player, opponent))
                {
                    // мяч и игрок двигаются НЕ в сторону границы
                    return HandlePlayerMoveWithBall(player, ball, direction, isItemTranslating);
                }
            }
            // игрок двигается НЕ в сторону мяча и НЕ в сторону соперника
            else if (!isItemTranslating(player, opponent))
            {
                // игрок в поле
                return HandlePlayerMoveWithOutBall(player, isItemTranslating, direction, isBallCaptured);
            }

            return (0, 0);
        }

        private (int, int) HandlePlayerMoveWithBall(
            PointF player, PointF ball, Direction direction, Func<PointF, PointF, bool> isItemTranslating)
        {
            // игрок и мяч НЕ покидает пределов поля
            if (!(IsOnBorder(ball.X - BallWidth / 2, ball.Y - BallWidth / 2, BallWidth) &&
                  isItemTranslating(ball, GetBorderPoint(ball.X - BallWidth / 2, ball.Y - BallWidth / 2, BallWidth / 2, direction))) &&
                !(IsOnBorder(player.X - PlayerWidth / 2, player.Y - PlayerWidth / 2, PlayerWidth) &&
                  isItemTranslating(player, GetBorderPoint(player.X - PlayerWidth / 2, player.Y - PlayerWidth / 2, PlayerWidth / 2, direction))))
            {
                //playerTranslator(_translationWithBall);
                //ballTranslator(_translationWithBall);

                return (_translationWithBall, _translationWithBall);
            }

            return (0, 0);
        }

        private (int, int) HandlePlayerMoveWithOutBall(
            PointF player, Func<PointF, PointF, bool> isItemTranslating, Direction direction, bool isBallCaptured)
        {
            // игрок НЕ покидает пределов поля
            if (!(IsOnBorder(player.X - PlayerWidth / 2, player.Y - PlayerWidth / 2, PlayerWidth) &&
                  isItemTranslating(player, GetBorderPoint(player.X - PlayerWidth / 2, player.Y - PlayerWidth / 2, PlayerWidth / 2, direction))))
            {
                if (isBallCaptured)
                {
                    //playerTranslator(_translationWithBall);
                    //ballTranslator(_translationWithBall);

                    return (_translationWithBall, _translationWithBall);
                }
                else
                    // playerTranslator(_translationWithOutBall);
                    return (_translationWithOutBall, 0);
            }

            return (0, 0);
        }

        private bool IsOnBorder(double x, double y, double diameter)
        {
            return (x <= 180 && y > 205 && y <= 300 - diameter) || (x <= 235 && (y <= 205 || y > 300 - diameter)) ||
                   (x >= 955 - diameter && y > 205 && y <= 300 - diameter) || (x >= 905 - diameter && (y <= 205 || y > 300 - diameter)) ||
                   (y <= 30) || (y <= 206 && (x < 230 || x > 910 - diameter)) ||
                   (y >= 475 - diameter) || (y >= 299 - diameter && (x < 230 || x > 910 - diameter));
        }

        private PointF GetBorderPoint(double x, double y, double radius, Direction direction)
        {
            var candidates = GetBorderPointCandidates(x, y, radius);

            if (candidates.Count == 1)
                return candidates[0];

            switch (direction)
            {
                case Direction.Up:
                    var point = candidates.FirstOrDefault(p => p.Y == 30 || p.Y == 206);
                    return point == PointF.Empty ? candidates[0] : point;
                case Direction.Down:
                    point = candidates.FirstOrDefault(p => p.Y == 475 || p.Y == 299);
                    return point == PointF.Empty ? candidates[0] : point;
                case Direction.Right:
                    point = candidates.FirstOrDefault(p => p.X == 905 || p.X == 955);
                    return point == PointF.Empty ? candidates[0] : point;
                case Direction.Left:
                    point = candidates.FirstOrDefault(p => p.X == 235 || p.X == 180);
                    return point == PointF.Empty ? candidates[0] : point;
            }

            return PointF.Empty;
        }

        private List<PointF> GetBorderPointCandidates(double x, double y, double radius)
        {
            var candidates = new List<PointF>();

            // вверхняя граница вне ворот
            if (y <= 30)
                candidates.Add(new PointF((float)(x + radius), 30));

            // левая граница вне ворот
            if (x <= 235 && (y <= 205 || y > 300 - radius * 2))
                candidates.Add(new PointF(235, (float)(y + radius)));

            // верхняя граница в воротах
            if (y <= 206 && (x < 230 || x > 910 - radius * 2))
                candidates.Add(new PointF((float)(x + radius), 206));

            // левая граница в воротах
            if (x <= 180 && y > 205 && y <= 300 - radius * 2)
                candidates.Add(new PointF(180, (float)(y + radius)));

            // нижняя граница в воротах
            if (y >= 299 - radius * 2 && (x < 230 || x > 910 - radius * 2))
                candidates.Add(new PointF((float)(x + radius), 299));

            // нижняя граница вне ворот
            if (y >= 475 - radius * 2)
                candidates.Add(new PointF((float)(x + radius), 475));

            // правая граница вне ворот
            if (x >= 905 - radius * 2 && (y <= 205 || y > 300 - radius * 2))
                candidates.Add(new PointF(905, (float)(y + radius)));

            // правая граница в воротах
            if (x >= 955 - radius * 2 && y > 205 && y <= 300 - radius * 2)
                candidates.Add(new PointF(955, (float)(y + radius)));

            return candidates;
        }
        #endregion

        #region ROTATION
        public (double dx, double dy) RotateBall(
            double angle, PointF player, PointF opponent, PointF ball,
            double ballStartX, double ballStartY, double startTranslationX, double startTranslationY)
        {
            // игрок касается мяча
            if (player.Distance(ball) <= (PlayerWidth + BallWidth) / 2)
            {
                return TryRotate(angle, player, opponent, ball, ballStartX, ballStartY,
                    startTranslationX, startTranslationY, opponent.Distance(ball) <= (PlayerWidth + BallWidth) / 2);
            }

            return (startTranslationX, startTranslationY);
        }

        private (double, double) TryRotate(
            double angle, PointF playerCenter, PointF opponentCenter, PointF ballCenter,
            double ballStartX, double ballStartY, double startTranslationX, double startTranslationY,
            bool checkForOpponent)
        {
            var dx = (ballCenter.X - playerCenter.X) * Math.Cos(angle) -
                (ballCenter.Y - playerCenter.Y) * Math.Sin(angle) + playerCenter.X;
            var dy = (ballCenter.X - playerCenter.X) * Math.Sin(angle) +
                     (ballCenter.Y - playerCenter.Y) * Math.Cos(angle) + playerCenter.Y;

            var newBallCenter = new PointF((float)dx, (float)dy);

            // мяч на краю поля
            if (IsOnBorder(ballCenter.X - BallWidth / 2, ballCenter.Y - BallWidth / 2, BallWidth))
            {
                if (checkForOpponent)
                {
                    // мяч ушёл от края поля и соперник НЕ касается мяча
                    if (newBallCenter.Distance(opponentCenter) > ballCenter.Distance(opponentCenter) &&
                        !IsOnBorder(newBallCenter.X - BallWidth / 2, newBallCenter.Y - BallWidth / 2, BallWidth))
                    {
                        return UpdateTranslation();
                    }
                }
                else
                {
                    // мяч ушёл от края поля
                    if (!IsOnBorder(newBallCenter.X - BallWidth / 2, newBallCenter.Y - BallWidth / 2, BallWidth))
                    {
                        return UpdateTranslation();
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
                        return UpdateTranslation();
                    }
                }
                else
                {
                    return UpdateTranslation();
                }
            }

            (double, double) UpdateTranslation()
            {
                return (dx - ballStartX - BallWidth / 2, dy - ballStartY - BallWidth / 2);
            }

            return (startTranslationX, startTranslationY);
        }
        #endregion
    }
}
