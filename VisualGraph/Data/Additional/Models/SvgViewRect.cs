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
            var mstring = HideInformation ? "": $"<div id=\"ViewInformation\"><p>"+
                    $"<span>{String.Format("Window Width: {0,0:0.00}, Height {1,0:0.00}", ZoomedWidth,ZoomedHeight)}</span></br>"+
                    $"<span>{String.Format("Zoom: {0:0.00}", ZoomLevel)} Zoom step: {ZoomStep}</span></br>" +
                    $"<span>Center: {{{Center.X.ToString(CultureInfo.InvariantCulture)};{Center.X.ToString(CultureInfo.InvariantCulture)}}}</span></p></div>";
            return  new MarkupString(mstring);
        }
    }
}
