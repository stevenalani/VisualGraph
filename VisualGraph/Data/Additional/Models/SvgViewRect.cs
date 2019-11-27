using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace VisualGraph.Data.Additional.Models
{
    public class SvgViewRect
    {   public static double ZoomStep { get; set; } = 1.5;
        public static double MaxZoomLevel { get; set; } = 2000;
        public static double PanSpeed { get; set; } = 1.0;
        public double Width { get; set; }
        public double Height { get; set; }
        public double Ratio => Width / Height;
        public double ZommedWidth => Width / ZoomLevel;
        public double ZoomedHeight => Height / ZoomLevel;
        public Point2 Center { get; set; } = new Point2();
        public double ZoomLevel { get; set; } = 1.0;
        public double TextSize { get; set; } = 10;
        public bool HideInformation { get; set; } = false;
        public Point2 UpperLeft => new Point2(Center.X - (Width/ 2) , Center.Y - (Height/ 2) );
        public string ViewBox => $"{(UpperLeft.X / ZoomLevel).ToString(CultureInfo.InvariantCulture)}," +
                $" {(UpperLeft.Y / ZoomLevel).ToString(CultureInfo.InvariantCulture)}," +
                $" {(Width / ZoomLevel).ToString(CultureInfo.InvariantCulture) }," +
                $" {(Height / ZoomLevel).ToString(CultureInfo.InvariantCulture) }";
       public MarkupString GetDisplayInformationMarkupString(GraphStyleParameters graphStyleParameters)
        {
            var mstring = HideInformation ? "": $"<text x=\"{(UpperLeft.X / ZoomLevel).ToString(CultureInfo.InvariantCulture)}\" y=\"{(UpperLeft.Y / ZoomLevel + 1/ZoomLevel).ToString(CultureInfo.InvariantCulture)}\" fill=\"{graphStyleParameters.TextColor}\" font-size=\"{(TextSize/ ZoomLevel).ToString(CultureInfo.InvariantCulture)}\" dy=\"0\">"+
                    $"<tspan x=\"{(UpperLeft.X / ZoomLevel).ToString(CultureInfo.InvariantCulture)}\" dy=\".6em\">{String.Format("Window Width: {0,0:0.00}, Height {1,0:0.00}", Width,Height)}</tspan>"+
                    $"<tspan x=\"{(UpperLeft.X / ZoomLevel).ToString(CultureInfo.InvariantCulture)}\" dy=\"1.2em\">{String.Format("Zoom: {0:0.00}", ZoomLevel)} Zoom step: {ZoomStep}</tspan>" +
                    $"<tspan x=\"{(UpperLeft.X / ZoomLevel).ToString(CultureInfo.InvariantCulture)}\" dy=\"1.2em\">Center: {{{Center.X.ToString(CultureInfo.InvariantCulture)};{Center.X.ToString(CultureInfo.InvariantCulture)}}}</tspan></text>";
            return  new MarkupString(mstring);
        }
    }
    public class Point2
    {
        public double X { get; set; }
        public double Y { get; set; }
        public Point2()
        {
            X = 0;
            Y = 0;
        }
        public Point2(double x, double y)
        {
            X = x;
            Y = y;
        }
        public static Point2 operator +(Point2 point, Point2 point1) => new Point2(point.X + point1.X, point.Y + point1.Y);
        public static Point2 operator -(Point2 point, Point2 point1) => new Point2(point.X - point1.X, point.Y - point1.Y);
        public double Length => Math.Sqrt(X * X + Y * Y);
        public static double GetLength(Point2 point, Point2 point1) => ((point - point1).Length);
    }
    public struct Point3 
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
