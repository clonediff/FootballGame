using System.Drawing;
using System.Numerics;

namespace FootballLogicLib
{
    public class Player
    {
        public string Id { get; set; }
        public string TeamName { get; set; }
        public Point Location { get; set; }

        private Point Speed { get; } = new Point(10, 10);

        public void Move(Point direction)
        {
            Location = new Point(   Location.X + Speed.X * direction.X,
                                    Location.Y + Speed.Y * direction.Y);
        }
    }
}