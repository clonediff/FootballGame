using System.Drawing;

namespace FootballLogicLib
{
    internal static class PointFExtensions
    {
        public static double Distance(this PointF point, PointF other)
        {
            return (double)Math.Sqrt(Math.Pow(point.X - other.X, 2) + Math.Pow(point.Y - other.Y, 2));
        }
    }
}
