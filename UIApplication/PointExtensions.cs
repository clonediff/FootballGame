namespace UIApplication
{
    public static class PointExtensions
    {
        public static System.Drawing.PointF ToSystemPoint(this Microsoft.Maui.Graphics.Point point)
        {
            return new System.Drawing.PointF()
            {
                X = (float)point.X,
                Y = (float)point.Y
            };
        }
    }
}