using System;

namespace VisualGraph.Data.Additional.Models
{
    public class Point2
    {
        public Point2()
        {
            X = 0;
            Y = 0;
        }

        public Point2(double x, double y)
        {
            this.X = x;
            this.Y = y;
        }
        public Point2(double value)
        {
            X = Y = value;
        }

        public double X { get; set; }
        public double Y { get; set; }

        public static Point2 operator /(Point2 a, double factor)
        {
            return new Point2(a.X / factor, a.Y / factor);
        }
        public static Point2 operator *(Point2 a, double factor)
        {
            return new Point2(a.X * factor, a.Y * factor);
        }
        public static Point2 operator -(Point2 a, Point2 b){
            return new Point2(a.X - b.X, a.Y-b.Y);
        }
        public static Point2 operator +(Point2 a, Point2 b)
        {
            return new Point2(a.X - b.X, a.Y - b.Y);
        }
        public static Point2 operator +(Point2 a)
        {
            return new Point2(Math.Abs(a.X),Math.Abs(a.Y));
        }
        public static Point2 operator -(Point2 a)
        {
            return new Point2( - Math.Abs(a.X), - Math.Abs(a.Y));
        }
    }
}