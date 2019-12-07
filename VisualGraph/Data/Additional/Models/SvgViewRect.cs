using Microsoft.AspNetCore.Components;
using System;
using System.Collections.Generic;
using System.Globalization;
using System.Linq;
using System.Threading.Tasks;

namespace VisualGraph.Data.Additional.Models
{
    public class SvgViewRect
    {   public static double ZoomStep { get; set; } = 0.5;
        public static double MaxZoomLevel { get; set; } = 200;
        private double Width { get; set; }
        private double Height { get; set; }
        public double ZoomedWidth => Width / ZoomLevel;
        public double ZoomedHeight => Height / ZoomLevel;
        public Point2 Center { get; set; } = new Point2();
        private double ZoomLevel { get; set; } = 1.0;
        public double TextSize { get; set; } = 10;
        public bool HideInformation { get; set; } = false;
        private Point2 UpperLeft => new Point2(Center.X - (ZoomedWidth/ 2) , Center.Y - (ZoomedHeight/ 2) );

        public void ZoomIn()
        {
            if (ZoomLevel > 0.09)
            {
                ZoomLevel /= 2;
            }
        }
        public void ZoomOut()
        {
            if (ZoomLevel < MaxZoomLevel) { 
                ZoomLevel *= 2;
            }
        }
        public void CropRect(double minX, double minY, double maxX,double maxY)
        {
            var width = maxX - minX;
            var height = maxY - minY;
            if(width > height)
                ZoomLevel = Width / width;
            else
                ZoomLevel = Height / height;
            Center = new Point2(width / 2, height / 2);
        }
        public void SetHeight(double height)
        {
            Height = height;
        }
        public void SetWidth(double width)
        {
            Width = width;
        }
        public void SetCenter(double x, double y)
        {
            var upperleftx = UpperLeft.X;
            var upperlefty = UpperLeft.Y;
            if (x > upperleftx && x < upperleftx + ZoomedWidth)
            {
                Center.X = x;
            }
            if (y > upperlefty && y < upperlefty + ZoomedHeight)
            {
                Center.Y = y;
            }


        }
        public string ViewBox => $"{(UpperLeft.X).ToString(CultureInfo.InvariantCulture)}," +
                $" {(UpperLeft.Y).ToString(CultureInfo.InvariantCulture)}," +
                $" {(ZoomedWidth).ToString(CultureInfo.InvariantCulture) }," +
                $" {(ZoomedHeight).ToString(CultureInfo.InvariantCulture) }";
       public MarkupString GetDisplayInformationMarkupString(GraphStyleParameters graphStyleParameters)
        {
            var mstring = HideInformation ? "": $"<text x=\"{(UpperLeft.X).ToString(CultureInfo.InvariantCulture)}\" y=\"{(UpperLeft.Y +1/ZoomLevel).ToString(CultureInfo.InvariantCulture)}\" fill=\"{graphStyleParameters.TextColor}\" font-size=\"{(TextSize/ ZoomLevel).ToString(CultureInfo.InvariantCulture)}\" dy=\"0\">"+
                    $"<tspan x=\"{(UpperLeft.X).ToString(CultureInfo.InvariantCulture)}\" dy=\".6em\">{String.Format("Window Width: {0,0:0.00}, Height {1,0:0.00}", ZoomedWidth,ZoomedHeight)}</tspan>"+
                    $"<tspan x=\"{(UpperLeft.X).ToString(CultureInfo.InvariantCulture)}\" dy=\"1.2em\">{String.Format("Zoom: {0:0.00}", ZoomLevel)} Zoom step: {ZoomStep}</tspan>" +
                    $"<tspan x=\"{(UpperLeft.X).ToString(CultureInfo.InvariantCulture)}\" dy=\"1.2em\">Center: {{{Center.X.ToString(CultureInfo.InvariantCulture)};{Center.X.ToString(CultureInfo.InvariantCulture)}}}</tspan></text>";
            return  new MarkupString(mstring);
        }
    }
    public struct Point3 
    {
        public double X { get; set; }
        public double Y { get; set; }
        public double Z { get; set; }
    }
}
